namespace Physalia.AbilitySystem.Tests
{
    public class CustomDamageEventNode : EntryNode
    {
        public override bool CanExecute()
        {
            var payload = GetPayload<CustomDamageEvent>();
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
    }
}
