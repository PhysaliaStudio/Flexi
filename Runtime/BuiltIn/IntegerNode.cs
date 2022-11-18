namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Value")]
    public class IntegerNode : ValueNode
    {
        public Outport<int> output;
        public Variable<int> value;

        protected override void EvaluateSelf()
        {
            output.SetValue(value.Value);
        }
    }
}
