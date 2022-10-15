using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public class MultiplierModifierHandler : IModifierHandler
    {
        private readonly Dictionary<int, int> sumsCache = new();

        public void RefreshStats(StatOwner owner)
        {
            for (var i = 0; i < owner.Modifiers.Count; i++)
            {
                StatModifierInstance modifierInstance = owner.Modifiers[i];
                for (var j = 0; j < modifierInstance.Items.Count; j++)
                {
                    StatModifierItem modifierItem = modifierInstance.Items[j];
                    if (modifierItem.op == StatModifierItem.Operator.MUL)
                    {
                        if (sumsCache.ContainsKey(modifierItem.statId))
                        {
                            sumsCache[modifierItem.statId] += modifierItem.value;
                        }
                        else
                        {
                            sumsCache.Add(modifierItem.statId, modifierItem.value);
                        }
                    }
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
