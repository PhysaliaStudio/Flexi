using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public class AbilitySystem
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

        internal Ability InstantiateAbility(AbilityData abilityData)
        {
            var ability = new Ability(this, abilityData);
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

        public AbilityFlow CreateAbilityFlow(AbilityGraphAsset graphAsset)
        {
            AbilityGraph graph = AbilityGraphUtility.Deserialize(graphAsset.name, graphAsset.Text, macroLibrary);
            AbilityFlow flow = new AbilityFlow(this, graph, null);
            return flow;
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
            foreach (AbilityFlow flow in owner.AbilityFlows)
            {
                if (flow.CanExecute(eventContext))
                {
                    EnqueueAbility(flow, eventContext);
                }
            }
        }

        public void EnqueueAbilityAndRun(AbilityFlow flow, IEventContext eventContext)
        {
            EnqueueAbility(flow, eventContext);
            Run();
        }

        public void EnqueueAbility(AbilityFlow flow, IEventContext eventContext)
        {
            flow.Reset();
            flow.SetPayload(eventContext);
            runner.Add(flow);
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
            foreach (AbilityFlow flow in owner.AbilityFlows)
            {
                if (flow.CanExecute(refreshEvent))
                {
                    flow.SetPayload(refreshEvent);
                    flow.Execute();
                }
            }
        }

        internal void TriggerChoice(IChoiceContext context)
        {
            ChoiceOccurred?.Invoke(context);
        }
    }
}
