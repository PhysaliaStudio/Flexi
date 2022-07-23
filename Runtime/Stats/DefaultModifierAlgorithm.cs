namespace Physalia.AbilitySystem.StatSystem
{
    public class DefaultModifierAlgorithm : IModifierAlgorithm
    {
        public void RefreshStats(StatOwner owner)
        {
            new AddendModifierHandler().RefreshStats(owner);
            new MultiplierModifierHandler().RefreshStats(owner);
        }
    }
}
