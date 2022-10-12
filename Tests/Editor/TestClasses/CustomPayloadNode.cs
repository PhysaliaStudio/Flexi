namespace Physalia.AbilitySystem.Tests
{
    public class CustomPayloadNode : ValueNode
    {
        public Outport<CustomUnit> owner;

        public override void Evaluate()
        {
            CustomPayload payload = GetPayload<CustomPayload>();
            owner.SetValue(payload.owner);
        }
    }
}
