using System;
using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    [Serializable]
    public class AbilityContext
    {
        public enum Type
        {
            ACTION,
            MODIFIER,
        }

        public Type ContextType;
        public List<AbilityEffect> Effects = new();
    }
}
