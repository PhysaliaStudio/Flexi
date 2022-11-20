namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Entry")]
    public sealed class StartNode : EntryNode
    {
        public override bool CanExecute(IEventContext payloadObj)
        {
            return payloadObj is not StatRefreshEvent;
        }
    }
}
