namespace Physalia.Flexi.Samples.ActionGame
{
    public class AbilityContainer : AbilityDataContainer
    {
        public Unit Unit;
    }

    public abstract class EntryNode : EntryNode<AbilityContainer>
    {

    }

    public abstract class FlowNode : FlowNode<AbilityContainer>
    {

    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {

    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {

    }
}
