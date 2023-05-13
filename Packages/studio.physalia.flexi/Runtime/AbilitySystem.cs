using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class AbilitySystem
    {
        private static readonly StatRefreshEvent STAT_REFRESH_EVENT = new();

        public event Action<IEventContext> EventOccurred;
        public event Action<IChoiceContext> ChoiceOccurred;

        public Action<IEventContext> EventResolveMethod;

        private readonly StatOwnerRepository ownerRepository;
        private readonly AbilityFlowRunner runner;
        private readonly AbilityEventQueue eventQueue = new();
        private readonly StatRefreshRunner statRefreshRunner = new();

        private readonly MacroLibrary macroLibrary = new();
        private readonly AbilityPoolManager poolManager;

        internal AbilitySystem(StatDefinitionListAsset statDefinitionListAsset, AbilityFlowRunner runner)
        {
            ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
            this.runner = runner;
            runner.abilitySystem = this;

            poolManager = new(this);
        }

        internal StatOwner CreateOwner()
        {
            return ownerRepository.CreateOwner();
        }

        internal void RemoveOwner(StatOwner owner)
        {
            ownerRepository.RemoveOwner(owner);
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

        public void CreateAbilityPool(AbilityData abilityData, int startSize)
        {
            poolManager.CreatePool(abilityData, startSize);
        }

        public void DestroyAbilityPool(AbilityData abilityData)
        {
            poolManager.DestroyPool(abilityData);
        }

        internal AbilityPool GetAbilityPool(AbilityData abilityData)
        {
            return poolManager.GetPool(abilityData);
        }

        public Ability GetAbility(AbilityData abilityData, object userData = null)
        {
            if (poolManager.ContainsPool(abilityData))
            {
                Ability ability = poolManager.GetAbility(abilityData);
                ability.SetUserData(userData);
                return ability;
            }

            return InstantiateAbility(abilityData, userData);
        }

        public void ReleaseAbility(Ability ability)
        {
            if (poolManager.ContainsPool(ability.Data))
            {
                poolManager.ReleaseAbility(ability);
                return;
            }

            ability.Reset();
        }

#if UNITY_5_3_OR_NEWER
        public Ability InstantiateAbility(AbilityAsset abilityAsset, object userData = null)
        {
            return InstantiateAbility(abilityAsset.Data, userData);
        }
#endif

        public Ability InstantiateAbility(AbilityData abilityData, object userData = null)
        {
            var ability = new Ability(this, abilityData, userData);
            ability.Initialize();
            return ability;
        }

        internal AbilityFlow InstantiateAbilityFlow(Ability ability, int index)
        {
            string graphJson = ability.Data.graphJsons[index];
            AbilityGraph graph = AbilityGraphUtility.Deserialize("", graphJson, macroLibrary);
            AbilityFlow flow = new AbilityFlow(this, graph, ability);
            return flow;
        }

        internal void EnqueueEvent(IEventContext eventContext)
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

        public bool TryEnqueueAbility(Ability ability, IEventContext eventContext)
        {
            bool hasAnyEnqueued = false;
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                AbilityFlow abilityFlow = ability.Flows[i];
                if (abilityFlow.IsRunning())
                {
                    continue;
                }

                if (abilityFlow.CanExecute(eventContext))
                {
                    hasAnyEnqueued = true;
                    EnqueueAbilityFlow(abilityFlow, eventContext);
                    break;
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
                for (var j = 0; j < ability.Flows.Count; j++)
                {
                    AbilityFlow abilityFlow = ability.Flows[j];
                    if (abilityFlow.IsRunning())
                    {
                        continue;
                    }

                    if (abilityFlow.CanExecute(eventContext))
                    {
                        // Move to next ability
                        hasAnyEnqueued = true;
                        EnqueueAbilityFlow(abilityFlow, eventContext);
                        break;
                    }
                }
            }

            return hasAnyEnqueued;
        }

        private void EnqueueAbilityFlow(AbilityFlow flow, IEventContext eventContext)
        {
            flow.Reset();
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
            ownerRepository.RefreshStatsForAllOwners();
            DoStatRefreshLogicForAllOwners();
            ownerRepository.RefreshStatsForAllOwners();
        }

        /// <remarks>
        /// StatRefresh does not run with other events and abilities. It runs in another line.
        /// </remarks>
        private void DoStatRefreshLogicForAllOwners()
        {
            IReadOnlyList<StatOwner> owners = ownerRepository.Owners;
            for (var i = 0; i < owners.Count; i++)
            {
                owners[i].ClearAllModifiers();
            }

            for (var i = 0; i < owners.Count; i++)
            {
                StatOwner owner = owners[i];
                for (var j = 0; j < owner.AbilityFlows.Count; j++)
                {
                    AbilityFlow abilityFlow = owner.AbilityFlows[j];
                    if (abilityFlow.CanStatRefresh())
                    {
                        abilityFlow.Reset();
                        abilityFlow.SetPayload(STAT_REFRESH_EVENT);
                        statRefreshRunner.AddFlow(abilityFlow);
                    }
                }
            }

            statRefreshRunner.Start();
        }

        internal void TriggerChoice(IChoiceContext context)
        {
            ChoiceOccurred?.Invoke(context);
        }
    }
}
