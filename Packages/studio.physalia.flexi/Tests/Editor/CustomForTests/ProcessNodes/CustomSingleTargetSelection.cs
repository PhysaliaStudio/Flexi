namespace Physalia.Flexi.Tests
{
    public class CustomSingleTargetAnswerContext : IResumeContext
    {
        public CustomUnit target;
    }

    [NodeCategoryForTests]
    public class CustomSingleTargetSelection : DefaultProcessNode
    {
        public Outport<CustomUnit> targetPort;

        protected override AbilityState OnExecute()
        {
            Container.CoreWrapper.TriggerChoice();
            return AbilityState.PAUSE;
        }

        public override bool CanResume(IResumeContext resumeContext)
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

        protected override AbilityState ResumeLogic(IResumeContext resumeContext)
        {
            if (resumeContext is CustomCancellation)
            {
                return AbilityState.ABORT;
            }

            var answerContext = resumeContext as CustomSingleTargetAnswerContext;
            targetPort.SetValue(answerContext.target);
            return AbilityState.RUNNING;
        }
    }
}
