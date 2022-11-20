using System;
using System.IO;
using Newtonsoft.Json;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public static class AbilityGraphEditorIO
    {
        private static readonly JsonSerializerSettings SERIALIZER_SETTINGS = new()
        {
            Formatting = Formatting.Indented,
        };

        public static AbilityGraph Read(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate);
            using var reader = new StreamReader(fs);
            string json = reader.ReadToEnd();

            AbilityGraph abilityGraph = Deserialize(json);
            return abilityGraph;
        }

        public static void Write(string filePath, AbilityGraph abilityGraph)
        {
            string json = Serialize(abilityGraph);
            using var fs = new FileStream(filePath, FileMode.Create);
            using var writer = new StreamWriter(fs);
            writer.Write(json);
        }

        public static AbilityGraph Deserialize(string json)
        {
            AbilityGraph abilityGraph;
            try
            {
                abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json, SERIALIZER_SETTINGS);
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                Logger.Error("Failed to parse JSON to AbilitySource");
                abilityGraph = null;
            }
            return abilityGraph;
        }

        public static string Serialize(AbilityGraph abilityGraph)
        {
            string json = JsonConvert.SerializeObject(abilityGraph, SERIALIZER_SETTINGS);
            return json;
        }
    }
}
