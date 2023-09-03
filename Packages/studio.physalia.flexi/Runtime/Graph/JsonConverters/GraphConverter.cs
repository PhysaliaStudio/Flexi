using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Physalia.Flexi
{
    internal class GraphConverter : JsonConverter<Graph>
    {
        public override Graph ReadJson(JsonReader reader, Type objectType, Graph existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            Graph graph = CreateGraphInstance(jsonObject);

            // Graph input/output
            ReadInputToken(graph, jsonObject);
            ReadOutputToken(graph, jsonObject);

            // Nodes
            JToken nodesToken = jsonObject[TokenKeys.GRAPH_NODES];
            if (nodesToken == null)
            {
                return graph;
            }

            List<Node> nodes = nodesToken.ToObject<List<Node>>();
            graph.AddNodes(nodes);

            // Edges
            JToken edgesToken = jsonObject[TokenKeys.GRAPH_EDGES];
            if (edgesToken != null)
            {
                // Rule: The port1 must be Outport, and the port2 must be Inport
                List<Edge> edges = edgesToken.ToObject<List<Edge>>();
                for (var i = 0; i < edges.Count; i++)
                {
                    Edge edge = edges[i];

                    Node node1 = graph.GetNode(edge.id1);
                    Port port1 = node1.GetPort(edge.port1);
                    if (port1 == null)
                    {
                        var missingOutport = new MissingOutport(node1, edge.port1, true);
                        node1.AddOutport(edge.port1, missingOutport);
                        port1 = missingOutport;
                    }

                    Node node2 = graph.GetNode(edge.id2);
                    Port port2 = node2.GetPort(edge.port2);
                    if (port2 == null)
                    {
                        var missingInport = new MissingInport(node2, edge.port2, true);
                        node2.AddInport(edge.port2, missingInport);
                        port2 = missingInport;
                    }

                    port1.ConnectForce(port2);
                }
            }

            // Handle data
            graph.HandleInvalidNodeIds();

            return graph;
        }

        private static void ReadInputToken(Graph graph, JObject jsonObject)
        {
            JToken token = jsonObject[TokenKeys.GRAPH_INPUT];
            if (token == null)
            {
                return;
            }

            // Create the node
            graph.AddGraphInputNode();

            // Setup
            GraphInputData inputData = token.ToObject<GraphInputData>();
            GraphInputNode node = graph.GraphInputNode;
            node.position = inputData.position;
            for (var i = 0; i < inputData.portDatas.Count; i++)
            {
                PortData portData = inputData.portDatas[i];

                Type portDataType = ReflectionUtilities.GetTypeByName(portData.type);
                if (portDataType == null)
                {
                    Logger.Error($"[{nameof(GraphConverter)}] Deserialize failed: Cannot find the type from all assemblies, typeName: {portData.type}");
                    continue;
                }

                node.CreateOutportWithArgumentType(portDataType, portData.name, true);
            }
        }

        private static void ReadOutputToken(Graph graph, JObject jsonObject)
        {
            JToken token = jsonObject[TokenKeys.GRAPH_OUTPUT];
            if (token == null)
            {
                return;
            }

            // Create the node
            graph.AddGraphOutputNode();

            // Setup
            GraphOutputData outputData = token.ToObject<GraphOutputData>();
            GraphOutputNode node = graph.GraphOutputNode;
            node.position = outputData.position;
            for (var i = 0; i < outputData.portDatas.Count; i++)
            {
                PortData portData = outputData.portDatas[i];

                Type portDataType = ReflectionUtilities.GetTypeByName(portData.type);
                if (portDataType == null)
                {
                    Logger.Error($"[{nameof(GraphConverter)}] Deserialize failed: Cannot find the type from all assemblies, typeName: {portData.type}");
                    continue;
                }

                node.CreateInportWithArgumentType(portDataType, portData.name, true);
            }
        }

        private static Graph CreateGraphInstance(JObject jsonObject)
        {
            JToken typeToken = jsonObject[TokenKeys.GRAPH_TYPE];
            if (typeToken == null)
            {
                Logger.Warn($"[{nameof(GraphConverter)}] Missing the type field! Will create Graph instead.");
                return new Graph();
            }

            string typeName = typeToken.ToString();
            Type type = ReflectionUtilities.GetTypeByName(typeName);
            if (type == null)
            {
                Logger.Error($"[{nameof(GraphConverter)}] Deserialize failed: Cannot find the type from all assemblies, typeName: {typeName}");
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
            writer.WritePropertyName(TokenKeys.GRAPH_TYPE);
            Type graphType = value.GetType();
            writer.WriteValue(graphType.FullName);

            // Graph input/output
            WriteInputToken(writer, value, serializer);
            WriteOutputToken(writer, value, serializer);

            // Nodes
            writer.WritePropertyName(TokenKeys.GRAPH_NODES);
            writer.WriteStartArray();

            for (var i = 0; i < value.Nodes.Count; i++)
            {
                serializer.Serialize(writer, value.Nodes[i]);
            }

            writer.WriteEndArray();

            // Calculate edges
            var edges = new List<Edge>();
            if (value.HasNode())
            {
                var totalNodes = new List<Node>(value.Nodes.Count + 2);
                if (value.GraphInputNode != null)
                {
                    totalNodes.Add(value.GraphInputNode);
                }
                if (value.GraphOutputNode != null)
                {
                    totalNodes.Add(value.GraphOutputNode);
                }
                totalNodes.AddRange(value.Nodes);

                var unhandledNodes = new HashSet<Node>(totalNodes);
                while (unhandledNodes.Count > 0)
                {
                    Node startNode = totalNodes.Find(x => unhandledNodes.Contains(x));
                    AddEdges(startNode, ref edges, ref unhandledNodes);
                }
            }

            // Edges
            writer.WritePropertyName(TokenKeys.GRAPH_EDGES);
            writer.WriteStartArray();

            for (var i = 0; i < edges.Count; i++)
            {
                serializer.Serialize(writer, edges[i]);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private static void WriteInputToken(JsonWriter writer, Graph graph, JsonSerializer serializer)
        {
            GraphInputNode inputNode = graph.GraphInputNode;
            if (inputNode == null)
            {
                return;
            }

            writer.WritePropertyName(TokenKeys.GRAPH_INPUT);

            writer.WriteStartObject();

            var inputData = new GraphInputData(inputNode);

            // Position
            writer.WritePropertyName(nameof(GraphInputData.position));
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(GraphInputData.position.x));
            writer.WriteValue(inputData.position.x);
            writer.WritePropertyName(nameof(GraphInputData.position.y));
            writer.WriteValue(inputData.position.y);
            writer.WriteEndObject();

            // Port datas
            writer.WritePropertyName(nameof(GraphInputData.portDatas));
            writer.WriteStartArray();
            for (var i = 0; i < inputData.portDatas.Count; i++)
            {
                serializer.Serialize(writer, inputData.portDatas[i]);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private static void WriteOutputToken(JsonWriter writer, Graph graph, JsonSerializer serializer)
        {
            GraphOutputNode outputNode = graph.GraphOutputNode;
            if (outputNode == null)
            {
                return;
            }

            writer.WritePropertyName(TokenKeys.GRAPH_OUTPUT);

            writer.WriteStartObject();

            var outputData = new GraphOutputData(outputNode);

            // Position
            writer.WritePropertyName(nameof(GraphOutputData.position));
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(GraphOutputData.position.x));
            writer.WriteValue(outputData.position.x);
            writer.WritePropertyName(nameof(GraphOutputData.position.y));
            writer.WriteValue(outputData.position.y);
            writer.WriteEndObject();

            // Port datas
            writer.WritePropertyName(nameof(GraphOutputData.portDatas));
            writer.WriteStartArray();
            for (var i = 0; i < outputData.portDatas.Count; i++)
            {
                serializer.Serialize(writer, outputData.portDatas[i]);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private void AddEdges(Node node, ref List<Edge> edges, ref HashSet<Node> unhandledNodes)
        {
            // Rule: The port1 must be Outport, and the port2 must be Inport

            var linkedNodes = new HashSet<Node>();

            IReadOnlyList<Outport> outports = node.Outports;
            for (var i = 0; i < outports.Count; i++)
            {
                Outport outport = outports[i];
                IReadOnlyList<Port> ports = outport.GetConnections();
                foreach (Port port in ports)
                {
                    if (port is not Inport inport)
                    {
                        continue;
                    }

                    if (!unhandledNodes.Contains(inport.Node))
                    {
                        continue;
                    }

                    var edge = new Edge
                    {
                        id1 = node.id,
                        port1 = outport.Name,
                        id2 = inport.Node.id,
                        port2 = inport.Name,
                    };
                    edges.Add(edge);
                    linkedNodes.Add(inport.Node);
                }
            }

            IReadOnlyList<Inport> inports = node.Inports;
            for (var i = 0; i < inports.Count; i++)
            {
                Inport inport = inports[i];
                IReadOnlyList<Port> ports = inport.GetConnections();
                foreach (Outport port in ports)
                {
                    if (port is not Outport outport)
                    {
                        continue;
                    }

                    if (!unhandledNodes.Contains(outport.Node))
                    {
                        continue;
                    }

                    var edge = new Edge
                    {
                        id1 = outport.Node.id,
                        port1 = outport.Name,
                        id2 = node.id,
                        port2 = inport.Name,
                    };
                    edges.Add(edge);
                    linkedNodes.Add(outport.Node);
                }
            }

            _ = unhandledNodes.Remove(node);
            foreach (Node linkedNode in linkedNodes)
            {
                if (!unhandledNodes.Contains(linkedNode))
                {
                    continue;
                }

                AddEdges(linkedNode, ref edges, ref unhandledNodes);
            }
        }
    }
}
