namespace Physalia.Flexi.Samples.CardGame
{
    public class SystemProcessContext : IEventContext
    {
        public Game game;
    }

    [NodeCategory("Card Game Sample")]
    public class SystemProcessEntryNode : EntryNode<SystemProcessContext>
    {
        public Outport<Game> gamePort;
        public Outport<Player> playerPort;

        public override bool CanExecute(SystemProcessContext context)
        {
            return true;
        }

        protected override AbilityState DoLogic()
        {
            var context = GetPayload<SystemProcessContext>();
            gamePort.SetValue(context.game);
            playerPort.SetValue(context.game.Player);
            return AbilityState.RUNNING;
        }
    }
}
