namespace Physalia.AbilityFramework.Tests
{
    public class CustomActivationNode : EntryNode
    {
        public Outport<CustomUnit> activatorPort;

        public override bool CanExecute()
        {
            var payload = GetPayload<CustomActivationPayload>();
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
