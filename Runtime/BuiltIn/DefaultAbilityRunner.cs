namespace Physalia.AbilityFramework
{
    internal sealed class DefaultAbilityRunner : AbilityRunner
    {
        protected override AbilityState IterateAbilities(AbilitySystem abilitySystem)
        {
            while (Next())
            {
                AbilityInstance instance = Current;
                instance.Execute();

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
            }

            Clear();
            return AbilityState.DONE;
        }
    }
}
