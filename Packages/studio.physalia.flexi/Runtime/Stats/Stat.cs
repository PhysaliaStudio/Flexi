namespace Physalia.Flexi
{
    public class Stat
    {
        private readonly int id;

        public int Id => id;
        public int OriginalBase { get; private set; }
        public int CurrentBase { get; set; }
        public int CurrentValue { get; set; }

        internal Stat(int id, int baseValue)
        {
            this.id = id;
            OriginalBase = baseValue;
            CurrentBase = baseValue;
            CurrentValue = baseValue;
        }
    }
}
