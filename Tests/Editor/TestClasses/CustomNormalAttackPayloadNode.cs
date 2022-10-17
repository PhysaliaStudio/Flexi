namespace Physalia.AbilitySystem.Tests
{
    public class CustomNormalAttackPayloadNode : ValueNode
    {
        public Outport<CustomUnit> attackerPort;
        public Outport<CustomUnit> mainTargetPort;

        protected override void EvaluateSelf()
        {
            var payload = GetPayload<CustomNormalAttackPayload>();
            attackerPort.SetValue(payload.attacker);
            mainTargetPort.SetValue(payload.mainTarget);
        }
    }
}
