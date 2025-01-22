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
    public class SingleTargetNode : DefaultProcessNode
    {
        public Outport<Unit> target;
        public Variable<UnitType> unitType;

        protected override AbilityState OnExecute()
        {
            Container.Game.StartSingleTargetChoice(Container.Card, unitType.Value);
            return AbilityState.PAUSE;
        }

        public override bool CanResume(IResumeContext resumeContext)
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

        protected override AbilityState OnResume(IResumeContext resumeContext)
        {
            if (resumeContext is CancellationContext)
            {
                return AbilityState.ABORT;
            }

            var answerContext = resumeContext as SingleTargetAnswerContext;
            target.SetValue(answerContext.unit);
            return AbilityState.RUNNING;
        }
    }
}
