namespace Physalia.AbilityFramework
{
    internal sealed class DefaultAbilityRunner : AbilityRunner
    {
        public override AbilityState Run(AbilitySystem abilitySystem, AbilityEventQueue eventQueue)
        {
            Reset();
            while (Next())
            {
                AbilityInstance instance = Current;
                instance.Execute();

                AbilityState state = instance.CurrentState;
                if (state != AbilityState.DONE)
                {
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
