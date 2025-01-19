namespace Physalia.Flexi.Tests
{
    public class AbilityContainer : AbilityDataContainer
    {
        public AbilitySystemWrapperDefault systemWrapper;
        public Actor actor;
    }

    public abstract class EntryNode<TEventContext> : EntryNode<AbilityContainer, TEventContext> where TEventContext : IEventContext
    {
        public AbilitySystemWrapperDefault SystemWrapper => Container.systemWrapper;
        public Actor Actor => Container.actor;
    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {
        public AbilitySystemWrapperDefault SystemWrapper => Container.systemWrapper;
        public Actor Actor => Container.actor;
    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {
        public AbilitySystemWrapperDefault SystemWrapper => Container.systemWrapper;
        public Actor Actor => Container.actor;
    }
}
