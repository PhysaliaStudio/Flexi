namespace Physalia.AbilityFramework
{
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
