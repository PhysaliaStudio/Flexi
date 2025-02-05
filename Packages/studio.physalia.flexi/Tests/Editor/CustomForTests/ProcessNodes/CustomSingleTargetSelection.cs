namespace Physalia.Flexi.Tests
{
    public class CustomSingleTargetAnswerContext : IResumeContext
    {
        public CustomUnit target;
    }

    [NodeCategoryForTests]
    public class CustomSingleTargetSelection : DefaultProcessNode<IResumeContext>
    {
        public Outport<CustomUnit> targetPort;

        protected override FlowState OnExecute()
        {
            Container.CoreWrapper.TriggerChoice();
            return FlowState.Pause;
        }

        protected override bool CanResume(IResumeContext resumeContext)
        {
            if (resumeContext is CustomCancellation)
            {
                return true;
            }

            if (resumeContext is CustomSingleTargetAnswerContext answerContext)
            {
                if (answerContext.target != null)
                {
                    return true;
                }
            }

            return false;
        }

        protected override FlowState OnResume(IResumeContext resumeContext)
        {
            if (resumeContext is CustomCancellation)
            {
                return FlowState.Abort;
            }

            var answerContext = resumeContext as CustomSingleTargetAnswerContext;
            targetPort.SetValue(answerContext.target);
            return FlowState.Success;
        }
    }
}
