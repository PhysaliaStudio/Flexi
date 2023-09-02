using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// Actor is a wrapper class of <see cref="StatOwner"/>, which is used for inheritance.
    /// </summary>
    public abstract class Actor
    {
        private readonly AbilitySystem abilitySystem;
        private readonly StatOwner owner;

        public int OwnerId => owner.Id;
        internal StatOwner Owner => owner;
        internal IReadOnlyDictionary<int, Stat> Stats => owner.Stats;
        public IReadOnlyList<AbilityDataSource> AbilityDataSources => owner.AbilityDataSources;
        public IReadOnlyList<StatModifier> Modifiers => owner.Modifiers;

        public Actor(AbilitySystem abilitySystem)
        {
            this.abilitySystem = abilitySystem;
            owner = abilitySystem.CreateOwner(this);
        }

        public bool IsValid()
        {
            return owner.IsValid();
        }

        public void AddStat<TEnum>(TEnum statId, int baseValue) where TEnum : Enum
        {
            owner.AddStat(statId, baseValue);
        }

        public void RemoveStat<TEnum>(TEnum statId) where TEnum : Enum
        {
            owner.RemoveStat(statId);
        }

        public Stat GetStat<TEnum>(TEnum statId) where TEnum : Enum
        {
            return owner.GetStat(statId);
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

        public void AppendAbilityDataSource(AbilityDataSource abilityDataSource, object userData = null)
        {
            owner.AppendAbilityDataSource(abilityDataSource);
        }

        public bool RemoveAbilityDataSource(AbilityDataSource abilityDataSource)
        {
            return owner.RemoveAbilityDataSource(abilityDataSource);
        }

        public void AppendModifier(StatModifier modifier)
        {
            owner.AppendModifier(modifier);
        }

        public void AppendModifiers(IReadOnlyList<StatModifier> modifiers)
        {
            for (var i = 0; i < modifiers.Count; i++)
            {
                owner.AppendModifier(modifiers[i]);
            }
        }

        public void RemoveModifier(StatModifier modifier)
        {
            owner.RemoveModifier(modifier);
        }

        public void ClearAllModifiers()
        {
            owner.ClearAllModifiers();
        }

        /// <summary>
        /// This method just total all modifiers by algorithm, so there is no priority issue.
        /// </summary>
        public void RefreshStats()
        {
            abilitySystem.RefreshStats(this);
        }

        internal void ResetAllStats()
        {
            owner.ResetAllStats();
        }

        public void Destroy()
        {
            abilitySystem.DestroyOwner(this);
        }
    }
}
