namespace Physalia.Flexi.Samples.ActionGame
{
    public class DefaultAbilityContainer : AbilityContainer
    {
        public Unit Unit;
    }

    public abstract class DefaultEntryNode : EntryNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultEntryNode<TEventContext> : EntryNode<DefaultAbilityContainer, TEventContext>
        where TEventContext : IEventContext
    {

    }

    public abstract class DefaultProcessNode : ProcessNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultValueNode : ValueNode<DefaultAbilityContainer>
    {

    }
}
