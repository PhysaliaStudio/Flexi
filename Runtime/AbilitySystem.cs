using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Physalia.AbilityFramework
{
    public class AbilitySystem
    {
        public event Action<object> EventReceived;
        public event Action<ChoiceContext> ChoiceOccurred;

        private readonly StatOwnerRepository ownerRepository;
        private readonly AbilityRunner runner;
        private readonly AbilityEventQueue eventQueue = new();

        private readonly Dictionary<int, string> graphTable = new();

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

        public void LoadAbilityGraph(int id, string graphJson)
        {
            bool success = graphTable.TryAdd(id, graphJson);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] Load graph failed! Already exists graph with Id:{id}");
            }
        }

        public AbilityInstance GetAbilityInstance(int id)
        {
            bool success = graphTable.TryGetValue(id, out string graphJson);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] Get instance failed! Not exists graph with Id:{id}");
                return null;
            }

            AbilityGraph graph = JsonConvert.DeserializeObject<AbilityGraph>(graphJson);
            AbilityInstance instance = new AbilityInstance(id, this, graph);
            return instance;
        }

        public AbilityInstance AppendAbility(IHasStatOwner hasStatOwner, int abilityId)
        {
            return AppendAbility(hasStatOwner.Owner, abilityId);
        }

        public AbilityInstance AppendAbility(StatOwner owner, int abilityId)
        {
            AbilityInstance abilityInstance = GetAbilityInstance(abilityId);
            if (abilityInstance == null)
            {
                return null;
            }

            abilityInstance.SetOwner(owner);
            owner.AppendAbility(abilityInstance);
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

        public void ActivateInstance(AbilityInstance instance, object payload)
        {
            AddToLast(instance, payload);
            Run();
        }

        public void AddEventToLast(object payload)
        {
            eventQueue.Enqueue(payload);
            EventReceived?.Invoke(payload);
        }

        public void TriggerNextEvent()
        {
            if (eventQueue.Count == 0)
            {
                return;
            }

            object payload = eventQueue.Dequeue();
            foreach (StatOwner owner in ownerRepository.Owners)
            {
                foreach (AbilityInstance ability in owner.Abilities)
                {
                    if (ability.CanExecute(payload))
                    {
                        AddToLast(ability, payload);
                    }
                }
            }
        }

        public void AddToLast(AbilityInstance instance, object payload)
        {
            instance.SetPayload(payload);
            runner.Add(instance);
        }

        public void Run()
        {
            _ = runner.Run(this);
        }

        public void ResumeWithContext(NodeContext context)
        {
            _ = runner.ResumeWithContext(this, context);
        }

        public void RefreshStatsAndModifiers()
        {
            ownerRepository.RefreshStatsForAllOwners();
            RefreshModifiers();
        }

        public void RefreshModifiers()
        {
            var payload = new StatRefreshEvent();
            foreach (StatOwner owner in ownerRepository.Owners)
            {
                foreach (AbilityInstance ability in owner.Abilities)
                {
                    if (ability.CanExecute(payload))
                    {
                        ability.SetPayload(payload);
                        ability.Execute();
                    }
                }
            }

            ownerRepository.RefreshStatsForAllOwners();
        }

        public void TriggerChoice(ChoiceContext context)
        {
            ChoiceOccurred?.Invoke(context);
        }
    }
}
