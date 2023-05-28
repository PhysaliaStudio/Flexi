using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// AbilityGraph is a graph that iterate the ability.
    /// </summary>
    public class AbilityGraph : Graph
    {
        private FlowNode currentNode;
        private int indexOfEntryNode;
        private bool isRunning;

        private readonly Stack<FlowNode> nodeStack = new();
        private readonly Stack<Graph> graphStack = new();

        public FlowNode Current => currentNode;
        public EntryNode EntryNodeAssigned
        {
            get
            {
                if (indexOfEntryNode < 0 || indexOfEntryNode >= EntryNodes.Count)
                {
                    return null;
                }

                return EntryNodes[indexOfEntryNode];
            }
        }

        public void Reset(int indexOfEntryNode)
        {
            currentNode = null;
            this.indexOfEntryNode = indexOfEntryNode;
            isRunning = false;

            nodeStack.Clear();
            graphStack.Clear();

            IReadOnlyList<Node> nodes = Nodes;
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].Reset();
            }
        }

        public bool HasNext()
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

                return true;
            }

            if (currentNode.Next != null)
            {
                return true;
            }
            else if (nodeStack.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
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
            else if (nodeStack.Count > 0)
            {
                if (graphStack.Count > 0)
                {
                    graphStack.Pop();
                }

                FlowNode flowNode = nodeStack.Pop();
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
            nodeStack.Push(flowNode);
        }

        internal void PushGraph(AbilityGraph graph)
        {
            nodeStack.Push(currentNode);
            graphStack.Push(graph);
        }
    }
}
