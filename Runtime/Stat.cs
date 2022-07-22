namespace Physalia.Stats
{
    public class Stat
    {
        private readonly StatDefinition statDefinition;

        public int Id => statDefinition.Id;
        public int OriginalValue { get; private set; }
        public int CurrentValue { get; internal set; }

        internal Stat(StatDefinition statDefinition, int originalValue)
        {
            this.statDefinition = statDefinition;
            OriginalValue = originalValue;
            CurrentValue = originalValue;
        }
    }
}
