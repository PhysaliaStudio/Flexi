using System.Collections.Generic;

namespace Physalia.Stats
{
    public class MultiplierModifierHandler : IStatModifierHandler
    {
        private readonly Dictionary<int, int> sumsCache = new();

        public void RefreshStats(StatOwner owner)
        {
            for (var i = 0; i < owner.Modifiers.Count; i++)
            {
                StatModifier modifier = owner.Modifiers[i];
                if (!sumsCache.ContainsKey(modifier.StatId))
                {
                    sumsCache.Add(modifier.StatId, modifier.Multiplier);
                }
                else
                {
                    sumsCache[modifier.StatId] += modifier.Multiplier;
                }
            }

            foreach (KeyValuePair<int, int> pair in sumsCache)
            {
                int statId = pair.Key;
                if (owner.Stats.TryGetValue(statId, out Stat stat))
                {
                    int sum = pair.Value;
                    stat.CurrentValue = stat.CurrentValue * (100 + sum) / 100;
                }
            }
        }
    }
}
