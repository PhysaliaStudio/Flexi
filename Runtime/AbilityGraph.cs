using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public class AbilityGraph : Graph
    {
        private FlowNode currentNode;
        private int indexOfEntryNode;
        private bool isRunning;

        private readonly Stack<FlowNode> stack = new();

        public FlowNode Current => currentNode;

        public void Reset(int indexOfEntryNode)
        {
            currentNode = null;
            this.indexOfEntryNode = indexOfEntryNode;
            isRunning = false;

            stack.Clear();

            IReadOnlyList<Node> nodes = Nodes;
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].Reset();
            }
        }

        public bool MoveNext()
        {
            if (currentNode == null)
            {
                if (isRunning)
                {
                    return false;
                }

                if (indexOfEntryNode < 0 || indexOfEntryNode >= EntryNodes.Count)
                {
                    return false;
                }

                isRunning = true;
                currentNode = EntryNodes[indexOfEntryNode];
                return true;
            }

            currentNode = currentNode.Next;
            if (currentNode != null)
            {
                return true;
            }
            else if (stack.Count > 0)
            {
                FlowNode flowNode = stack.Pop();
                currentNode = flowNode;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Push(FlowNode flowNode)
        {
            stack.Push(flowNode);
        }
    }
}
