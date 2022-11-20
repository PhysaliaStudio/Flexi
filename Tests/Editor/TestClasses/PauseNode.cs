namespace Physalia.AbilityFramework.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class PauseNode : ProcessNode
    {
        protected override AbilityState DoLogic()
        {
            return AbilityState.PAUSE;
        }

        public override bool CheckNodeContext(INodeContext nodeContext)
        {
            return true;
        }

        protected override AbilityState ResumeLogic(INodeContext nodeContext)
        {
            return AbilityState.RUNNING;
        }
    }
}
