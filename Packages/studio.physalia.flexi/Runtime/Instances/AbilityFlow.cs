using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// AbilityFlow is a wrapper class of <see cref="AbilityGraph"/> for handling higher level logic,
    /// created and contained by <see cref="Flexi.Ability"/>.
    /// </summary>
    public sealed class AbilityFlow : IAbilityFlow
    {
        private readonly AbilitySystem system;
        private readonly Ability ability;
        private readonly AbilityGraph graph;

        public object userData;

        private IEventContext payload;

        public AbilitySystem System => system;
        internal Ability Ability => ability;
        internal AbilityGraph Graph => graph;

        public IReadOnlyList<Node> EntryNodes => graph.EntryNodes;
        public IReadOnlyList<Node> Nodes => graph.Nodes;

        public Actor Actor => ability.Actor;
        internal IEventContext Payload => payload;
        public FlowNode Current => graph.Current;

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

        internal int GetAvailableEntry(IEventContext payload)
        {
            if (graph.EntryNodes.Count == 0)
            {
                return -1;
            }

            IReadOnlyList<EntryNode> entryNodes = graph.EntryNodes;
            for (var i = 0; i < entryNodes.Count; i++)
            {
                bool success = entryNodes[i].CheckCanExecute(payload);
                if (success)
                {
                    return i;
                }
            }

            return -1;
        }

        internal bool CanStatRefresh()
        {
            if (graph.EntryNodes.Count == 0)
            {
                return false;
            }

            bool result = graph.EntryNodes[0] is StatRefreshEventNode;
            return result;
        }

        internal void Push(FlowNode flowNode)
        {
            graph.Push(flowNode);
        }

        public void Reset(int entryIndex = 0)
        {
            graph.Reset(entryIndex);
            SetPayload(null);
        }
    }
}
