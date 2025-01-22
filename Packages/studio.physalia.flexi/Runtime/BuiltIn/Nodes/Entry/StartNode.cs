namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Entry)]
    public sealed class StartNode : EntryNode<AbilityContainer>
    {
        protected override FlowState OnExecute()
        {
            return FlowState.Success;
        }
    }
}
