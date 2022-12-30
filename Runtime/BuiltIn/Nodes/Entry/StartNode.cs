namespace Physalia.Flexi
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
