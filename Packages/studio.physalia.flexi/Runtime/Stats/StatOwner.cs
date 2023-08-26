using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// StatOwner is a container of stats and abilities.
    /// </summary>
    public class StatOwner
    {
        private readonly int id;
        private readonly StatOwnerRepository repository;

        private readonly Dictionary<int, Stat> stats = new();
        private readonly List<Ability> abilities = new();
        private readonly List<AbilityFlow> abilityFlows = new();
        private readonly List<StatModifier> modifiers = new();

        private bool isValid = true;

        public int Id => id;

        internal IReadOnlyDictionary<int, Stat> Stats => stats;
        public IReadOnlyList<Ability> Abilities => abilities;
        public IReadOnlyList<AbilityFlow> AbilityFlows => abilityFlows;
        internal IReadOnlyList<StatModifier> Modifiers => modifiers;

        internal StatOwner(int id, StatOwnerRepository repository)
        {
            this.id = id;
            this.repository = repository;
        }

        public bool IsValid()
        {
            return isValid;
        }

        public void AddStat(int statId, int baseValue)
        {
            if (stats.ContainsKey(statId))
            {
                Logger.Error($"Create stat failed! Owner {id} already has stat {statId}");
                return;
            }

            var stat = new Stat(statId, baseValue);
            stats.Add(statId, stat);
        }

        public void RemoveStat(int statId)
        {
            stats.Remove(statId);
        }

        public Stat GetStat(int statId)
        {
            if (!stats.TryGetValue(statId, out Stat stat))
            {
                return null;
            }

            return stat;
        }

        internal Ability FindAbility(AbilityDataSource abilityDataSource)
        {
            return abilities.Find(x => x.DataSource == abilityDataSource);
        }

        internal void AppendAbility(Ability ability)
        {
            abilities.Add(ability);
        }

        internal void RemoveAbility(Ability ability)
        {
            abilities.Remove(ability);
        }

        public AbilityFlow FindAbilityFlow(Predicate<AbilityFlow> match)
        {
            return abilityFlows.Find(match);
        }

        internal void AppendAbilityFlow(AbilityFlow abilityFlow)
        {
            abilityFlows.Add(abilityFlow);
        }

        internal void RemoveAbilityFlow(AbilityFlow abilityFlow)
        {
            abilityFlows.Remove(abilityFlow);
        }

        internal bool RemoveAbilityFlow(Predicate<AbilityFlow> match)
        {
            AbilityFlow abilityFlow = abilityFlows.Find(match);
            if (abilityFlow == null)
            {
                return false;
            }

            abilityFlows.Remove(abilityFlow);
            return true;
        }

        internal void RemoveAbilityFlowAt(int index)
        {
            abilityFlows.RemoveAt(index);
        }

        internal void ClearAllAbilityFlows()
        {
            abilityFlows.Clear();
        }

        public void AppendModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier);
        }

        public void ClearAllModifiers()
        {
            modifiers.Clear();
        }

        internal void ResetAllStats()
        {
            foreach (Stat stat in stats.Values)
            {
                stat.CurrentValue = stat.CurrentBase;
            }
        }

        public void Destroy()
        {
            isValid = false;
            repository.RemoveOwner(this);
        }
    }
}
