using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilitySystem.GraphViewEditor
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

            AbilityGraph abilityGraph = null;
            try
            {
                abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json, SERIALIZER_SETTINGS);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Failed to parse JSON to AbilitySource");
                abilityGraph = null;
            }
            return abilityGraph;
        }

        public static void Write(string filePath, AbilityGraph abilityGraph)
        {
            string json = JsonConvert.SerializeObject(abilityGraph, SERIALIZER_SETTINGS);
            using var fs = new FileStream(filePath, FileMode.Create);
            using var writer = new StreamWriter(fs);
            writer.Write(json);
        }
    }
}
