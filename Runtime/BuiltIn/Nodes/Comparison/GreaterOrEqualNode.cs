namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Comparison")]
    public class GreaterOrEqualNode : ValueNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<bool> result;

        protected override void EvaluateSelf()
        {
            result.SetValue(a.GetValue() >= b.GetValue());
        }
    }
}
