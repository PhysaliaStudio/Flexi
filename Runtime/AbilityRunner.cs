using System.Collections.Generic;

namespace Physalia.AbilityFramework
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

        public AbilityState Run(AbilitySystem abilitySystem)
        {
            Reset();
            return IterateAbilities(abilitySystem);
        }

        protected abstract AbilityState IterateAbilities(AbilitySystem abilitySystem);

        public AbilityState ResumeWithContext(AbilitySystem abilitySystem, NodeContext context)
        {
            AbilityInstance instance = Current;
            Current.Resume(context);

            AbilityState state = instance.CurrentState;
            if (state != AbilityState.DONE)
            {
                if (state == AbilityState.ABORT)
                {
                    Clear();
                }
                return state;
            }

            abilitySystem.RefreshStatsAndModifiers();
            abilitySystem.TriggerNextEvent();

            return IterateAbilities(abilitySystem);
        }
    }
}
