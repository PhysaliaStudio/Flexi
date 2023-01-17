namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Value)]
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
