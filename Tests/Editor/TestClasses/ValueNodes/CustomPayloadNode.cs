namespace Physalia.AbilityFramework.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class CustomPayloadNode : ValueNode
    {
        public Outport<CustomUnit> owner;

        protected override void EvaluateSelf()
        {
            CustomPayload payload = GetPayload<CustomPayload>();
            owner.SetValue(payload.owner);
        }
    }
}
