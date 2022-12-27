namespace Physalia.AbilityFramework
{
    public sealed class AbilityFlow : IAbilityFlow
    {
        private readonly AbilitySystem system;
        private readonly Ability ability;
        private readonly AbilityGraph graph;

        public object userData;

        private Actor actor;
        private IEventContext payload;

        public AbilitySystem System => system;
        internal Ability Ability => ability;
        internal AbilityGraph Graph => graph;

        public Actor Actor => actor;
        internal IEventContext Payload => payload;
        public FlowNode Current => graph.Current;

        internal AbilityFlow(AbilityGraph graph) : this(null, graph, null)
        {

        }

        internal AbilityFlow(AbilitySystem system, AbilityGraph graph, Ability ability)
        {
            this.system = system;
            this.ability = ability;
            this.graph = graph;

            for (var i = 0; i < graph.Nodes.Count; i++)
            {
                graph.Nodes[i].flow = this;
            }
        }

        internal void SetOwner(Actor actor)
        {
            this.actor = actor;
        }

        public void SetPayload(IEventContext payload)
        {
            this.payload = payload;
        }

        public bool HasNext()
        {
            return graph.HasNext();
        }

        public bool MoveNext()
        {
            return graph.MoveNext();
        }

        public bool CanExecute(IEventContext payload)
        {
            if (graph.EntryNodes.Count == 0)
            {
                return false;
            }

            bool result = graph.EntryNodes[0].CanExecute(payload);
            return result;
        }

        internal void Push(FlowNode flowNode)
        {
            graph.Push(flowNode);
        }

        public void Reset()
        {
            graph.Reset(0);
            SetPayload(null);
        }
    }
}
