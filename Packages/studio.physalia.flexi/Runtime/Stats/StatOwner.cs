using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private readonly List<StatModifier> modifiers = new();

        private bool isValid = true;

        public int Id => id;

        internal IReadOnlyDictionary<int, Stat> Stats => stats;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddStat<TEnum>(TEnum statId, int baseValue) where TEnum : Enum
        {
            AddStat(CastTo<int>.From(statId), baseValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveStat<TEnum>(TEnum statId) where TEnum : Enum
        {
            RemoveStat(CastTo<int>.From(statId));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Stat GetStat<TEnum>(TEnum statId) where TEnum : Enum
        {
            return GetStat(CastTo<int>.From(statId));
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
