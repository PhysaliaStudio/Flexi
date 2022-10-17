namespace Physalia.AbilitySystem
{
    public sealed class StartNode : EntryNode
    {
        public override bool CanExecute()
        {
            return Instance.Payload is not StatRefreshEvent;
        }
    }
}
