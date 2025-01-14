namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Entry)]
    public sealed class StartNode : EntryNode<EmptyContext>
    {
        public override bool CanExecute(EmptyContext context)
        {
            return true;
        }
    }
}
