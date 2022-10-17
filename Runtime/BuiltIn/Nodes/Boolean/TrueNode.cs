namespace Physalia.AbilitySystem
{
    public class TrueNode : ValueNode
    {
        public Outport<bool> value;

        public override void Evaluate()
        {
            value.SetValue(true);
        }
    }
}
