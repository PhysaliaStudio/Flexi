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
            return graph;
        }

        internal static string Serialize(AbilityGraph abilityGraph)
        {
            string json = JsonConvert.SerializeObject(abilityGraph, SERIALIZER_SETTINGS);
            return json;
        }
    }
}
