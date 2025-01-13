namespace Physalia.Flexi
{
    public class EmptyContext : IEventContext
    {
        // Empty Content
    }

    [NodeCategory(BuiltInCategory.Entry)]
    public sealed class StartNode : EntryNode<EmptyContext>
    {
        public override bool CanExecute(EmptyContext context)
        {
            return true;
        }
    }
}
