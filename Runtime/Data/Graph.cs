using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    [JsonConverter(typeof(GraphConverter))]
    public class Graph
    {
        private readonly List<EntryNode> entryNodes = new();
        private readonly List<Node> nodes = new();

        public IReadOnlyList<EntryNode> EntryNodes => entryNodes;

        public IReadOnlyList<Node> Nodes => nodes;

        public T AddNewNode<T>() where T : Node, new()
        {
            return AddNewNode(typeof(T)) as T;
        }

        public Node AddNewNode(Type type)
        {
            Node node = NodeFactory.Create(type);
            GenerateNodeId(node);
            AddNode(node);
            return node;
        }

        public void AddNodes(IReadOnlyList<Node> newNodes)
        {
            for (var i = 0; i < newNodes.Count; i++)
            {
                AddNode(newNodes[i]);
            }
        }

        public void AddNode(Node newNode)
        {
            nodes.Add(newNode);
            if (newNode is EntryNode entryNode)
            {
                entryNodes.Add(entryNode);
            }
        }

        public void RemoveNode(Node node)
        {
            node.DisconnectAllPorts();

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
                Logger.Warn($"[{nameof(Graph)}] Try to delete node, but the node doesn't belong to this graph");
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

        private void GenerateNodeId(Node node)
        {
            do
            {
                node.id = UnityEngine.Random.Range(1, 1000000);
            }
            while (!IsNodeIdValid(node));
        }

        private bool IsNodeIdValid(Node node)
        {
            if (node.id <= 0)
            {
                return false;
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] == node)
                {
                    continue;
                }

                if (nodes[i].id == node.id)
                {
                    return false;
                }
            }

            return true;
        }

        public void HandleInvalidNodeIds()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (!IsNodeIdValid(nodes[i]))
                {
                    GenerateNodeId(nodes[i]);
                }
            }
        }
    }
}
