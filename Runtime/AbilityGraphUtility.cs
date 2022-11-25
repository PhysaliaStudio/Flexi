using Newtonsoft.Json;

namespace Physalia.AbilityFramework
{
    internal static class AbilityGraphUtility
    {
        private static readonly JsonSerializerSettings SERIALIZER_SETTINGS = new()
        {
            Formatting = Formatting.Indented,
        };

        internal static AbilityGraph Deserialize(string graphName, string graphJson)
        {
            AbilityGraph graph = JsonConvert.DeserializeObject<AbilityGraph>(graphJson);

            // Log missing elements
            foreach (Node node in graph.Nodes)
            {
                if (node is MissingNode missingNode)
                {
                    Logger.Error($"[{nameof(AbilitySystem)}] Detect a missing node! assetName: {graphName}, nodeType: {missingNode.TypeName}");
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
            string json = JsonConvert.SerializeObject(abilityGraph, SERIALIZER_SETTINGS);
            return json;
        }
    }
}
