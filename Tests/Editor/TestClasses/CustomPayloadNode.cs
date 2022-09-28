namespace Physalia.AbilitySystem.Tests
{
    public class CustomPayloadNode : ValueNode
    {
        public Outport<Character> owner;

        public override void Evaluate()
        {
            owner.SetValue(((CustomPayload)Payload).owner);
        }
    }
}
