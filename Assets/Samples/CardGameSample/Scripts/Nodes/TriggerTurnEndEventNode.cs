namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class TriggerTurnEndEventNode : ProcessNode
    {
        public Inport<Game> gamePort;

        protected override AbilityState DoLogic()
        {
            EnqueueEvent(new TurnEndContext { game = gamePort.GetValue() });
            return AbilityState.RUNNING;
        }
    }
}
