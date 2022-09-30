namespace Physalia.AbilitySystem.Tests
{
    public class CustomDamageEventNode : EntryNode
    {
        public override bool CanExecute()
        {
            CustomPayload payload = GetPayload<CustomPayload>();
            if (payload == null)
            {
                return false;
            }

            if (payload.instigator == payload.owner)
            {
                return true;
            }

            return false;
        }
    }
}
