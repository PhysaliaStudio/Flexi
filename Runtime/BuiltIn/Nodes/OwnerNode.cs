namespace Physalia.AbilitySystem
{
    public class OwnerNode : ValueNode
    {
        public Outport<StatOwner> owner;

        protected override void EvaluateSelf()
        {
            if (Instance != null)
            {
                owner.SetValue(Instance.Owner);
            }
            else
            {
                owner.SetValue(null);
            }
        }
    }
}
