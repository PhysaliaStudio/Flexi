namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomDamageEventNode : EntryNode
    {
        public Outport<CustomUnit> instigatorPort;
        public Outport<CustomUnit> targetPort;

        public override bool CanExecute(IEventContext payloadObj)
        {
            var payload = payloadObj as CustomDamageEvent;
            if (payload == null)
            {
                return false;
            }

            if (payload.target == Actor)
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
