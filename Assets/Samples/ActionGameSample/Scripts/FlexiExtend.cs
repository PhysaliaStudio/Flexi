namespace Physalia.Flexi.Samples.ActionGame
{
    public class DefaultAbilityContainer : AbilityContainer
    {
        public Unit Unit;
    }

    public abstract class EntryNode : EntryNode<DefaultAbilityContainer>
    {

    }

    public abstract class FlowNode : FlowNode<DefaultAbilityContainer>
    {

    }

    public abstract class ProcessNode : ProcessNode<DefaultAbilityContainer>
    {

    }

    public abstract class ValueNode : ValueNode<DefaultAbilityContainer>
    {

    }
}
