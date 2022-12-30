namespace Physalia.Flexi
{
    [NodeCategory("Built-in/Value")]
    public class StringNode : ValueNode
    {
        public Outport<string> output;
        public Variable<string> text;

        protected override void EvaluateSelf()
        {
            output.SetValue(text.Value);
        }
    }
}
