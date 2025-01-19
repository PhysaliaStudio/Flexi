namespace Physalia.Flexi
{
    internal sealed class StatRefreshEvent : IEventContext
    {
        // Empty Content
    }

    [NodeCategory(BuiltInCategory.Entry)]
    internal class StatRefreshEventNode : EntryNode<AbilityDataContainer, StatRefreshEvent>
    {
        public Variable<int> order;

        public override bool CanExecute(StatRefreshEvent context)
        {
            return true;
        }
    }
}
