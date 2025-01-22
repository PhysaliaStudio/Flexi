using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// AbilityFlow is a wrapper class of <see cref="AbilityGraph"/> for handling higher level logic,
    /// created and contained by <see cref="Flexi.Ability"/>.
    /// </summary>
    public sealed class AbilityFlow : IAbilityFlow
    {
        private readonly FlexiCore flexiCore;
        private readonly Ability ability;
        private readonly AbilityGraph graph;

        private IEventContext payload;

        internal FlexiCore Core => flexiCore;
        internal Ability Ability => ability;
        internal AbilityGraph Graph => graph;

        public IReadOnlyList<Node> EntryNodes => graph.EntryNodes;
        public IReadOnlyList<Node> Nodes => graph.Nodes;

        internal IEventContext Payload => payload;
        public FlowNode Current => graph.Current;

        internal AbilityFlow(FlexiCore flexiCore, Ability ability, AbilityGraph graph)
        {
            this.flexiCore = flexiCore;
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

        internal bool IsEntryAvailable(int entryIndex, IEventContext context)
        {
            if (entryIndex < 0 || entryIndex >= graph.EntryNodes.Count)
            {
                return false;
            }

            return graph.EntryNodes[entryIndex].CheckCanExecute(context);
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
