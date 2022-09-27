using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public class AddendModifierHandler : IModifierHandler
    {
        private readonly Dictionary<int, int> sumsCache = new();

        public void RefreshStats(StatOwner owner)
        {
            for (var i = 0; i < owner.AbilityContexts.Count; i++)
            {
                AbilityContextInstance context = owner.AbilityContexts[i];
                if (context.ContextType != AbilityContext.Type.MODIFIER)
                {
                    continue;
                }

                for (var j = 0; j < context.Effects.Count; j++)
                {
                    AbilityEffect effect = context.Effects[j];
                    if (effect.Op == AbilityEffect.Operator.ADD)
                    {
                        if (!sumsCache.ContainsKey(effect.StatId))
                        {
                            sumsCache.Add(effect.StatId, effect.Value);
                        }
                        else
                        {
                            sumsCache[effect.StatId] += effect.Value;
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
                    stat.CurrentValue += sum;
                }
            }
        }
    }
}
