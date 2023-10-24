namespace Physalia.Flexi
{
    internal sealed class StatRefreshEvent : IEventContext
    {
        // Empty Content
    }

    [NodeCategory(BuiltInCategory.Entry)]
    public class StatRefreshEventNode : EntryNode
    {
        public Variable<int> order;

        public override bool CanExecute(IEventContext payloadObj)
        {
            return payloadObj is StatRefreshEvent;
        }
    }
}
