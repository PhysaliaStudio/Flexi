namespace Physalia.AbilitySystem.Tests
{
    public class CustomPayloadNode : ValueNode
    {
        public Outport<Character> owner;

        public override void Evaluate()
        {
            CustomPayload payload = GetPayload<CustomPayload>();
            owner.SetValue(payload.owner);
        }
    }
}
