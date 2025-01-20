namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomDamageEventNode : DefaultEntryNode<CustomDamageEventNode.Context>
    {
        public class Context : IEventContext
        {
            public CustomUnit instigator;
            public CustomUnit target;
        }

        public Outport<CustomUnit> instigatorPort;
        public Outport<CustomUnit> targetPort;

        public override bool CanExecute(Context context)
        {
            if (context.target == Container.Actor)
            {
                return true;
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var context = GetPayload<Context>();
            instigatorPort.SetValue(context.instigator);
            targetPort.SetValue(context.target);
            return AbilityState.RUNNING;
        }
    }
}
