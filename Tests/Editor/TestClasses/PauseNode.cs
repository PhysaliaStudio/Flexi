namespace Physalia.AbilityFramework.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class PauseNode : ProcessNode
    {
        protected override AbilityState DoLogic()
        {
            return AbilityState.PAUSE;
        }

        public override bool CheckNodeContext(NodeContext nodeContext)
        {
            return true;
        }

        protected override AbilityState ResumeLogic(NodeContext nodeContext)
        {
            return AbilityState.RUNNING;
        }
    }
}
