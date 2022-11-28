using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Physalia.AbilityFramework
{
    internal class GraphConverter : JsonConverter<Graph>
    {
        private const string TYPE_KEY = "_type";
        private const string INPUT_KEY = "$input";
        private const string OUTPUT_KEY = "$output";
        private const string VARIABLE_KEY = "variables";
        private const string NODES_KEY = "nodes";
        private const string EDGES_KEY = "edges";

        public override Graph ReadJson(JsonReader reader, Type objectType, Graph existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            Graph graph = CreateGraphInstance(jsonObject);

            // Graph input/output
            ReadInputToken(graph, jsonObject);
            ReadOutputToken(graph, jsonObject);

            // Variables
            JToken variablesToken = jsonObject[VARIABLE_KEY];
            if (variablesToken != null)
            {
                List<BlackboardVariable> variables = variablesToken.ToObject<List<BlackboardVariable>>();
                graph.AddVariables(variables);
            }

            // Nodes
            JToken nodesToken = jsonObject[NODES_KEY];
            if (nodesToken == null)
            {
                return graph;
            }

            List<Node> nodes = nodesToken.ToObject<List<Node>>();
            graph.AddNodes(nodes);

            // Edges
            JToken edgesToken = jsonObject[EDGES_KEY];
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
                        var missingOutport = new MissingOutport(node1, edge.port1);
                        node1.AddOutport(edge.port1, missingOutport);
                        port1 = missingOutport;
                    }

                    Node node2 = graph.GetNode(edge.id2);
                    Port port2 = node2.GetPort(edge.port2);
                    if (port2 == null)
                    {
                        var missingInport = new MissingInport(node2, edge.port2);
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
            JToken token = jsonObject[INPUT_KEY];
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

                PortFactory.CreateOutport(node, portDataType, portData.name);
            }
        }

        private static void ReadOutputToken(Graph graph, JObject jsonObject)
        {
            JToken token = jsonObject[OUTPUT_KEY];
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

                PortFactory.CreateInport(node, portDataType, portData.name);
            }
        }

        private static Graph CreateGraphInstance(JObject jsonObject)
        {
            JToken typeToken = jsonObject[TYPE_KEY];
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
            writer.WritePropertyName(TYPE_KEY);
            Type graphType = value.GetType();
            writer.WriteValue(graphType.FullName);

            // Graph input/output
            WriteInputToken(writer, value, serializer);
            WriteOutputToken(writer, value, serializer);

            // Variable
            writer.WritePropertyName(VARIABLE_KEY);
            writer.WriteStartArray();
            for (var i = 0; i < value.BlackboardVariables.Count; i++)
            {
                serializer.Serialize(writer, value.BlackboardVariables[i]);
            }
            writer.WriteEndArray();

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
            if (value.Nodes.Count > 0)
            {
                var handledNodes = new HashSet<Node>();
                Node node = value.Nodes[0];
                AddEdges(node, ref edges, ref handledNodes);
            }

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

        private static void WriteInputToken(JsonWriter writer, Graph graph, JsonSerializer serializer)
        {
            GraphInputNode inputNode = graph.GraphInputNode;
            if (inputNode == null)
            {
                return;
            }

            writer.WritePropertyName(INPUT_KEY);

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

            writer.WritePropertyName(OUTPUT_KEY);

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

        private void AddEdges(Node node, ref List<Edge> edges, ref HashSet<Node> handledNodes)
        {
            // Rule: The port1 must be Outport, and the port2 must be Inport

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
                        id1 = outport.node.id,
                        port1 = outport.name,
                        id2 = node.id,
                        port2 = inport.name,
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
