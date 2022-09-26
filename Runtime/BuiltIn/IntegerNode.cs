namespace Physalia.AbilitySystem
{
    public class IntegerNode : ValueNode
    {
        public Outport<int> output;
        public Variable<int> value;

        public override void Evaluate()
        {
            output.SetValue(value.Value);
        }
    }
}
