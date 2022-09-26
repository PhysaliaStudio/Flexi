namespace Physalia.AbilitySystem
{
    public class StringNode : ValueNode
    {
        public Outport<string> output;
        public Variable<string> text;

        public override void Evaluate()
        {
            output.SetValue(text.Value);
        }
    }
}
