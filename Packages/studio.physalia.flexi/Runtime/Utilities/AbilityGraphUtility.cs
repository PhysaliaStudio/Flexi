using Newtonsoft.Json;

namespace Physalia.Flexi
{
    internal static class AbilityGraphUtility
    {
        public static AbilityGraph Deserialize(string graphName, string graphJson, MacroLibrary macroLibrary = null)
        {
            if (string.IsNullOrEmpty(graphJson))
            {
                return new AbilityGraph();
            }

            AbilityGraph graph = JsonConvert.DeserializeObject<AbilityGraph>(graphJson);
            macroLibrary?.SetUpMacroNodes(graph);

            // Log missing elements
            foreach (Node node in graph.Nodes)
            {
                if (node is MissingNode missingNode)
                {
                    Logger.Error($"[{nameof(FlexiCore)}] Detect a missing node! assetName: {graphName}, nodeType: {missingNode.TypeFullName}");
                    continue;
                }

                foreach (Port port in node.Ports)
                {
                    if (port is IIsMissing)
                    {
                        Logger.Error($"[{nameof(FlexiCore)}] Detect connection to a missing port! assetName: {graphName}, nodeType: {node.GetType().Name}, portName: {port.Name}");
                    }
                }
            }

            return graph;
        }

        public static string Serialize(AbilityGraph abilityGraph)
        {
            string json = JsonConvert.SerializeObject(abilityGraph);
            return json;
        }

        public static bool HasMissingElement(this AbilityGraph graph)
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
