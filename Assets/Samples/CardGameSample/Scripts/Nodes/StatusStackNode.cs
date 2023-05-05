namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class StatusStackNode : ValueNode
    {
        public Inport<Game> gamePort;
        public Inport<Unit> unitPort;
        public Outport<int> stackPort;
        public Variable<int> statusId;

        protected override void EvaluateSelf()
        {
            Game game = gamePort.GetValue();
            Unit unit = unitPort.GetValue();
            int stack = game.GetUnitStatusStack(unit, statusId.Value);
            stackPort.SetValue(stack);
        }
    }
}
