using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class AddendModifierHandler : IModifierHandler
    {
        private readonly Dictionary<int, int> sumsCache = new();

        public void RefreshStats(StatOwner owner)
        {
            for (var i = 0; i < owner.Modifiers.Count; i++)
            {
                StatModifier modifier = owner.Modifiers[i];
                if (modifier.op == StatModifier.Operator.ADD)
                {
                    if (sumsCache.ContainsKey(modifier.statId))
                    {
                        sumsCache[modifier.statId] += modifier.value;
                    }
                    else
                    {
                        sumsCache.Add(modifier.statId, modifier.value);
                    }
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

            sumsCache.Clear();
        }
    }
}
