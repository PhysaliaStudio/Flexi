using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class AbilitySystem
    {
        private static readonly StatRefreshEvent STAT_REFRESH_EVENT = new();
        private const int DEFAULT_ABILITY_POOL_SIZE = 2;

        public event Action<IEventContext> EventOccurred;
        public event Action<IChoiceContext> ChoiceOccurred;

        public Action<IEventContext> EventResolveMethod;

        private readonly StatOwnerRepository ownerRepository;
        private readonly ActorRepository actorRepository;
        private readonly AbilityFlowRunner runner;
        private readonly AbilityEventQueue eventQueue = new();
        private readonly StatRefreshRunner statRefreshRunner = new();

        private readonly MacroLibrary macroLibrary = new();
        private readonly AbilityPoolManager poolManager;

        private readonly List<AbilityFlowOrderList> statRefreshFlowOrderLists = new(4);
        private readonly Dictionary<int, AbilityFlowOrderList> statRefreshFlowOrderListTable = new(4);
        private readonly List<Ability> _cachedAbilites = new(8);

        private readonly Dictionary<Type, EntryHandleTable> entryLookupTable = new(32);

        internal AbilitySystem(IStatsRefreshAlgorithm statsRefreshAlgorithm, AbilityFlowRunner runner)
        {
            ownerRepository = new StatOwnerRepository();
            actorRepository = new ActorRepository(statsRefreshAlgorithm);
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

        internal StatOwner CreateOwner(Actor actor)
        {
            StatOwner owner = ownerRepository.CreateOwner();
            actorRepository.AddActor(owner.Id, actor);
            return owner;
        }

        internal void DestroyOwner(Actor actor)
        {
            StatOwner owner = actor.Owner;
            actorRepository.RemoveActor(owner.Id);
            owner.Destroy();
        }

        internal Actor GetActor(int id)
        {
            return actorRepository.GetActor(id);
        }

        internal StatOwner GetOwner(int id)
        {
            return ownerRepository.GetOwner(id);
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

        public bool HasAbilityPool(AbilityDataSource abilityDataSource)
        {
            return poolManager.ContainsPool(abilityDataSource);
        }

        public void CreateAbilityPool(AbilityDataSource abilityDataSource, int startSize)
        {
            poolManager.CreatePool(abilityDataSource, startSize);

            // Perf: Cache the event listen handles.
            CacheEntryHandles(abilityDataSource);
        }

        public void DestroyAbilityPool(AbilityDataSource abilityDataSource)
        {
            poolManager.DestroyPool(abilityDataSource);

            // Remove cache.
            RemoveEntryHandles(abilityDataSource);
        }

        internal AbilityPool GetAbilityPool(AbilityDataSource abilityDataSource)
        {
            return poolManager.GetPool(abilityDataSource);
        }

        internal Ability GetAbility(AbilityData abilityData, int groupIndex)
        {
            AbilityDataSource abilityDataSource = abilityData.CreateDataSource(groupIndex);
            return GetAbility(abilityDataSource);
        }

        internal Ability GetAbility(AbilityDataSource abilityDataSource)
        {
            Ability ability = poolManager.GetAbility(abilityDataSource);
            if (ability != null)
            {
                return ability;
            }
            else
            {
                Logger.Warn($"[{nameof(AbilitySystem)}] Create pool with {abilityDataSource}. Note that instantiation is <b>VERY</b> expensive!");
                CreateAbilityPool(abilityDataSource, DEFAULT_ABILITY_POOL_SIZE);
                ability = poolManager.GetAbility(abilityDataSource);
                return ability;
            }
        }

        public Ability GetAbility(AbilityDataContainer container)
        {
            AbilityDataSource abilityDataSource = container.DataSource;
            if (!abilityDataSource.IsValid)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] GetAbility failed! container.DataSource is invalid!");
                return null;
            }

            Ability ability = GetAbility(abilityDataSource);
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

        internal Ability InstantiateAbility(AbilityDataSource abilityDataSource)
        {
            var ability = new Ability(this, abilityDataSource);
            ability.Initialize();
            return ability;
        }

        internal AbilityFlow InstantiateAbilityFlow(Ability ability, string json)
        {
            AbilityGraph graph = AbilityGraphUtility.Deserialize("", json, macroLibrary);
            AbilityFlow flow = new AbilityFlow(this, graph, ability);
            return flow;
        }

        public void EnqueueEvent(IEventContext eventContext)
        {
            eventQueue.Enqueue(eventContext);
            EventOccurred?.Invoke(eventContext);
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
                if (EventResolveMethod != null)
                {
                    EventResolveMethod.Invoke(eventContext);
                }
                else
                {
                    EnqueueAbilitiesForAllOwners(eventContext);
                }
            }
            runner.AfterTriggerEvents();
        }

        private void EnqueueAbilitiesForAllOwners(IEventContext eventContext)
        {
            IReadOnlyList<Actor> actors = actorRepository.Actors;
            for (var i = 0; i < actors.Count; i++)
            {
                _ = TryEnqueueAbility(actors[i], eventContext);
            }
        }

        public bool TryEnqueueAbility(Actor actor, IEventContext eventContext = null)
        {
            IReadOnlyList<AbilityDataContainer> containers = actor.AbilityDataContainers;
            return TryEnqueueAbility(containers, eventContext);
        }

        public bool TryEnqueueAbility(IReadOnlyList<AbilityDataContainer> containers, IEventContext eventContext = null)
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

        public bool TryEnqueueAbility(AbilityDataContainer container, IEventContext eventContext = null)
        {
            AbilityDataSource abilityDataSource = container.DataSource;
            if (!abilityDataSource.IsValid)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] TryEnqueueAbility failed! container.DataSource is invalid!");
                return false;
            }

            eventContext ??= EmptyContext.Instance;
            Type eventContextType = eventContext.GetType();
            bool success = entryLookupTable.TryGetValue(eventContextType, out EntryHandleTable handleTable);
            if (!success)
            {
                // This might not an error, since some event might not be used.
                Logger.Warn($"[{nameof(AbilitySystem)}] TryEnqueueAbility failed! EventContext Type: '{eventContextType}' is not registered!");
                return false;
            }

            if (!handleTable.TryGetHandles(abilityDataSource, out List<EntryHandle> handles))
            {
                Logger.Error($"[{nameof(AbilitySystem)}] TryEnqueueAbility failed! AbilityDataSource: '{abilityDataSource}' is not registered!");
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
            for (var i = 0; i < actorRepository.Actors.Count; i++)
            {
                Actor actor = actorRepository.Actors[i];
                actor.ClearAllModifiers();
                actor.ResetAllStats();
            }

            DoStatRefreshLogicForAllOwners();
            actorRepository.RefreshStatsForAll();
        }

        internal void RefreshStats(Actor actor)
        {
            actorRepository.RefreshStats(actor);
        }

        /// <remarks>
        /// StatRefresh does not run with other events and abilities. It runs in another line.
        /// </remarks>
        private void DoStatRefreshLogicForAllOwners()
        {
            actorRepository.OnBeforeCollectModifiersForAll();

            IReadOnlyList<Actor> actors = actorRepository.Actors;
            for (var i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];

                IReadOnlyList<AbilityDataContainer> containers = actor.AbilityDataContainers;
                for (var j = 0; j < containers.Count; j++)
                {
                    AbilityDataContainer container = containers[j];

                    bool success = entryLookupTable.TryGetValue(typeof(StatRefreshEvent), out EntryHandleTable handleTable);
                    if (!success)
                    {
                        continue;
                    }

                    if (!handleTable.TryGetHandles(container.DataSource, out List<EntryHandle> handles))
                    {
                        continue;
                    }

                    for (var handleIndex = 0; handleIndex < handles.Count; handleIndex++)
                    {
                        EntryHandle handle = handles[handleIndex];

                        // Get another copy and setup the flow.
                        Ability copy = GetAbility(container.DataSource);
                        copy.Container = container;

                        AbilityFlow copyFlow = copy.Flows[handle.flowIndex];
                        copyFlow.Reset(handle.entryIndex);
                        copyFlow.SetPayload(STAT_REFRESH_EVENT);

                        // Then add into the correct order list.
                        int nodeOrder = handle.order;
                        AbilityFlowOrderList orderList = GetStatRefreshFlowOrderList(nodeOrder);
                        orderList.Add(copyFlow);

                        _cachedAbilites.Add(copy);
                    }
                }
            }

            // Run all flows in order.
            statRefreshFlowOrderLists.Sort((a, b) => a.Order.CompareTo(b.Order));
            for (var i = 0; i < statRefreshFlowOrderLists.Count; i++)
            {
                AbilityFlowOrderList orderList = statRefreshFlowOrderLists[i];
                RunStatRefreshFlows(orderList);
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
                if (orderList.Count == 0)
                {
                    return;
                }

                for (var i = 0; i < orderList.Count; i++)
                {
                    statRefreshRunner.AddFlow(orderList[i]);
                }

                statRefreshRunner.Start();
                actorRepository.RefreshStatsForAll();
            }

            void CleaupStatRefreshFlows()
            {
                for (var i = 0; i < statRefreshFlowOrderLists.Count; i++)
                {
                    statRefreshFlowOrderLists[i].Clear();
                }
            }
        }

        internal void TriggerChoice(IChoiceContext context)
        {
            ChoiceOccurred?.Invoke(context);
        }

        private void CacheEntryHandles(AbilityDataSource abilityDataSource)
        {
            // Get a copy for iterating.
            Ability ability = GetAbility(abilityDataSource);

            // Iterate all entry nodes to find all StatRefreshEventNode.
            IReadOnlyList<AbilityFlow> abilityFlows = ability.Flows;

            int flowCount = abilityFlows.Count;
            for (var indexOfFlow = 0; indexOfFlow < flowCount; indexOfFlow++)
            {
                AbilityFlow abilityFlow = abilityFlows[indexOfFlow];
                IReadOnlyList<EntryNodeBase> entryNodes = abilityFlow.Graph.EntryNodes;

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

                    if (entryNodes[indexOfEntry] is StatRefreshEventNode statRefreshEventNode)
                    {
                        handleTable.Add(abilityDataSource, indexOfFlow, indexOfEntry, statRefreshEventNode.order.Value);
                    }
                    else
                    {
                        handleTable.Add(abilityDataSource, indexOfFlow, indexOfEntry, 0);
                    }
                }
            }

            // Always release the ability copy which only for iterating.
            ReleaseAbility(ability);
        }

        private void RemoveEntryHandles(AbilityDataSource abilityDataSource)
        {
            foreach (EntryHandleTable handleTable in entryLookupTable.Values)
            {
                handleTable.Remove(abilityDataSource);
            }
        }
    }
}
