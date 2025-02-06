namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class StatusStackNode : DefaultValueNode
    {
        public Inport<Unit> unitPort;
        public Outport<int> stackPort;
        public Variable<int> statusId;

        protected override void EvaluateSelf()
        {
            Unit unit = unitPort.GetValue();
            int stack = Container.Game.GetUnitStatusStack(unit, statusId.Value);
            stackPort.SetValue(stack);
        }
    }
}
