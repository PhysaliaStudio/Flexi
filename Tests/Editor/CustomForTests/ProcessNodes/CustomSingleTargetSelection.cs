namespace Physalia.Flexi.Tests
{
    public class CustomSingleTargetChoiseContext : IChoiceContext
    {
        public CustomUnit target;
    }

    public class CustomSingleTargetAnswerContext : IResumeContext
    {
        public CustomUnit target;
    }

    [NodeCategory("Built-in/[Test Custom]")]
    public class CustomSingleTargetSelection : ProcessNode
    {
        public Outport<CustomUnit> targetPort;

        protected override AbilityState DoLogic()
        {
            return WaitAndChoice(new CustomSingleTargetChoiseContext());
        }

        public override bool CheckNodeContext(IResumeContext resumeContext)
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
