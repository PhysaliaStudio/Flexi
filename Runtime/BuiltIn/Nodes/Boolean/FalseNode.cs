namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Logical")]
    public class FalseNode : ValueNode
    {
        public Outport<bool> value;

        protected override void EvaluateSelf()
        {
            value.SetValue(false);
        }
    }
}
