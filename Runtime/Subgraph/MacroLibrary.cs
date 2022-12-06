using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Physalia.AbilityFramework
{
    public class MacroLibrary : Dictionary<string, string>
    {
        public SubgraphNode AddMacroNode(Graph graph, string guid)
        {
            bool success = TryGetValue(guid, out string macroJson);
            if (!success)
            {
                Logger.Error($"[{nameof(MacroLibrary)}] Get macro failed! guid: {guid}");
                return null;
            }

            SubgraphNode node = graph.AddNewNode<SubgraphNode>();
            node.guid = guid;

            Graph macro = JsonConvert.DeserializeObject<Graph>(macroJson);

            var inputData = new GraphInputData(macro.GraphInputNode);
            for (var i = 0; i < inputData.portDatas.Count; i++)
            {
                PortData portData = inputData.portDatas[i];
                Type type = ReflectionUtilities.GetTypeByName(portData.type);
                PortFactory.CreateInport(node, type, portData.name);
            }

            var outputData = new GraphOutputData(macro.GraphOutputNode);
            for (var i = 0; i < outputData.portDatas.Count; i++)
            {
                PortData portData = outputData.portDatas[i];
                Type type = ReflectionUtilities.GetTypeByName(portData.type);
                PortFactory.CreateOutport(node, type, portData.name);
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
                    bool success = TryGetValue(macroNode.guid, out string macroJson);
                    if (!success)
                    {
                        Logger.Error($"[{nameof(MacroLibrary)}] Get macro failed! guid: {macroNode.guid}");
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

                Inport existedPort = node.GetInput(portData.name);
                if (existedPort != null)
                {
                    var portsCache = new List<Port>(existedPort.GetConnections());
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].Disconnect(existedPort);
                    }
                    node.RemoveInport(existedPort);

                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    PortFactory.CreateInport(node, type, portData.name);

                    Inport fixedPort = node.GetInput(portData.name);
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].Connect(fixedPort);
                    }
                }
                else
                {
                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    PortFactory.CreateInport(node, type, portData.name);
                }
            }

            var outputData = new GraphOutputData(macro.GraphOutputNode);
            for (var i = 0; i < outputData.portDatas.Count; i++)
            {
                PortData portData = outputData.portDatas[i];

                Outport existedPort = node.GetOutput(portData.name);
                if (existedPort != null)
                {
                    var portsCache = new List<Port>(existedPort.GetConnections());
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].Disconnect(existedPort);
                    }

                    node.RemoveOutport(existedPort);

                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    PortFactory.CreateOutport(node, type, portData.name);

                    Outport fixedPort = node.GetOutput(portData.name);
                    for (var j = 0; j < portsCache.Count; j++)
                    {
                        portsCache[j].Connect(fixedPort);
                    }
                }
                else
                {
                    Type type = ReflectionUtilities.GetTypeByName(portData.type);
                    PortFactory.CreateOutport(node, type, portData.name);
                }
            }
        }
    }
}
