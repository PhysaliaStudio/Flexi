namespace Physalia.AbilitySystem
{
    internal sealed class DefaultAbilityRunner : AbilityRunner
    {
        public override AbilityState Run()
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
            }

            Clear();
            return AbilityState.DONE;
        }
    }
}
