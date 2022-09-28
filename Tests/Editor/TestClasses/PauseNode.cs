namespace Physalia.AbilitySystem.Tests
{
    public class PauseNode : ProcessNode
    {
        protected override AbilityState DoLogic()
        {
            return AbilityState.PAUSE;
        }
    }
}
