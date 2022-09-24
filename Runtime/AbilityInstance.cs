using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public class AbilityInstance
    {
        private readonly AbilityGraph abilityGraph;
        private readonly Dictionary<Node, NodeLogic> nodeToLogic = new();

        private FlowNode currentNode;
        private NodeLogic currentLogic;

        public NodeLogic Current => currentLogic;

        internal AbilityInstance(AbilityGraph abilityGraph)
        {
            this.abilityGraph = abilityGraph;
        }

        internal void Initialize()
        {
            for (var i = 0; i < abilityGraph.Nodes.Count; i++)
            {
                Node node = abilityGraph.Nodes[i];
                NodeLogic nodeLogic = NodeLogicFactory.Create(node);
                nodeToLogic.Add(node, nodeLogic);
            }

            Reset(0);
        }

        public void Reset(int indexOfEntryNode)
        {
            if (indexOfEntryNode < 0 || indexOfEntryNode >= abilityGraph.EntryNodes.Count)
            {
                currentNode = null;
                currentLogic = null;
                return;
            }

            currentNode = abilityGraph.EntryNodes[indexOfEntryNode];
            currentLogic = nodeToLogic[currentNode];
        }

        public bool MoveNext()
        {
            if (currentNode == null)
            {
                return false;
            }

            currentNode = currentNode.Next;
            if (currentNode == null)
            {
                currentLogic = null;
                return false;
            }

            currentLogic = nodeToLogic[currentNode];
            return true;
        }
    }
}
