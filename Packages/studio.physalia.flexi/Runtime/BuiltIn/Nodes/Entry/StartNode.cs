namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Entry)]
    public sealed class StartNode : EntryNode<AbilityContainer, EmptyEventContext>
    {
        protected internal override bool CanExecute(EmptyEventContext context)
        {
            return true;
        }

        protected override FlowState OnExecute(EmptyEventContext context)
        {
            return FlowState.Success;
        }
    }
}
