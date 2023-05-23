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
        public IReadOnlyList<Ability> Abilities => owner.Abilities;
        public IReadOnlyList<AbilityFlow> AbilityFlows => owner.AbilityFlows;

        public Actor(AbilitySystem abilitySystem)
        {
            this.abilitySystem = abilitySystem;
            owner = abilitySystem.CreateOwner(this);
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
            Stat stat = owner.GetStat(statId);
            if (stat != null)
            {
                stat.CurrentBase = newBase;
                RefreshStats();
            }
        }

        public void ModifyStat(int statId, int value)
        {
            Stat stat = owner.GetStat(statId);
            if (stat != null)
            {
                stat.CurrentBase += value;
                RefreshStats();
            }
        }

        public Ability FindAbility(AbilityData abilityData)
        {
            return owner.FindAbility(abilityData);
        }

#if UNITY_5_3_OR_NEWER
        public Ability AppendAbility(AbilityAsset abilityAsset, object userData = null)
        {
            return AppendAbility(abilityAsset.Data, userData);
        }
#endif

        public Ability AppendAbility(AbilityData abilityData, object userData = null)
        {
            Ability ability = abilitySystem.GetAbility(abilityData, userData);
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

#if UNITY_5_3_OR_NEWER
        public bool RemoveAbility(AbilityAsset abilityAsset)
        {
            return RemoveAbility(abilityAsset.Data);
        }
#endif

        public bool RemoveAbility(AbilityData abilityData)
        {
            Ability ability = owner.FindAbility(abilityData);
            if (ability == null)
            {
                return false;
            }

            RemoveAbility(ability);
            return true;
        }

        public void RemoveAbility(Ability ability)
        {
            owner.RemoveAbility(ability);

            IReadOnlyList<AbilityFlow> abilityFlows = Owner.AbilityFlows;
            for (var i = abilityFlows.Count - 1; i >= 0; i--)
            {
                AbilityFlow abilityFlow = abilityFlows[i];
                if (abilityFlow.Ability == ability)
                {
                    owner.RemoveAbilityFlowAt(i);
                }
            }

            abilitySystem.ReleaseAbility(ability);
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
