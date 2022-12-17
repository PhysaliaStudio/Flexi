namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Common")]
    public class StackNode : ValueNode
    {
        public Outport<int> value;

        protected override void EvaluateSelf()
        {
            value.SetValue(Instance.Stack);
        }
    }
}
