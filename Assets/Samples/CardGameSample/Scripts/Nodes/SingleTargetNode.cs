namespace Physalia.Flexi.Samples.CardGame
{
    public class SingleTargetAnswerContext : IResumeContext
    {
        public Unit unit;
    }

    public class CancellationContext : IResumeContext
    {

    }

    [NodeCategory("Card Game Sample")]
    public class SingleTargetNode : DefaultProcessNode<IResumeContext>
    {
        public Outport<Unit> target;
        public Variable<UnitType> unitType;

        protected override FlowState OnExecute()
        {
            Container.Game.StartSingleTargetChoice(Container.Card, unitType.Value);
            return FlowState.Pause;
        }

        protected override bool CanResume(IResumeContext resumeContext)
        {
            if (resumeContext is CancellationContext)
            {
                return true;
            }

            if (resumeContext is SingleTargetAnswerContext answerContext)
            {
                return answerContext.unit != null;
            }

            return false;
        }

        protected override FlowState OnResume(IResumeContext resumeContext)
        {
            if (resumeContext is CancellationContext)
            {
                return FlowState.Abort;
            }

            var answerContext = resumeContext as SingleTargetAnswerContext;
            target.SetValue(answerContext.unit);
            return FlowState.Success;
        }
    }
}
