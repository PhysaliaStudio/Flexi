using System;
using System.Collections.Generic;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    public class AbilitySystem : ICreateStatOwner
    {
        public event Action<IEventContext> EventReceived;
        public event Action<IChoiceContext> ChoiceOccurred;

        private readonly StatOwnerRepository ownerRepository;
        private readonly AbilityRunner runner;
        private readonly AbilityEventQueue eventQueue = new();

        private readonly MacroLibrary macroLibrary = new();
        private readonly Dictionary<int, AbilityGraphAsset> graphTable = new();

        private IEnumerable<Actor> overridedIteratorGetter;

        internal AbilitySystem(StatDefinitionListAsset statDefinitionListAsset, AbilityRunner runner)
        {
            ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
            this.runner = runner;
        }

        public StatOwner CreateOwner()
        {
            return ownerRepository.CreateOwner();
        }

        public void RemoveOwner(StatOwner owner)
        {
            ownerRepository.RemoveOwner(owner);
        }

        public StatOwner GetOwner(int id)
        {
            return ownerRepository.GetOwner(id);
        }

        public void LoadMacroGraph(string key, MacroGraphAsset macroGraphAsset)
        {
            macroLibrary.Add(key, macroGraphAsset.Text);
        }

        public void LoadAbilityGraph(int id, string graphName, string graphJson)
        {
            AbilityGraphAsset graphAsset = ScriptableObject.CreateInstance<AbilityGraphAsset>();
            graphAsset.name = graphName;
            graphAsset.Text = graphJson;

            LoadAbilityGraph(id, graphAsset);
        }

        public void LoadAbilityGraph(int id, AbilityGraphAsset graphAsset)
        {
            bool success = graphTable.TryAdd(id, graphAsset);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] Load graph failed! Already exists graph with Id:{id}");
                return;
            }

            // Deserialize once to log potential errors
            _ = AbilityGraphUtility.Deserialize(graphAsset.name, graphAsset.Text, macroLibrary);
        }

        public AbilityGraph GetMacroGraph(string key)
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

        public AbilityInstance GetAbilityInstance(int id)
        {
            bool success = graphTable.TryGetValue(id, out AbilityGraphAsset graphAsset);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] Get instance failed! Not exists graph with Id:{id}");
                return null;
            }

            AbilityGraph graph = AbilityGraphUtility.Deserialize(graphAsset.name, graphAsset.Text, macroLibrary);
            AbilityInstance instance = new AbilityInstance(id, this, graph);
            return instance;
        }

        public AbilityInstance AppendAbility(Actor actor, int abilityId)
        {
            AbilityInstance abilityInstance = GetAbilityInstance(abilityId);
            if (abilityInstance == null)
            {
                return null;
            }

            abilityInstance.SetOwner(actor);
            actor.AppendAbility(abilityInstance);
            return abilityInstance;
        }

        public void RemoveAbility(StatOwner owner, int abilityId)
        {
            owner.RemoveAbility(abilityId);
        }

        public void ClearAllAbilities(StatOwner owner)
        {
            owner.ClearAllAbilities();
        }

        public void OverrideIterator(IEnumerable<Actor> iterator)
        {
            overridedIteratorGetter = iterator;
        }

        internal void EnqueueEvent(IEventContext eventContext)
        {
            eventQueue.Enqueue(eventContext);
            EventReceived?.Invoke(eventContext);
        }

        internal void TriggerCachedEvents()
        {
            if (eventQueue.Count == 0)
            {
                return;
            }

            runner.PushNewAbilityQueue();
            while (eventQueue.Count > 0)
            {
                IEventContext eventContext = eventQueue.Dequeue();
                IterateAbilitiesFromStatOwners(eventContext);
            }
            runner.PopEmptyQueues();
        }

        private void IterateAbilitiesFromStatOwners(IEventContext eventContext)
        {
            if (overridedIteratorGetter != null)
            {
                IEnumerator<Actor> enumerator = overridedIteratorGetter.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Actor actor = enumerator.Current;
                    EnqueueAbilityIfAble(actor.Owner, eventContext);
                }
            }
            else
            {
                foreach (StatOwner owner in ownerRepository.Owners)
                {
                    EnqueueAbilityIfAble(owner, eventContext);
                }
            }
        }

        private void EnqueueAbilityIfAble(StatOwner owner, IEventContext eventContext)
        {
            foreach (AbilityInstance ability in owner.Abilities)
            {
                if (ability.CanExecute(eventContext))
                {
                    EnqueueAbility(ability, eventContext);
                }
            }
        }

        public void EnqueueAbilityAndRun(AbilityInstance instance, IEventContext eventContext)
        {
            EnqueueAbility(instance, eventContext);
            Run();
        }

        public void EnqueueAbility(AbilityInstance instance, IEventContext eventContext)
        {
            instance.Reset();
            instance.SetPayload(eventContext);
            runner.Add(instance);
        }

        public void Run()
        {
            runner.Run(this);
        }

        public void Resume(IResumeContext resumeContext)
        {
            runner.Resume(this, resumeContext);
        }

        public void RefreshStatsAndModifiers()
        {
            ownerRepository.RefreshStatsForAllOwners();
            RefreshModifiers();
        }

        public void RefreshModifiers()
        {
            var refreshEvent = new StatRefreshEvent();
            IterateModifierCheckFromStatOwners(refreshEvent);
            ownerRepository.RefreshStatsForAllOwners();
        }

        private void IterateModifierCheckFromStatOwners(StatRefreshEvent refreshEvent)
        {
            if (overridedIteratorGetter != null)
            {
                IEnumerator<Actor> enumerator = overridedIteratorGetter.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Actor actor = enumerator.Current;
                    CheckModifiers(actor.Owner, refreshEvent);
                }
            }
            else
            {
                foreach (StatOwner owner in ownerRepository.Owners)
                {
                    CheckModifiers(owner, refreshEvent);
                }
            }
        }

        private void CheckModifiers(StatOwner owner, StatRefreshEvent refreshEvent)
        {
            foreach (AbilityInstance ability in owner.Abilities)
            {
                if (ability.CanExecute(refreshEvent))
                {
                    ability.SetPayload(refreshEvent);
                    ability.Execute();
                }
            }
        }

        internal void TriggerChoice(IChoiceContext context)
        {
            ChoiceOccurred?.Invoke(context);
        }
    }
}
