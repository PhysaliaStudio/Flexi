using System.Collections.Generic;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    [CreateAssetMenu(fileName = "NewStatList", menuName = "Flexi/Stat List Asset", order = 150)]
    public class StatDefinitionListAsset : ScriptableObject
    {
        public string namespaceName = "";
        public string scriptAssetPath = "";
        public List<StatDefinition> stats = new();

        /// <remarks>
        /// The file should be the exported file, or match the format
        /// </remarks>
        public static StatDefinitionListAsset CreateWithFile(string fileContents)
        {
            string[] lines = fileContents.Replace("\r\n", "\n").Split('\n');
            var stats = new List<StatDefinition>();
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "")
                {
                    continue;
                }

                StatDefinition definition = JsonUtility.FromJson<StatDefinition>(lines[i]);
                if (definition != null)
                {
                    stats.Add(definition);
                }
                else
                {
                    Logger.Error($"Parse line {i} failed!");
                }
            }

            var instance = CreateInstance<StatDefinitionListAsset>();
            instance.stats = stats;
            return instance;
        }

        public static StatDefinitionListAsset CreateWithList(List<StatDefinition> stats)
        {
            var instance = CreateInstance<StatDefinitionListAsset>();
            instance.stats = stats;
            return instance;
        }
    }
}
