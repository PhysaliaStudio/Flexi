namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class PauseNode : DefaultProcessNode<EmptyResumeContext>
    {
        protected override FlowState OnExecute()
        {
            return FlowState.Pause;
        }

        protected override bool CanResume(EmptyResumeContext context)
        {
            return true;
        }

        protected override FlowState OnResume(EmptyResumeContext context)
        {
            return FlowState.Success;
        }
    }
}
