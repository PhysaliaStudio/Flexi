namespace Physalia.Flexi.Samples.CardGame
{
    public class DefaultAbilityContainer : AbilityContainer
    {
        private readonly Game game;
        public Unit Unit;
        public Card Card;

        public Game Game => game;

        public DefaultAbilityContainer(Game game, AbilityHandle handle)
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

    public abstract class EntryNode<TEventContext> : EntryNode<DefaultAbilityContainer, TEventContext> where TEventContext : IEventContext
    {

    }

    public abstract class ProcessNode : ProcessNode<DefaultAbilityContainer>
    {

    }

    public abstract class FlowNode : FlowNode<DefaultAbilityContainer>
    {

    }

    public abstract class ValueNode : ValueNode<DefaultAbilityContainer>
    {

    }
}
