using System;

namespace Physalia.AbilityFramework
{
    [Serializable]
    public class StatDefinition
    {
        public int Id;
        public string Name;

        public override string ToString()
        {
            return $"<{Id}:{Name}>";
        }
    }
}
