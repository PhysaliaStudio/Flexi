namespace Physalia.AbilityFramework
{
    internal sealed class StatRefreshEvent : IEventContext
    {
        // Empty Content
    }

    [NodeCategory("Built-in/Entry")]
    public class StatRefreshEventNode : EntryNode
    {
        public override bool CanExecute(IEventContext payloadObj)
        {
            return payloadObj is StatRefreshEvent;
        }
    }
}
