namespace Physalia.Stats
{
    public class Stat
    {
        private readonly StatDefinition statDefinition;

        public int Id => statDefinition.Id;
        public int Value;

        internal Stat(StatDefinition statDefinition)
        {
            this.statDefinition = statDefinition;
        }
    }
}
