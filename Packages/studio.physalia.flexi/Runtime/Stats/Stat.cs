namespace Physalia.Flexi
{
    public class Stat
    {
        private readonly StatDefinition statDefinition;

        public int Id => statDefinition.Id;
        public int OriginalBase { get; private set; }
        public int CurrentBase { get; set; }
        public int CurrentValue { get; set; }

        internal Stat(StatDefinition statDefinition, int baseValue)
        {
            this.statDefinition = statDefinition;
            OriginalBase = baseValue;
            CurrentBase = baseValue;
            CurrentValue = baseValue;
        }
    }
}
