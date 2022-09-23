using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public class AbilityInstance
    {
        private readonly Ability ability;
        private readonly Dictionary<Node, NodeLogic> nodeToLogic = new();

        private FlowNode currentNode;
        private NodeLogic currentLogic;

        public NodeLogic Current => currentLogic;

        internal AbilityInstance(Ability ability)
        {
            this.ability = ability;
            for (var i = 0; i < ability.Nodes.Count; i++)
            {
                Node node = ability.Nodes[i];
                NodeLogic nodeLogic = NodeLogicFactory.Create(node);
                nodeToLogic.Add(node, nodeLogic);
            }
        }

        public void Reset(int indexOfEntryNode)
        {
            if (indexOfEntryNode < 0 || indexOfEntryNode >= ability.EntryNodes.Count)
            {
                return;
            }

            currentNode = ability.EntryNodes[indexOfEntryNode];
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
