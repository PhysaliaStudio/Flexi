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
        public IReadOnlyList<AbilityInstance> Abilities => owner.Abilities;
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

        public AbilityInstance FindAbility(int abilityId)
        {
            return owner.FindAbility(abilityId);
        }

        public AbilityInstance FindAbility(Predicate<AbilityInstance> match)
        {
            return owner.FindAbility(match);
        }

        public AbilityInstance AppendAbility(AbilityGraphAsset graphAsset)
        {
            AbilityInstance instance = abilitySystem.CreateAbilityInstance(graphAsset);
            instance.SetOwner(this);
            owner.AppendAbility(instance);
            return instance;
        }

        public bool RemoveAbility(Predicate<AbilityInstance> match)
        {
            return owner.RemoveAbility(match);
        }

        internal void AppendAbility(AbilityInstance ability)
        {
            owner.AppendAbility(ability);
        }

        internal void RemoveAbility(AbilityInstance ability)
        {
            owner.RemoveAbility(ability);
        }

        internal void RemoveAbility(int abilityId)
        {
            owner.RemoveAbility(abilityId);
        }

        internal void ClearAllAbilities()
        {
            owner.ClearAllAbilities();
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
