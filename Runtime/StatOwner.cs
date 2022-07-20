using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Stats
{
    public class StatOwner
    {
        private class StatModifierSum
        {
            public int TotalAddend;
            public int TotalMultiplier;
        }

        private readonly int id;
        private readonly StatDefinitionTable table;
        private readonly StatOwnerRepository repository;

        private readonly Dictionary<int, Stat> stats = new();
        private readonly List<StatModifier> modifiers = new();
        private readonly Dictionary<int, StatModifierSum> modifierSumsCache = new();

        private bool isValid = true;

        public int Id => id;
        public int CountOfModifier => modifiers.Count;

        internal StatOwner(int id, StatDefinitionTable table, StatOwnerRepository repository)
        {
            this.id = id;
            this.table = table;
            this.repository = repository;
        }

        public bool IsValid()
        {
            return isValid;
        }

        public void AddStat(int statId, int originalValue)
        {
            StatDefinition definition = table.GetStatDefinition(statId);
            if (definition == null)
            {
                Debug.LogError($"Create stat failed! See upon messages for details");
                return;
            }

            if (stats.ContainsKey(statId))
            {
                Debug.LogError($"Create stat failed! Owner {id} already has stat {definition}");
                return;
            }

            var stat = new Stat(definition, originalValue);
            stats.Add(definition.Id, stat);
            modifierSumsCache.Add(definition.Id, new StatModifierSum());
        }

        public void RemoveStat(int statId)
        {
            stats.Remove(statId);
            modifierSumsCache.Remove(statId);
        }

        public Stat GetStat(int statId)
        {
            if (!stats.TryGetValue(statId, out Stat stat))
            {
                return null;
            }

            return stat;
        }

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void ClearAllModifiers()
        {
            modifiers.Clear();
        }

        public void RefreshStats()
        {
            ResetAllStats();
            for (var i = 0; i < modifiers.Count; i++)
            {
                var statId = modifiers[i].StatId;
                if (!stats.ContainsKey(statId))
                {
                    continue;
                }

                var sum = modifierSumsCache[statId];
                sum.TotalAddend += modifiers[i].Addend;
                sum.TotalMultiplier += modifiers[i].Multiplier;
            }

            foreach (Stat stat in stats.Values)
            {
                var sum = modifierSumsCache[stat.Id];
                if (sum.TotalMultiplier <= -100)
                {
                    stat.CurrentValue = 0;
                }
                else
                {
                    stat.CurrentValue = (int)((stat.CurrentValue + sum.TotalAddend) * (100 + sum.TotalMultiplier) / 100f);
                }
            }

            ResetAllModifierSums();
        }

        private void ResetAllStats()
        {
            foreach (Stat stat in stats.Values)
            {
                stat.CurrentValue = stat.OriginalValue;
            }
        }

        private void ResetAllModifierSums()
        {
            foreach (StatModifierSum sum in modifierSumsCache.Values)
            {
                sum.TotalAddend = 0;
                sum.TotalMultiplier = 0;
            }
        }

        public void Destroy()
        {
            isValid = false;
            repository.RemoveOwner(this);
        }
    }
}
