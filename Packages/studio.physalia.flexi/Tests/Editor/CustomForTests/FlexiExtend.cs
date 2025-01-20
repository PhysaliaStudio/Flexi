namespace Physalia.Flexi.Tests
{
    public class DefaultAbilityContainer : AbilityContainer
    {
        public AbilitySystemWrapperDefault SystemWrapper;
        public Actor Actor;
    }

    public abstract class EntryNode<TEventContext> : EntryNode<DefaultAbilityContainer, TEventContext> where TEventContext : IEventContext
    {

    }

    public abstract class ProcessNode : ProcessNode<DefaultAbilityContainer>
    {

    }

    public abstract class ValueNode : ValueNode<DefaultAbilityContainer>
    {

    }
}
