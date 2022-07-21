using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Stats
{
    public class StatOwner
    {
        private readonly int id;
        private readonly StatDefinitionTable table;
        private readonly StatOwnerRepository repository;
        private readonly IModifierAlgorithm modifierAlgorithm;

        private readonly Dictionary<int, Stat> stats = new();
        private readonly List<StatModifier> modifiers = new();

        private bool isValid = true;

        public int Id => id;
        public int CountOfModifier => modifiers.Count;

        internal IReadOnlyDictionary<int, Stat> Stats => stats;
        internal IReadOnlyList<StatModifier> Modifiers => modifiers;

        internal StatOwner(int id, StatDefinitionTable table, StatOwnerRepository repository, IModifierAlgorithm modifierAlgorithm)
        {
            this.id = id;
            this.table = table;
            this.repository = repository;
            this.modifierAlgorithm = modifierAlgorithm;
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
            modifierAlgorithm.RefreshStats(this);
        }

        private void ResetAllStats()
        {
            foreach (Stat stat in stats.Values)
            {
                stat.CurrentValue = stat.OriginalValue;
            }
        }

        public void Destroy()
        {
            isValid = false;
            repository.RemoveOwner(this);
        }
    }
}
