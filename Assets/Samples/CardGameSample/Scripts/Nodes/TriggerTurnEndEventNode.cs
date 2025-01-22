namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class TriggerTurnEndEventNode : DefaultProcessNode
    {
        public Inport<Game> gamePort;

        protected override AbilityState OnExecute()
        {
            EnqueueEvent(new TurnEndContext { game = gamePort.GetValue() });
            return AbilityState.RUNNING;
        }
    }
}
