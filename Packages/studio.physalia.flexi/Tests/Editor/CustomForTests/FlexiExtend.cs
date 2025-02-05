namespace Physalia.Flexi.Tests
{
    public class DefaultAbilityContainer : AbilityContainer
    {
        public CustomFlexiCoreWrapper CoreWrapper;
        public Actor Actor;

        public DefaultAbilityContainer(AbilityData abilityData, int groupIndex)
            : base(abilityData, groupIndex)
        {

        }
    }

    public abstract class DefaultEntryNode<TEventContext>
        : EntryNode<DefaultAbilityContainer, TEventContext>
        where TEventContext : IEventContext
    {

    }

    public abstract class DefaultEntryNode<TEventContext, TResumeContext>
        : EntryNode<DefaultAbilityContainer, TEventContext, TResumeContext>
        where TEventContext : IEventContext
        where TResumeContext : IResumeContext
    {

    }

    public abstract class DefaultProcessNode : ProcessNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultProcessNode<TResumeContext>
        : ProcessNode<DefaultAbilityContainer, TResumeContext>
        where TResumeContext : IResumeContext
    {

    }

    public abstract class DefaultModifierNode : ModifierNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultValueNode : ValueNode<DefaultAbilityContainer>
    {

    }
}
