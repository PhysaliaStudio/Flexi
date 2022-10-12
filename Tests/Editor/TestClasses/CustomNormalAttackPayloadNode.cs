namespace Physalia.AbilitySystem.Tests
{
    public class CustomNormalAttackPayloadNode : ValueNode
    {
        public Outport<CustomUnit> attackerPort;
        public Outport<CustomUnit> mainTargetPort;

        public override void Evaluate()
        {
            var payload = GetPayload<CustomNormalAttackPayload>();
            attackerPort.SetValue(payload.attacker);
            mainTargetPort.SetValue(payload.mainTarget);
        }
    }
}
