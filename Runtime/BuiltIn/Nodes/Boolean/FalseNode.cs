namespace Physalia.AbilitySystem
{
    public class FalseNode : ValueNode
    {
        public Outport<bool> value;

        public override void Evaluate()
        {
            value.SetValue(false);
        }
    }
}
