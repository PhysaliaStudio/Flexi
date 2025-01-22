using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public interface IAbilitySystemWrapper
    {
        void OnEventReceived(IEventContext eventContext);
        void ResolveEvent(AbilitySystem abilitySystem, IEventContext eventContext);

        IReadOnlyList<StatOwner> CollectStatRefreshOwners();
        IReadOnlyList<AbilityContainer> CollectStatRefreshContainers();

        void OnBeforeCollectModifiers();
        void ApplyModifiers(StatOwner statOwner);
    }

    public class AbilitySystem
    {
        private const int DEFAULT_ABILITY_POOL_SIZE = 2;

        private readonly IAbilitySystemWrapper wrapper;
        private readonly AbilityFlowRunner runner;
        private readonly AbilityEventQueue eventQueue = new();
        private readonly StatRefreshRunner statRefreshRunner = new();

        private readonly MacroLibrary macroLibrary = new();
        private readonly AbilityPoolManager poolManager;

        private readonly List<AbilityFlowOrderList> statRefreshFlowOrderLists = new(4);
        private readonly Dictionary<int, AbilityFlowOrderList> statRefreshFlowOrderListTable = new(4);
        private readonly List<Ability> _cachedAbilites = new(8);

        private readonly Dictionary<Type, EntryHandleTable> entryLookupTable = new(32);

        internal MacroLibrary MacroLibrary => macroLibrary;

        internal AbilitySystem(IAbilitySystemWrapper wrapper, AbilityFlowRunner runner)
        {
            this.wrapper = wrapper;
            this.runner = runner;
            runner.abilitySystem = this;

            poolManager = new(this);
            runner.FlowFinished += OnFlowFinished;
        }

        private void OnFlowFinished(IAbilityFlow flow)
        {
            AbilityFlow abilityFlow = flow as AbilityFlow;
            ReleaseAbility(abilityFlow.Ability);
        }

        public void LoadMacroGraph(string key, MacroAsset macroAsset)
        {
            macroLibrary.Add(key, macroAsset.Text);
        }

        internal AbilityGraph GetMacroGraph(string key)
        {
            bool success = macroLibrary.TryGetValue(key, out string macroJson);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] Get macro failed! key: {key}");
                return null;
            }

            AbilityGraph graph = AbilityGraphUtility.Deserialize("", macroJson, macroLibrary);
            return graph;
        }

        public bool HasAbilityPool(AbilityHandle abilityHandle)
        {
            return poolManager.ContainsPool(abilityHandle);
        }

        public void CreateAbilityPool(AbilityHandle abilityHandle, int startSize)
        {
            poolManager.CreatePool(abilityHandle, startSize);

            // Perf: Cache the event listen handles.
            CacheEntryHandles(abilityHandle);
        }

        public void DestroyAbilityPool(AbilityHandle abilityHandle)
        {
            poolManager.DestroyPool(abilityHandle);

            // Remove cache.
            RemoveEntryHandles(abilityHandle);
        }

        internal AbilityPool GetAbilityPool(AbilityHandle abilityHandle)
        {
            return poolManager.GetPool(abilityHandle);
        }

        internal Ability GetAbility(AbilityData abilityData, int groupIndex)
        {
            AbilityHandle abilityHandle = abilityData.CreateHandle(groupIndex);
            return GetAbility(abilityHandle);
        }

        internal Ability GetAbility(AbilityHandle abilityHandle)
        {
            Ability ability = poolManager.GetAbility(abilityHandle);
            if (ability != null)
            {
                return ability;
            }
            else
            {
                Logger.Warn($"[{nameof(AbilitySystem)}] Create pool with {abilityHandle}. Note that instantiation is <b>VERY</b> expensive!");
                CreateAbilityPool(abilityHandle, DEFAULT_ABILITY_POOL_SIZE);
                ability = poolManager.GetAbility(abilityHandle);
                return ability;
            }
        }

        public Ability GetAbility(AbilityContainer container)
        {
            AbilityHandle abilityHandle = container.Handle;
            if (!abilityHandle.IsValid)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] GetAbility failed! container.Handle is invalid!");
                return null;
            }

            Ability ability = GetAbility(abilityHandle);
            ability.Container = container;
            return ability;
        }

        public void ReleaseAbility(Ability ability)
        {
            bool success = poolManager.ReleaseAbility(ability);
            if (!success)
            {
                ability.Reset();
            }
        }

        public void EnqueueEvent(IEventContext eventContext)
        {
            eventQueue.Enqueue(eventContext);
            wrapper.OnEventReceived(eventContext);
        }

        internal void TriggerCachedEvents(AbilityFlowRunner runner)
        {
            if (eventQueue.Count == 0)
            {
                return;
            }

            runner.BeforeTriggerEvents();
            while (eventQueue.Count > 0)
            {
                IEventContext eventContext = eventQueue.Dequeue();
                wrapper.ResolveEvent(this, eventContext);
            }
            runner.AfterTriggerEvents();
        }

        public bool TryEnqueueAbility(IReadOnlyList<AbilityContainer> containers, IEventContext eventContext = null)
        {
            bool hasAnyEnqueued = false;

            for (var i = 0; i < containers.Count; i++)
            {
                bool hasAnyEnqueuedInThis = TryEnqueueAbility(containers[i], eventContext);
                if (hasAnyEnqueuedInThis)
                {
                    hasAnyEnqueued = true;
                }
            }

            return hasAnyEnqueued;
        }

        public bool TryEnqueueAbility(AbilityContainer container, IEventContext eventContext = null)
        {
            AbilityHandle abilityHandle = container.Handle;
            if (!abilityHandle.IsValid)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] TryEnqueueAbility failed! container.Handle is invalid!");
                return false;
            }

            eventContext ??= EmptyContext.Instance;
            Type eventContextType = eventContext.GetType();
            bool success = entryLookupTable.TryGetValue(eventContextType, out EntryHandleTable handleTable);
            if (!success)
            {
                return false;
            }

            if (!handleTable.TryGetHandles(abilityHandle, out List<EntryHandle> handles))
            {
                return false;
            }

            bool hasAnyEnqueued = false;
            for (var i = 0; i < handles.Count; i++)
            {
                EntryHandle handle = handles[i];

                Ability copy = GetAbility(container);
                AbilityFlow abilityFlow = copy.Flows[handle.flowIndex];
                bool isEntryAvailable = abilityFlow.IsEntryAvailable(handle.entryIndex, eventContext);
                if (isEntryAvailable)
                {
                    hasAnyEnqueued = true;
                    EnqueueAbilityFlow(abilityFlow, handle.entryIndex, eventContext);
                }
                else
                {
                    ReleaseAbility(copy);
                }
            }

            return hasAnyEnqueued;
        }

        private void EnqueueAbilityFlow(AbilityFlow flow, int entryIndex, IEventContext eventContext)
        {
            flow.Reset(entryIndex);
            flow.SetPayload(eventContext);
            runner.AddFlow(flow);
        }

        public void Run()
        {
            runner.Start();
        }

        public void Resume(IResumeContext resumeContext)
        {
            runner.Resume(resumeContext);
        }

        public void Tick()
        {
            runner.Tick();
        }

        public void RefreshStatsAndModifiers()
        {
            // 1. Clear all modifiers and reset all stats.
            IReadOnlyList<StatOwner> owners = wrapper.CollectStatRefreshOwners();
            for (var i = 0; i < owners.Count; i++)
            {
                StatOwner owner = owners[i];
                owner.ClearAllModifiers();
                owner.ResetAllStats();
            }

            // 2. Do user method before collecting modifiers.
            wrapper.OnBeforeCollectModifiers();

            // 3. Do apply all modifiers first, to promise it's at least run once.
            for (var i = 0; i < owners.Count; i++)
            {
                ApplyStatOwnerModifiers(owners[i]);
            }

            // 4. Iterate all containers, and ApplyStatOwnerModifiers layer by layer.
            DoStatRefreshLogicForAllOwners(owners, wrapper.CollectStatRefreshContainers());
        }

        internal void ApplyStatOwnerModifiers(StatOwner statOwner)
        {
            statOwner.ResetAllStats();
            wrapper.ApplyModifiers(statOwner);
        }

        /// <remarks>
        /// StatRefresh does not run with other events and abilities. It runs in another line.
        /// </remarks>
        private void DoStatRefreshLogicForAllOwners(IReadOnlyList<StatOwner> owners, IReadOnlyList<AbilityContainer> containers)
        {
            // If no OnCollectModifierNode, just return.
            bool success = entryLookupTable.TryGetValue(typeof(OnCollectModifierNode.Context), out EntryHandleTable handleTable);
            if (!success)
            {
                return;
            }

            for (var i = 0; i < containers.Count; i++)
            {
                AbilityContainer container = containers[i];
                if (!handleTable.TryGetHandles(container.Handle, out List<EntryHandle> handles))
                {
                    continue;
                }

                for (var handleIndex = 0; handleIndex < handles.Count; handleIndex++)
                {
                    EntryHandle handle = handles[handleIndex];

                    // Get another copy and setup the flow.
                    Ability copy = GetAbility(container.Handle);
                    copy.Container = container;

                    AbilityFlow copyFlow = copy.Flows[handle.flowIndex];
                    copyFlow.Reset(handle.entryIndex);
                    copyFlow.SetPayload(OnCollectModifierNode.Context.Instance);

                    // Then add into the correct order list.
                    int nodeOrder = handle.order;
                    AbilityFlowOrderList orderList = GetStatRefreshFlowOrderList(nodeOrder);
                    orderList.Add(copyFlow);

                    _cachedAbilites.Add(copy);
                }
            }

            // Run all flows in order.
            statRefreshFlowOrderLists.Sort((a, b) => a.Order.CompareTo(b.Order));
            for (var i = 0; i < statRefreshFlowOrderLists.Count; i++)
            {
                AbilityFlowOrderList orderList = statRefreshFlowOrderLists[i];
                if (orderList.Count > 0)
                {
                    RunStatRefreshFlows(orderList);
                    for (var ownerIndex = 0; ownerIndex < owners.Count; ownerIndex++)
                    {
                        ApplyStatOwnerModifiers(owners[ownerIndex]);
                    }
                }
            }

            // Clean up
            for (var i = 0; i < _cachedAbilites.Count; i++)
            {
                Ability ability = _cachedAbilites[i];
                ReleaseAbility(ability);
            }

            CleaupStatRefreshFlows();
            _cachedAbilites.Clear();

            AbilityFlowOrderList GetStatRefreshFlowOrderList(int order)
            {
                if (!statRefreshFlowOrderListTable.TryGetValue(order, out AbilityFlowOrderList list))
                {
                    list = new AbilityFlowOrderList(order, 8);
                    statRefreshFlowOrderLists.Add(list);
                    statRefreshFlowOrderListTable.Add(order, list);
                }
                return list;
            }

            void RunStatRefreshFlows(AbilityFlowOrderList orderList)
            {
                for (var i = 0; i < orderList.Count; i++)
                {
                    statRefreshRunner.AddFlow(orderList[i]);
                }

                statRefreshRunner.Start();
            }

            void CleaupStatRefreshFlows()
            {
                for (var i = 0; i < statRefreshFlowOrderLists.Count; i++)
                {
                    statRefreshFlowOrderLists[i].Clear();
                }
            }
        }

        private void CacheEntryHandles(AbilityHandle abilityHandle)
        {
            // Get a copy for iterating.
            Ability ability = GetAbility(abilityHandle);

            // Iterate all entry nodes to find all StatRefreshEventNode.
            IReadOnlyList<AbilityFlow> abilityFlows = ability.Flows;

            int flowCount = abilityFlows.Count;
            for (var indexOfFlow = 0; indexOfFlow < flowCount; indexOfFlow++)
            {
                AbilityFlow abilityFlow = abilityFlows[indexOfFlow];
                IReadOnlyList<EntryNode> entryNodes = abilityFlow.Graph.EntryNodes;

                int entryNodeCount = entryNodes.Count;
                for (var indexOfEntry = 0; indexOfEntry < entryNodeCount; indexOfEntry++)
                {
                    Type contextType = entryNodes[indexOfEntry].ContextType;
                    if (contextType == null)
                    {
                        continue;
                    }

                    if (!entryLookupTable.TryGetValue(contextType, out EntryHandleTable handleTable))
                    {
                        handleTable = new EntryHandleTable();
                        entryLookupTable.Add(contextType, handleTable);
                    }

                    if (entryNodes[indexOfEntry] is OnCollectModifierNode statRefreshEventNode)
                    {
                        handleTable.Add(abilityHandle, indexOfFlow, indexOfEntry, statRefreshEventNode.order.Value);
                    }
                    else
                    {
                        handleTable.Add(abilityHandle, indexOfFlow, indexOfEntry, 0);
                    }
                }
            }

            // Always release the ability copy which only for iterating.
            ReleaseAbility(ability);
        }

        private void RemoveEntryHandles(AbilityHandle abilityHandle)
        {
            foreach (EntryHandleTable handleTable in entryLookupTable.Values)
            {
                handleTable.Remove(abilityHandle);
            }
        }
    }
}
