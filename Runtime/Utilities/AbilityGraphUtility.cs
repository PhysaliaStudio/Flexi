using Newtonsoft.Json;

namespace Physalia.Flexi
{
    internal static class AbilityGraphUtility
    {
        internal static AbilityGraph Deserialize(string graphName, string graphJson, MacroLibrary macroLibrary = null)
        {
            if (string.IsNullOrEmpty(graphJson))
            {
                return new AbilityGraph();
            }

            AbilityGraph graph = JsonConvert.DeserializeObject<AbilityGraph>(graphJson);
            if (macroLibrary != null)
            {
                macroLibrary.SetUpMacroNodes(graph);
            }

            // Log missing elements
            foreach (Node node in graph.Nodes)
            {
                if (node is MissingNode missingNode)
                {
                    Logger.Error($"[{nameof(AbilitySystem)}] Detect a missing node! assetName: {graphName}, nodeType: {missingNode.TypeFullName}");
                    continue;
                }

                foreach (Port port in node.Ports)
                {
                    if (port is IIsMissing)
                    {
                        Logger.Error($"[{nameof(AbilitySystem)}] Detect connection to a missing port! assetName: {graphName}, nodeType: {node.GetType().Name}, portName: {port.Name}");
                    }
                }
            }

            return graph;
        }

        internal static string Serialize(AbilityGraph abilityGraph)
        {
            string json = JsonConvert.SerializeObject(abilityGraph);
            return json;
        }

        internal static bool HasMissingElement(this AbilityGraph graph)
        {
            foreach (Node node in graph.Nodes)
            {
                if (node is MissingNode missingNode)
                {
                    return true;
                }

                foreach (Port port in node.Ports)
                {
                    if (port is IIsMissing)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
