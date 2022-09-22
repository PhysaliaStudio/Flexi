using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    [JsonConverter(typeof(GrpahConverter))]
    public class Graph
    {
        private readonly List<EntryNode> entryNodes = new();
        internal List<Node> nodes = new();

        public IReadOnlyList<EntryNode> EntryNodes => entryNodes;

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
            nodes.Add(node);
            if (node is EntryNode entryNode)
            {
                entryNodes.Add(entryNode);
            }

            return node;
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
