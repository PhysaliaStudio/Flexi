namespace Physalia.AbilitySystem
{
    public class NotNode : ValueNode
    {
        public Inport<bool> a;
        public Outport<bool> result;

        protected override void EvaluateSelf()
        {
            result.SetValue(!a.GetValue());
        }
    }
}
