namespace Physalia.Flexi
{
    public class EmptyContext : IEventContext
    {
        public static EmptyContext Instance { get; } = new EmptyContext();

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
