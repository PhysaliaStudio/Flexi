using System.Collections.Generic;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    public class AbilityInstance
    {
        private readonly Ability ability;
        private readonly Dictionary<Node, NodeLogic> nodeToLogic = new();

        private FlowNode current;

        public FlowNode Current => current;

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

        public NodeLogic GetNodeLogic(Node node)
        {
            if (nodeToLogic.TryGetValue(node, out NodeLogic nodeLogic))
            {
                return nodeLogic;
            }

            Debug.LogError($"[{nameof(AbilityInstance)}] GetNodeLogic failed! Node does not belong to this ability");
            return null;
        }

        public void Reset(int indexOfEntryNode)
        {
            if (indexOfEntryNode < 0 || indexOfEntryNode >= ability.EntryNodes.Count)
            {
                return;
            }

            current = ability.EntryNodes[indexOfEntryNode];
        }

        public bool MoveNext()
        {
            if (current != null)
            {
                current = current.Next;
            }

            return current != null;
        }
    }
}
