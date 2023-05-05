namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class UnitNode : ValueNode
    {
        public Outport<Unit> unitPort;

        protected override void EvaluateSelf()
        {
            unitPort.SetValue(Actor as Unit);
        }
    }
}
