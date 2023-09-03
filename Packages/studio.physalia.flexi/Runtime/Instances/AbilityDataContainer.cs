namespace Physalia.Flexi
{
    public class AbilityDataContainer
    {
        private Actor actor;
        private AbilityDataSource dataSource;

        public Actor Actor { get => actor; internal set => actor = value; }
        public AbilityDataSource DataSource { get => dataSource; set => dataSource = value; }
    }
}
