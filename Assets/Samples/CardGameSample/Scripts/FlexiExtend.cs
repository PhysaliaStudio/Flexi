namespace Physalia.Flexi.Samples.CardGame
{
    public class AbilityContainer : AbilityDataContainer
    {
        public Unit unit;
        public Card card;

        public void CleanUp()
        {
            unit = null;
            card = null;
        }
    }

    public abstract class EntryNode<TEventContext> : EntryNode<AbilityContainer, TEventContext> where TEventContext : IEventContext
    {
        public Unit SelfUnit => Container.unit;
        public Card SelfCard => Container.card;
    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {
        public Unit SelfUnit => Container.unit;
        public Card SelfCard => Container.card;
    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {
        public Unit SelfUnit => Container.unit;
        public Card SelfCard => Container.card;
    }
}
