namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomActivationNode : EntryNode
    {
        public Outport<CustomUnit> activatorPort;

        public override bool CanExecute(IEventContext payloadObj)
        {
            var payload = payloadObj as CustomActivationPayload;
            if (payload != null && payload.activator != null)
            {
                return true;
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var payload = GetPayload<CustomActivationPayload>();
            activatorPort.SetValue(payload.activator);
            return AbilityState.RUNNING;
        }
    }
}
