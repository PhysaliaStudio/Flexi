namespace Physalia.AbilitySystem
{
    public sealed class AbilityInstance
    {
        private readonly AbilityGraph graph;

        internal AbilityInstance(AbilityGraph graph)
        {
            this.graph = graph;
        }

        public void Execute()
        {
            graph.Reset(0);
            while (graph.MoveNext())
            {
                graph.Current.Run();
            }
        }
    }
}
