namespace Physalia.Flexi.Tests
{
    public class AbilityContainer : AbilityDataContainer
    {
        public AbilitySystemWrapperDefault SystemWrapper;
        public Actor Actor;
    }

    public abstract class EntryNode<TEventContext> : EntryNode<AbilityContainer, TEventContext> where TEventContext : IEventContext
    {

    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {

    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {

    }
}
