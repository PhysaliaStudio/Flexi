using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public abstract class Actor
    {
        private readonly AbilitySystem abilitySystem;
        private readonly StatOwner owner;

        public int OwnerId => owner.Id;
        internal StatOwner Owner => owner;
        internal IReadOnlyDictionary<int, Stat> Stats => owner.Stats;
        public IReadOnlyList<AbilityFlow> AbilityFlows => owner.AbilityFlows;
        internal IReadOnlyCollection<StatModifierInstance> Modifiers => owner.Modifiers;

        public Actor(AbilitySystem abilitySystem)
        {
            this.abilitySystem = abilitySystem;
            owner = abilitySystem.CreateOwner();
        }

        public bool IsValid()
        {
            return owner.IsValid();
        }

        public void AddStat(int statId, int baseValue)
        {
            owner.AddStat(statId, baseValue);
        }

        public void RemoveStat(int statId)
        {
            owner.RemoveStat(statId);
        }

        public Stat GetStat(int statId)
        {
            return owner.GetStat(statId);
        }

        public void SetStat(int statId, int newBase)
        {
            owner.SetStat(statId, newBase);
        }

        public void ModifyStat(int statId, int value)
        {
            owner.ModifyStat(statId, value);
        }

        public Ability FindAbility(AbilityData abilityData)
        {
            return owner.FindAbility(abilityData);
        }

        public Ability AppendAbility(AbilityData abilityData)
        {
            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            ability.Actor = this;
            owner.AppendAbility(ability);

            IReadOnlyList<AbilityFlow> abilityFlows = ability.Flows;
            for (var i = 0; i < abilityFlows.Count; i++)
            {
                AbilityFlow abilityFlow = abilityFlows[i];
                abilityFlow.SetOwner(this);
                owner.AppendAbilityFlow(abilityFlow);
            }

            return ability;
        }

        public bool RemoveAbility(AbilityData abilityData)
        {
            Ability ability = owner.FindAbility(abilityData);
            if (ability == null)
            {
                return false;
            }

            owner.RemoveAbility(ability);

            IReadOnlyList<AbilityFlow> abilityFlows = Owner.AbilityFlows;
            for (var i = abilityFlows.Count - 1; i >= 0; i--)
            {
                AbilityFlow abilityFlow = abilityFlows[i];
                if (abilityFlow.Ability.Data == abilityData)
                {
                    owner.RemoveAbilityFlowAt(i);
                }
            }

            return true;
        }

        public AbilityFlow FindAbilityFlow(Predicate<AbilityFlow> match)
        {
            return owner.FindAbilityFlow(match);
        }

        public AbilityFlow AppendAbilityFlow(AbilityGraphAsset graphAsset)
        {
            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(graphAsset);
            abilityFlow.SetOwner(this);
            owner.AppendAbilityFlow(abilityFlow);
            return abilityFlow;
        }

        public bool RemoveAbility(Predicate<AbilityFlow> match)
        {
            return owner.RemoveAbilityFlow(match);
        }

        internal void AppendAbilityFlow(AbilityFlow abilityFlow)
        {
            owner.AppendAbilityFlow(abilityFlow);
        }

        internal void RemoveAbilityFlow(AbilityFlow abilityFlow)
        {
            owner.RemoveAbilityFlow(abilityFlow);
        }

        internal void ClearAllAbilityFlow()
        {
            owner.ClearAllAbilityFlows();
        }

        public void AppendModifier(StatModifierInstance modifier)
        {
            owner.AppendModifier(modifier);
        }

        public void RemoveModifier(StatModifierInstance modifier)
        {
            owner.RemoveModifier(modifier);
        }

        public void ClearAllModifiers()
        {
            owner.ClearAllModifiers();
        }

        internal void RefreshStats()
        {
            owner.RefreshStats();
        }

        internal void ResetAllStats()
        {
            owner.ResetAllStats();
        }

        public void Destroy()
        {
            owner.Destroy();
        }
    }
}
