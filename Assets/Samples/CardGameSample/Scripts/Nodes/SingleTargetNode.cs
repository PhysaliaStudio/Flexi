namespace Physalia.Flexi.Samples.CardGame
{
    public class SingleTargetChoiceContext : IChoiceContext
    {
        public Actor actor;
        public UnitType unitType;
    }

    public class SingleTargetAnswerContext : IResumeContext
    {
        public Unit unit;
    }

    public class CancellationContext : IResumeContext
    {

    }

    [NodeCategory("Card Game Sample")]
    public class SingleTargetNode : ProcessNode
    {
        public Outport<Unit> target;
        public Variable<UnitType> unitType;

        protected override AbilityState DoLogic()
        {
            var context = new SingleTargetChoiceContext
            {
                actor = Actor,
                unitType = unitType.Value,
            };

            return WaitAndChoice(context);
        }

        public override bool CheckNodeContext(IResumeContext resumeContext)
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

        protected override AbilityState ResumeLogic(IResumeContext resumeContext)
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
