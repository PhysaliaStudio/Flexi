namespace Physalia.AbilityFramework.Tests
{
    public class CustomDamageEventNode : EntryNode
    {
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
    }
}
