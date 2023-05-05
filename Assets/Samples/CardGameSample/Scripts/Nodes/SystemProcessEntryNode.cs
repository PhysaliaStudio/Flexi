namespace Physalia.Flexi.Samples.CardGame
{
    public class SystemProcessPayload : IEventContext
    {
        public Game game;
    }

    [NodeCategory("Card Game Sample")]
    public class SystemProcessEntryNode : EntryNode
    {
        public Outport<Game> gamePort;
        public Outport<Player> playerPort;

        public override bool CanExecute(IEventContext payload)
        {
            if (payload is SystemProcessPayload)
            {
                return true;
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var payload = GetPayload<SystemProcessPayload>();
            gamePort.SetValue(payload.game);
            playerPort.SetValue(payload.game.Player);
            return AbilityState.RUNNING;
        }
    }
}
