namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class OnTurnEndNode : DefaultEntryNode<TurnEndContext>
    {
        public Outport<Game> gamePort;

        public override bool CanExecute(TurnEndContext context)
        {
            return true;
        }

        protected override AbilityState OnExecute(TurnEndContext context)
        {
            gamePort.SetValue(context.game);
            return AbilityState.RUNNING;
        }
    }
}
