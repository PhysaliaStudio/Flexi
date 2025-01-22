namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Entry)]
    public sealed class StartNode : EntryNode<AbilityContainer>
    {
        protected override AbilityState OnExecute()
        {
            return AbilityState.RUNNING;
        }
    }
}
