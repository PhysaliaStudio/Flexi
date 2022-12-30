namespace Physalia.Flexi
{
    [NodeCategory("Built-in/Common")]
    public class BlackboardNode : ValueNode
    {
        public Outport<int> value;
        public Variable<string> key;

        protected override void EvaluateSelf()
        {
            value.SetValue(Ability.GetVariable(key.Value));
        }
    }
}
