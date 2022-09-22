using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    [JsonConverter(typeof(GraphConverter))]
    public class Graph
    {
        private readonly List<EntryNode> entryNodes = new();
        private readonly List<Node> nodes = new();

        public IReadOnlyList<EntryNode> EntryNodes => entryNodes;

        public IReadOnlyList<Node> Nodes => nodes;

        public void ReorderNodes()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].id = i + 1;
            }
        }

        public T AddNode<T>() where T : Node, new()
        {
            T node = NodeFactory.Create<T>();
            AddNodeInternal(node);
            return node;
        }

        internal void AddNodesInternal(IReadOnlyList<Node> newNodes)
        {
            nodes.AddRange(newNodes);
            for (var i = 0; i < newNodes.Count; i++)
            {
                if (newNodes[i] is EntryNode entryNode)
                {
                    entryNodes.Add(entryNode);
                }
            }
        }

        internal void AddNodeInternal(Node newNode)
        {
            nodes.Add(newNode);
            if (newNode is EntryNode entryNode)
            {
                entryNodes.Add(entryNode);
            }
        }

        public void RemoveNode(Node node)
        {
            bool success = nodes.Remove(node);
            if (success)
            {
                if (node is EntryNode entryNode)
                {
                    entryNodes.Remove(entryNode);
                }
            }
            else
            {
                Debug.LogWarning($"[{nameof(Graph)}] Try to delete node, but the node doesn't belong to this graph");
            }

            return;
        }

        public Node GetNode(int id)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].id == id)
                {
                    return nodes[i];
                }
            }

            return null;
        }
    }
}
