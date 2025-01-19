namespace Physalia.Flexi.Samples.CardGame
{
    public class AbilityContainer : AbilityDataContainer
    {
        private readonly Game game;
        public Unit unit;
        public Card card;

        public Game Game => game;

        public AbilityContainer(Game game, AbilityDataSource dataSource)
        {
            this.game = game;
            DataSource = dataSource;
        }

        public void CleanUp()
        {
            unit = null;
            card = null;
        }
    }

    public abstract class EntryNode<TEventContext> : EntryNode<AbilityContainer, TEventContext> where TEventContext : IEventContext
    {
        public Game Game => Container.Game;
        public Unit SelfUnit => Container.unit;
        public Card SelfCard => Container.card;
    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {
        public Game Game => Container.Game;
        public Unit SelfUnit => Container.unit;
        public Card SelfCard => Container.card;
    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {
        public Game Game => Container.Game;
        public Unit SelfUnit => Container.unit;
        public Card SelfCard => Container.card;
    }
}
