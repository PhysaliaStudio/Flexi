namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomActivationNode : DefaultEntryNode<CustomActivationNode.Context>
    {
        public class Context : IEventContext
        {
            public CustomUnit activator;
        }

        public Outport<CustomUnit> activatorPort;

        public override bool CanExecute(Context context)
        {
            if (context.activator != null)
            {
                return true;
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var context = GetPayload<Context>();
            activatorPort.SetValue(context.activator);
            return AbilityState.RUNNING;
        }
    }
}
