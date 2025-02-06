namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class OnTurnEndNode : DefaultEntryNode<TurnEndContext>
    {
        protected override bool CanExecute(TurnEndContext context)
        {
            return true;
        }

        protected override FlowState OnExecute(TurnEndContext context)
        {
            return FlowState.Success;
        }
    }
}
