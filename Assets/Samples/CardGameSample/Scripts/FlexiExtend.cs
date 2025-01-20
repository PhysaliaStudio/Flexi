namespace Physalia.Flexi.Samples.CardGame
{
    public class AbilityContainer : AbilityDataContainer
    {
        private readonly Game game;
        public Unit Unit;
        public Card Card;

        public Game Game => game;

        public AbilityContainer(Game game, AbilityHandle handle)
        {
            this.game = game;
            Handle = handle;
        }

        public void CleanUp()
        {
            Unit = null;
            Card = null;
        }
    }

    public abstract class EntryNode<TEventContext> : EntryNode<AbilityContainer, TEventContext> where TEventContext : IEventContext
    {

    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {

    }

    public abstract class FlowNode : FlowNode<AbilityContainer>
    {

    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {

    }
}
