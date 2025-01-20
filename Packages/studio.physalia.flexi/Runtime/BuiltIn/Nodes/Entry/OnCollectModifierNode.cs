namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Entry)]
    internal class OnCollectModifierNode : EntryNode<AbilityDataContainer, OnCollectModifierNode.Context>
    {
        public sealed class Context : IEventContext
        {
            public static Context Instance { get; } = new Context();

            // Empty Content
        }

        public Variable<int> order;

        public override bool CanExecute(Context context)
        {
            return true;
        }
    }
}
