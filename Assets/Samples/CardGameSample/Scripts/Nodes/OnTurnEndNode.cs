namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class OnTurnEndNode : EntryNode
    {
        public Outport<Game> gamePort;

        public override bool CanExecute(IEventContext context)
        {
            if (context is TurnEndEvent)
            {
                return true;
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var context = GetPayload<TurnEndEvent>();
            gamePort.SetValue(context.game);
            return AbilityState.RUNNING;
        }
    }
}
