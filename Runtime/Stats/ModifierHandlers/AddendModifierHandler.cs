using System.Collections.Generic;

namespace Physalia.AbilitySystem.Stat
{
    public class AddendModifierHandler : IModifierHandler
    {
        private readonly Dictionary<int, int> sumsCache = new();

        public void RefreshStats(StatOwner owner)
        {
            for (var i = 0; i < owner.Modifiers.Count; i++)
            {
                Modifier modifier = owner.Modifiers[i];
                if (!sumsCache.ContainsKey(modifier.StatId))
                {
                    sumsCache.Add(modifier.StatId, modifier.Addend);
                }
                else
                {
                    sumsCache[modifier.StatId] += modifier.Addend;
                }
            }

            foreach (KeyValuePair<int, int> pair in sumsCache)
            {
                int statId = pair.Key;
                if (owner.Stats.TryGetValue(statId, out Stat stat))
                {
                    int sum = pair.Value;
                    stat.CurrentValue += sum;
                }
            }
        }
    }
}
