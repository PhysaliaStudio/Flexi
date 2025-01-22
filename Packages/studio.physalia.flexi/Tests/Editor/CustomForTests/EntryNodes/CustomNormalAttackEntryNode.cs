namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomNormalAttackEntryNode : DefaultEntryNode<CustomNormalAttackEntryNode.Context>
    {
        public class Context : IEventContext
        {
            public CustomUnit attacker;
            public CustomUnit mainTarget;
        }

        public Outport<CustomUnit> attackerPort;
        public Outport<CustomUnit> targetPort;

        public override bool CanExecute(Context context)
        {
            if (context.attacker != null && context.mainTarget != null)
            {
                return true;
            }

            return false;
        }

        protected override FlowState OnExecute(Context context)
        {
            attackerPort.SetValue(context.attacker);
            targetPort.SetValue(context.mainTarget);
            return FlowState.Success;
        }
    }
}
