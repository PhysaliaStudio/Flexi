namespace Physalia.AbilityFramework.Tests
{
    public class CustomActivationPayloadNode : ValueNode
    {
        public Outport<CustomUnit> activatorPort;

        protected override void EvaluateSelf()
        {
            activatorPort.SetValue(GetPayload<CustomActivationPayload>().activator);
        }
    }
}
