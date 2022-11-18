namespace Physalia.AbilityFramework.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class CustomDamageEventNode : EntryNode
    {
        public Outport<CustomUnit> instigatorPort;
        public Outport<CustomUnit> targetPort;

        public override bool CanExecute(object payloadObj)
        {
            var payload = payloadObj as CustomDamageEvent;
            if (payload == null)
            {
                return false;
            }

            if (payload.target.Owner == Instance.Owner)
            {
                return true;
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var payload = GetPayload<CustomDamageEvent>();
            instigatorPort.SetValue(payload.instigator);
            targetPort.SetValue(payload.target);
            return AbilityState.RUNNING;
        }
    }
}
