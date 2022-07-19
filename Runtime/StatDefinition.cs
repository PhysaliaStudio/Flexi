namespace Physalia.Stats
{
    public class StatDefinition
    {
        public int Id;
        public string Name;

        public override string ToString()
        {
            return $"<{Id}:{Name}>";
        }
    }
}
