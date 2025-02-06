namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class TriggerTurnEndEventNode : DefaultProcessNode
    {
        protected override FlowState OnExecute()
        {
            EnqueueEvent(new TurnEndContext());
            return FlowState.Success;
        }
    }
}
