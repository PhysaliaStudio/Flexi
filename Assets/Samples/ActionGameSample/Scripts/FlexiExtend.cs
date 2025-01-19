namespace Physalia.Flexi.Samples.ActionGame
{
    public class AbilityContainer : AbilityDataContainer
    {
        public Unit unit;
    }

    public abstract class EntryNode : EntryNode<AbilityContainer>
    {
        public Unit SelfUnit => Container.unit;
    }

    public abstract class FlowNode : FlowNode<AbilityContainer>
    {
        public Unit SelfUnit => Container.unit;
    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {
        public Unit SelfUnit => Container.unit;
    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {
        public Unit SelfUnit => Container.unit;
    }
}
