namespace Physalia.AbilityFramework.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class CustomNormalAttackEntryNode : EntryNode
    {
        public Outport<CustomUnit> attackerPort;
        public Outport<CustomUnit> targetPort;

        public override bool CanExecute(IEventContext payloadObj)
        {
            var payload = payloadObj as CustomNormalAttackPayload;
            if (payload != null && payload.attacker != null && payload.mainTarget != null)
            {
                return true;
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var payload = GetPayload<CustomNormalAttackPayload>();
            attackerPort.SetValue(payload.attacker);
            targetPort.SetValue(payload.mainTarget);
            return AbilityState.RUNNING;
        }
    }
}
