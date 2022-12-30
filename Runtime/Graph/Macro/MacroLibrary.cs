using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Physalia.Flexi
{
    public class MacroLibrary : Dictionary<string, string>
    {
        public SubgraphNode AddMacroNode(Graph graph, string macroKey)
        {
            bool success = TryGetValue(macroKey, out string macroJson);
            if (!success)
            {
                Logger.Error($"[{nameof(MacroLibrary)}] Get macro failed! guid: {macroKey}");
                return null;
            }

            SubgraphNode node = graph.AddNewNode<SubgraphNode>();
            node.key = macroKey;

            Graph macro = JsonConvert.DeserializeObject<Graph>(macroJson);

            var inputData = new GraphInputData(macro.GraphInputNode);
            for (var i = 0; i < inputData.portDatas.Count; i++)
            {
                PortData portData = inputData.portDatas[i];
                Type type = ReflectionUtilities.GetTypeByName(portData.type);
                node.CreateInportWithArgumentType(type, portData.name, true);
            }

            var outputData = new GraphOutputData(macro.GraphOutputNode);
            for (var i = 0; i < outputData.portDatas.Count; i++)
            {
                PortData portData = outputData.portDatas[i];
                Type type = ReflectionUtilities.GetTypeByName(portData.type);
                node.CreateOutportWithArgumentType(type, portData.name, true);
            }

            return node;
        }

        public void SetUpMacroNodes(Graph graph)
        {
            IReadOnlyList<Node> nodes = graph.Nodes;
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is SubgraphNode macroNode)
                {
                    bool success = TryGetValue(macroNode.key, out string macroJson);
                    if (!success)
                    {
                        Logger.Error($"[{nameof(MacroLibrary)}] Get macro failed! guid: {macroNode.key}");
                        continue;
                    }

                    Graph macro = JsonConvert.DeserializeObject<Graph>(macroJson);
                    CreatePortsForMacroNode(macroNode, macro);
                }
            }
        }

        private void CreatePortsForMacroNode(Node node, Graph macro)
        {
            var inputData = new GraphInputData(macro.GraphInputNode);
            for (var i = 0; i < inputData.portDatas.Count; i++)
            {
                PortData portData = inputData.portDatas[i];

                Inport existedPort = node.GetInport(portData.name);
                if (existedPort != null)
                {
                    var portsCache = new List<Port>(existedPort.GetConnections());
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].Disconnect(existedPort);
                    }
                    node.RemoveInport(existedPort);

                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    node.CreateInportWithArgumentType(type, portData.name, true);

                    Inport fixedPort = node.GetInport(portData.name);
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].ConnectForce(fixedPort);
                    }
                }
                else
                {
                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    node.CreateInportWithArgumentType(type, portData.name, true);
                }
            }

            var outputData = new GraphOutputData(macro.GraphOutputNode);
            for (var i = 0; i < outputData.portDatas.Count; i++)
            {
                PortData portData = outputData.portDatas[i];

                Outport existedPort = node.GetOutport(portData.name);
                if (existedPort != null)
                {
                    var portsCache = new List<Port>(existedPort.GetConnections());
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].Disconnect(existedPort);
                    }

                    node.RemoveOutport(existedPort);

                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    node.CreateOutportWithArgumentType(type, portData.name);

                    Outport fixedPort = node.GetOutport(portData.name);
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].Connect(fixedPort);
                    }
                }
                else
                {
                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    node.CreateOutportWithArgumentType(type, portData.name);
                }
            }
        }
    }
}
