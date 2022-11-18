namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Common")]
    public class BlackboardNode : ValueNode
    {
        public Outport<int> value;
        public Variable<string> key;

        protected override void EvaluateSelf()
        {
            value.SetValue(Instance.GetBlackboardVariable(key.Value));
        }
    }
}
