using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    internal class GraphConverter : JsonConverter<Graph>
    {
        private const string TYPE_KEY = "_type";
        private const string NODES_KEY = "nodes";
        private const string EDGES_KEY = "edges";

        public override Graph ReadJson(JsonReader reader, Type objectType, Graph existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            Graph graph = CreateGraphInstance(jsonObject);

            // Nodes
            JToken nodesToken = jsonObject[NODES_KEY];
            if (nodesToken == null)
            {
                return graph;
            }

            List<Node> nodes = nodesToken.ToObject<List<Node>>();
            graph.AddNodesInternal(nodes);

            // Edges
            JToken edgesToken = jsonObject[EDGES_KEY];
            if (edgesToken != null)
            {
                List<Edge> edges = edgesToken.ToObject<List<Edge>>();
                for (var i = 0; i < edges.Count; i++)
                {
                    Edge edge = edges[i];

                    Node node1 = graph.GetNode(edge.id1);
                    Port port1 = node1.GetPort(edge.port1);

                    Node node2 = graph.GetNode(edge.id2);
                    Port port2 = node2.GetPort(edge.port2);

                    port1.Connect(port2);
                }
            }

            return graph;
        }

        private static Graph CreateGraphInstance(JObject jsonObject)
        {
            JToken typeToken = jsonObject[TYPE_KEY];
            if (typeToken == null)
            {
                Debug.LogWarning($"[{nameof(GraphConverter)}] Missing the type field! Will create Graph instead.");
                return new Graph();
            }

            string typeName = typeToken.ToString();
            Type type = ReflectionUtilities.GetTypeByName(typeName);
            if (type == null)
            {
                Debug.LogError($"[{nameof(GraphConverter)}] Deserialize failed: Cannot find the type from all assemblies, typeName: {typeName}");
                return new Graph();
            }

            return Activator.CreateInstance(type) as Graph;
        }

        public override void WriteJson(JsonWriter writer, Graph value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            // Type
            writer.WritePropertyName(TYPE_KEY);
            Type graphType = value.GetType();
            writer.WriteValue(graphType.FullName);

            // Nodes
            writer.WritePropertyName(NODES_KEY);
            writer.WriteStartArray();

            for (var i = 0; i < value.Nodes.Count; i++)
            {
                serializer.Serialize(writer, value.Nodes[i]);
            }

            writer.WriteEndArray();

            // Calculate edges
            var edges = new List<Edge>();
            var handledNodes = new HashSet<Node>();
            Node node = value.Nodes[0];
            AddEdges(node, ref edges, ref handledNodes);

            // Edges
            writer.WritePropertyName(EDGES_KEY);
            writer.WriteStartArray();

            for (var i = 0; i < edges.Count; i++)
            {
                serializer.Serialize(writer, edges[i]);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private void AddEdges(Node node, ref List<Edge> edges, ref HashSet<Node> handledNodes)
        {
            var linkedNodes = new HashSet<Node>();

            foreach (Outport outport in node.Outports)
            {
                IReadOnlyList<Port> ports = outport.GetConnections();
                foreach (Port port in ports)
                {
                    if (port is not Inport inport)
                    {
                        continue;
                    }

                    if (handledNodes.Contains(inport.node))
                    {
                        continue;
                    }

                    var edge = new Edge
                    {
                        id1 = node.id,
                        port1 = outport.name,
                        id2 = inport.node.id,
                        port2 = inport.name,
                    };
                    edges.Add(edge);
                    linkedNodes.Add(inport.node);
                }
            }

            foreach (Inport inport in node.Inports)
            {
                IReadOnlyList<Port> ports = inport.GetConnections();
                foreach (Outport port in ports)
                {
                    if (port is not Outport outport)
                    {
                        continue;
                    }

                    if (handledNodes.Contains(outport.node))
                    {
                        continue;
                    }

                    var edge = new Edge
                    {
                        id1 = node.id,
                        port1 = inport.name,
                        id2 = outport.node.id,
                        port2 = outport.name,
                    };
                    edges.Add(edge);
                    linkedNodes.Add(outport.node);
                }
            }

            handledNodes.Add(node);
            foreach (Node linkedNode in linkedNodes)
            {
                if (handledNodes.Contains(linkedNode))
                {
                    continue;
                }

                AddEdges(linkedNode, ref edges, ref handledNodes);
            }
        }
    }
}
