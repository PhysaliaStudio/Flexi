namespace Physalia.AbilityFramework
{
    public sealed class StartNode : EntryNode
    {
        public override bool CanExecute(object payloadObj)
        {
            return payloadObj is not StatRefreshEvent;
        }
    }
}
