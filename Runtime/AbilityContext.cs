using System;
using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    [Serializable]
    public class AbilityContext
    {
        public List<AbilityEffect> Effects = new();
    }
}
