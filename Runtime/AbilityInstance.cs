namespace Physalia.AbilitySystem
{
    public sealed class AbilityInstance
    {
        private readonly AbilityGraph graph;

        internal AbilityInstance(AbilityGraph graph)
        {
            this.graph = graph;
        }

        public void Execute(object payload)
        {
            for (var i = 0; i < graph.Nodes.Count; i++)
            {
                graph.Nodes[i].Payload = payload;
            }

            graph.Reset(0);
            while (graph.MoveNext())
            {
                graph.Current.Run();
            }
        }
    }
}
