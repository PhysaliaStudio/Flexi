using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public abstract class AbilityRunner
    {
        private readonly List<AbilityInstance> instances = new();
        private int currentIndex = -1;

        public AbilityInstance Current
        {
            get
            {
                if (currentIndex < 0 || currentIndex >= instances.Count)
                {
                    return null;
                }

                return instances[currentIndex];
            }
        }

        public bool Next()
        {
            currentIndex++;
            if (currentIndex < 0 || currentIndex >= instances.Count)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public void Add(AbilityInstance instance)
        {
            instances.Add(instance);
        }

        public void Clear()
        {
            instances.Clear();
        }

        public abstract AbilityState Run();
    }
}
