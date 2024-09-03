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
        }

        public void DestroyAbilityPool(AbilityDataSource abilityDataSource)
        {
            poolManager.DestroyPool(abilityDataSource);
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
            if (!poolManager.ContainsPool(abilityDataSource))
            {
                Logger.Warn($"[{nameof(AbilitySystem)}] Create pool with {abilityDataSource}. Note that instantiation is <b>VERY</b> expensive!");
                CreateAbilityPool(abilityDataSource, DEFAULT_ABILITY_POOL_SIZE);
            }

            Ability ability = poolManager.GetAbility(abilityDataSource);
            return ability;
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
            if (poolManager.ContainsPool(ability.DataSource))
            {
                poolManager.ReleaseAbility(ability);
                return;
            }

            ability.Reset();
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

        public bool TryEnqueueAbility(Actor actor, IEventContext eventContext)
        {
            IReadOnlyList<AbilityDataContainer> containers = actor.AbilityDataContainers;
            return TryEnqueueAbility(containers, eventContext);
        }

        public bool TryEnqueueAbility(IReadOnlyList<AbilityDataContainer> containers, IEventContext eventContext)
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

        public bool TryEnqueueAbility(AbilityDataContainer container, IEventContext eventContext)
        {
            AbilityDataSource abilityDataSource = container.DataSource;
            if (!abilityDataSource.IsValid)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] TryEnqueueAbility failed! container.DataSource is invalid!");
                return false;
            }

            // Get a copy for iterating.
            Ability ability = GetAbility(container);

            Ability copy = null;
            bool hasAnyEnqueued = false;
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                copy ??= GetAbility(container);

                AbilityFlow abilityFlow = copy.Flows[i];
                int entryIndex = abilityFlow.GetAvailableEntry(eventContext);
                if (entryIndex != -1)
                {
                    hasAnyEnqueued = true;
                    EnqueueAbilityFlow(abilityFlow, entryIndex, eventContext);
                    copy = null;
                }
            }

            ReleaseAbility(ability);
            if (copy != null)
            {
                ReleaseAbility(copy);
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

            // TODO: 4 layers of for loop! Should we need to optimize?
            IReadOnlyList<Actor> actors = actorRepository.Actors;
            for (var i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                for (var j = 0; j < actor.AbilityDataContainers.Count; j++)
                {
                    AbilityDataContainer container = actor.AbilityDataContainers[j];

                    // Get a copy for iterating.
                    Ability ability = GetAbility(container);

                    // Iterate all entry nodes to find all StatRefreshEventNode.
                    for (var indexOfFlow = 0; indexOfFlow < ability.Flows.Count; indexOfFlow++)
                    {
                        AbilityFlow abilityFlow = ability.Flows[indexOfFlow];
                        IReadOnlyList<EntryNode> entryNodes = abilityFlow.Graph.EntryNodes;
                        for (var indexOfEntry = 0; indexOfEntry < entryNodes.Count; indexOfEntry++)
                        {
                            if (entryNodes[indexOfEntry] is StatRefreshEventNode node)
                            {
                                // If found, get another copy and setup the flow.
                                Ability copy = GetAbility(container.DataSource);
                                copy.Container = container;

                                AbilityFlow copyFlow = copy.Flows[indexOfFlow];
                                copyFlow.Reset(indexOfEntry);
                                copyFlow.SetPayload(STAT_REFRESH_EVENT);

                                // Then add into the correct order list.
                                int nodeOrder = node.order.Value;
                                AbilityFlowOrderList orderList = GetStatRefreshFlowOrderList(nodeOrder);
                                orderList.Add(copyFlow);

                                _cachedAbilites.Add(copy);
                            }
                        }
                    }

                    // Always release the ability copy which only for iterating.
                    ReleaseAbility(ability);
                }
            }

            // Run all flows in order.
            statRefreshFlowOrderLists.Sort();
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
    }
}
