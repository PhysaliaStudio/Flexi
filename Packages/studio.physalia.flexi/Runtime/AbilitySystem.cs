using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class AbilitySystem
    {
        private static readonly StatRefreshEvent STAT_REFRESH_EVENT = new();

        public event Action<IEventContext> EventOccurred;
        public event Action<IChoiceContext> ChoiceOccurred;
        public event Action<Ability> AbilityFinished;

        public Action<IEventContext> EventResolveMethod;

        private readonly StatOwnerRepository ownerRepository;
        private readonly ActorRepository actorRepository;
        private readonly AbilityFlowRunner runner;
        private readonly AbilityEventQueue eventQueue = new();
        private readonly StatRefreshRunner statRefreshRunner = new();

        private readonly MacroLibrary macroLibrary = new();
        private readonly AbilityPoolManager poolManager;

        private readonly List<Ability> _cachedAbilites = new(8);

        internal AbilitySystem(IStatsRefreshAlgorithm statsRefreshAlgorithm, AbilityFlowRunner runner)
        {
            ownerRepository = new StatOwnerRepository();
            actorRepository = new ActorRepository(statsRefreshAlgorithm);
            this.runner = runner;
            runner.abilitySystem = this;

            poolManager = new(this);
            runner.StepExecuted += OnFlowStepExecuted;
        }

        private void OnFlowStepExecuted(AbilityFlowStepper.StepResult stepResult)
        {
            if (stepResult.type == AbilityFlowStepper.ExecutionType.FLOW_FINISH)
            {
                AbilityFlow flow = stepResult.flow as AbilityFlow;
                AbilityFinished?.Invoke(flow.Ability);
            }
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

        public Ability GetAbility(AbilityData abilityData, int groupIndex, object userData = null)
        {
            AbilityDataSource abilityDataSource = abilityData.CreateDataSource(groupIndex);
            return GetAbility(abilityDataSource, userData);
        }

        public Ability GetAbility(AbilityDataSource abilityDataSource, object userData = null)
        {
            if (poolManager.ContainsPool(abilityDataSource))
            {
                Ability ability = poolManager.GetAbility(abilityDataSource);
                ability.SetUserData(userData);
                return ability;
            }

            return InstantiateAbility(abilityDataSource, userData);
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

        internal Ability InstantiateAbility(AbilityDataSource abilityDataSource, object userData = null)
        {
            var ability = new Ability(this, abilityDataSource, userData);
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

            IReadOnlyList<StatOwner> owners = ownerRepository.Owners;
            for (var i = 0; i < owners.Count; i++)
            {
                StatOwner owner = owners[i];
                _ = TryEnqueueAbility(owner.Abilities, eventContext);
            }
        }

        public bool TryEnqueueAndRunAbility(Ability ability, IEventContext eventContext)
        {
            bool success = TryEnqueueAbility(ability, eventContext);
            if (success)
            {
                Run();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryEnqueueAndRunAbility(IReadOnlyList<Ability> abilities, IEventContext eventContext)
        {
            bool success = TryEnqueueAbility(abilities, eventContext);
            if (success)
            {
                Run();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryEnqueueAbility(Actor actor, IEventContext eventContext)
        {
            bool hasAnyEnqueued = false;

            IReadOnlyList<AbilityDataSource> abilityDataSources = actor.AbilityDataSources;
            for (var i = 0; i < abilityDataSources.Count; i++)
            {
                AbilityDataSource abilityDataSource = abilityDataSources[i];
                bool hasAnyEnqueuedInThis = TryEnqueueAbility(actor, abilityDataSource, eventContext);
                if (hasAnyEnqueuedInThis)
                {
                    hasAnyEnqueued = true;
                }
            }

            return hasAnyEnqueued;
        }

        public bool TryEnqueueAbility(Actor actor, AbilityDataSource abilityDataSource, IEventContext eventContext)
        {
            Ability ability = GetAbility(abilityDataSource, eventContext);
            ability.Actor = actor;

            bool success = TryEnqueueAbility(ability, eventContext);
            if (!success)
            {
                ability.Actor = null;
                ReleaseAbility(ability);
            }

            return success;
        }

        public bool TryEnqueueAbility(Ability ability, IEventContext eventContext)
        {
            bool hasAnyEnqueued = false;
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                AbilityFlow abilityFlow = ability.Flows[i];
                if (runner.IsFlowRunning(abilityFlow))
                {
                    continue;
                }

                if (!abilityFlow.IsEnable)
                {
                    continue;
                }

                int entryIndex = abilityFlow.GetAvailableEntry(eventContext);
                if (entryIndex != -1)
                {
                    hasAnyEnqueued = true;
                    EnqueueAbilityFlow(abilityFlow, entryIndex, eventContext);
                }
            }

            return hasAnyEnqueued;
        }

        public bool TryEnqueueAbility(IReadOnlyList<Ability> abilities, IEventContext eventContext)
        {
            bool hasAnyEnqueued = false;
            for (var i = 0; i < abilities.Count; i++)
            {
                Ability ability = abilities[i];
                bool hasAnyEnqueuedInThis = TryEnqueueAbility(ability, eventContext);
                if (hasAnyEnqueuedInThis)
                {
                    hasAnyEnqueued = true;
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
            // TODO: 3 layers of for loop! Need to optimize.
            IReadOnlyList<Actor> actors = actorRepository.Actors;
            for (var i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                for (var j = 0; j < actor.AbilityDataSources.Count; j++)
                {
                    AbilityDataSource abilityDataSource = actor.AbilityDataSources[j];
                    Ability ability = GetAbility(abilityDataSource, STAT_REFRESH_EVENT);
                    ability.Actor = actor;

                    bool anySuccess = false;
                    for (var k = 0; k < ability.Flows.Count; k++)
                    {
                        AbilityFlow abilityFlow = ability.Flows[k];
                        if (!abilityFlow.IsEnable)
                        {
                            continue;
                        }

                        if (abilityFlow.CanStatRefresh())
                        {
                            anySuccess = true;
                            abilityFlow.Reset();
                            abilityFlow.SetPayload(STAT_REFRESH_EVENT);
                            statRefreshRunner.AddFlow(abilityFlow);
                        }
                    }

                    if (anySuccess)
                    {
                        _cachedAbilites.Add(ability);
                    }
                    else
                    {
                        ability.Actor = null;
                        ReleaseAbility(ability);
                    }
                }
            }

            IReadOnlyList<StatOwner> owners = ownerRepository.Owners;
            for (var i = 0; i < owners.Count; i++)
            {
                StatOwner owner = owners[i];
                for (var j = 0; j < owner.AbilityFlows.Count; j++)
                {
                    AbilityFlow abilityFlow = owner.AbilityFlows[j];
                    if (!abilityFlow.IsEnable)
                    {
                        continue;
                    }

                    if (abilityFlow.CanStatRefresh())
                    {
                        abilityFlow.Reset();
                        abilityFlow.SetPayload(STAT_REFRESH_EVENT);
                        statRefreshRunner.AddFlow(abilityFlow);
                    }
                }
            }

            statRefreshRunner.Start();
            for (var i = 0; i < _cachedAbilites.Count; i++)
            {
                Ability ability = _cachedAbilites[i];
                ability.Actor = null;
                ReleaseAbility(ability);
            }
            _cachedAbilites.Clear();
        }

        internal void TriggerChoice(IChoiceContext context)
        {
            ChoiceOccurred?.Invoke(context);
        }
    }
}
