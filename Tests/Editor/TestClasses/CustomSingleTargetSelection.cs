namespace Physalia.AbilityFramework.Tests
{
    public class CustomSingleTargetChoiseContext : ChoiceContext
    {
        public CustomUnit target;
    }

    public class CustomSingleTargetAnswerContext : NodeContext
    {
        public CustomUnit target;
    }

    [NodeCategory("Built-in/[Test Custom]")]
    public class CustomSingleTargetSelection : ProcessNode
    {
        public Outport<CustomUnit> targetPort;

        protected override AbilityState DoLogic()
        {
            Instance.System.TriggerChoice(new CustomSingleTargetChoiseContext());
            return AbilityState.PAUSE;
        }

        public override bool CheckNodeContext(NodeContext nodeContext)
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

        protected override AbilityState ResumeLogic(NodeContext nodeContext)
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
