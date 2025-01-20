namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class UnitNode : DefaultValueNode
    {
        public Outport<Unit> unitPort;

        protected override void EvaluateSelf()
        {
            unitPort.SetValue(Container.Unit);
        }
    }
}
