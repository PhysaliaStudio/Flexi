namespace Physalia.AbilityFramework.Tests
{
    public class CustomSingleTargetChoiseContext : IChoiceContext
    {
        public CustomUnit target;
    }

    public class CustomSingleTargetAnswerContext : INodeContext
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

        public override bool CheckNodeContext(INodeContext nodeContext)
        {
            if (nodeContext is CancellationContext)
            {
                return true;
            }

            if (nodeContext is CustomSingleTargetAnswerContext answerContext)
            {
                if (answerContext.target != null)
                {
                    return true;
                }
            }

            return false;
        }

        protected override AbilityState ResumeLogic(INodeContext nodeContext)
        {
            if (nodeContext is CancellationContext)
            {
                return AbilityState.ABORT;
            }

            var answerContext = nodeContext as CustomSingleTargetAnswerContext;
            targetPort.SetValue(answerContext.target);
            return AbilityState.RUNNING;
        }
    }
}
