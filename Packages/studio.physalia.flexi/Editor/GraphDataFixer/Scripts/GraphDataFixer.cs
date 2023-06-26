using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Physalia.Flexi.GraphDataFixer
{
    public class ValidationResult
    {
        internal List<GraphAsset> invalidAssets = new();
        internal List<string> invalidTypeNames = new();
    }

    internal static class GraphDataFixer
    {
        internal static ValidationResult ValidateGraphAssets(List<GraphAsset> assets)
        {
            var result = new ValidationResult();

            for (var i = 0; i < assets.Count; i++)
            {
                if (assets[i] is MacroAsset macroAsset)
                {
                    bool success = Validate(macroAsset.Text, result);
                    if (!success && !result.invalidAssets.Contains(assets[i]))
                    {
                        result.invalidAssets.Add(assets[i]);
                    }
                }
                else if (assets[i] is AbilityAsset abilityAsset)
                {
                    for (var groupIndex = 0; groupIndex < abilityAsset.GraphGroups.Count; groupIndex++)
                    {
                        AbilityGraphGroup group = abilityAsset.GraphGroups[groupIndex];
                        for (var graphIndex = 0; graphIndex < group.graphs.Count; graphIndex++)
                        {
                            bool success = Validate(group.graphs[graphIndex], result);
                            if (!success && !result.invalidAssets.Contains(assets[i]))
                            {
                                result.invalidAssets.Add(assets[i]);
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            return result;
        }

        private static bool Validate(string graphJson, ValidationResult result)
        {
            JObject jObject = JObject.Parse(graphJson);
            var hasAnyInvalidType = false;

            IterateInputPorts(jObject, RecordInvalidType);
            IterateOutputPorts(jObject, RecordInvalidType);
            IterateNodes(jObject, RecordInvalidType);

            return !hasAnyInvalidType;

            void RecordInvalidType(JToken typeToken)
            {
                string typeName = typeToken.ToString();
                Type type = ReflectionUtilities.GetTypeByName(typeName);
                if (type == null)
                {
                    hasAnyInvalidType = true;
                    if (!result.invalidTypeNames.Contains(typeName))
                    {
                        result.invalidTypeNames.Add(typeName);
                    }
                }
            }
        }

        internal static void FixGraphAssets(List<GraphAsset> assets, Dictionary<string, string> fixTable)
        {
            for (var i = 0; i < assets.Count; i++)
            {
                if (assets[i] is MacroAsset macroAsset)
                {
                    macroAsset.Text = Fix(macroAsset.Text, fixTable);
                }
                else if (assets[i] is AbilityAsset abilityAsset)
                {
                    for (var groupIndex = 0; groupIndex < abilityAsset.GraphGroups.Count; groupIndex++)
                    {
                        AbilityGraphGroup group = abilityAsset.GraphGroups[groupIndex];
                        for (var graphIndex = 0; graphIndex < group.graphs.Count; graphIndex++)
                        {
                            group.graphs[graphIndex] = Fix(group.graphs[graphIndex], fixTable);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        private static string Fix(string graphJson, Dictionary<string, string> fixTable)
        {
            JObject jObject = JObject.Parse(graphJson);
            IterateInputPorts(jObject, FixInvalidType);
            IterateOutputPorts(jObject, FixInvalidType);
            IterateNodes(jObject, FixInvalidType);

            string result = jObject.ToString(Formatting.None);
            return result;

            void FixInvalidType(JToken typeToken)
            {
                string typeName = typeToken.ToString();
                Type type = ReflectionUtilities.GetTypeByName(typeName);
                if (type == null)
                {
                    bool success = fixTable.TryGetValue(typeName, out string newName);
                    if (success)
                    {
                        typeToken.Replace(newName);
                    }
                }
            }
        }

        private static void IterateInputPorts(JObject jObject, Action<JToken> actionForTypeToken)
        {
            JToken input = jObject[TokenKeys.GRAPH_INPUT];
            if (input == null)
            {
                return;
            }

            JArray portDatas = (JArray)input[nameof(GraphInputData.portDatas)];
            if (portDatas == null)  // The ports field doesn't exist, which would not probably happened.
            {
                return;
            }

            for (var i = 0; i < portDatas.Count; i++)
            {
                JToken typeToken = portDatas[i][nameof(PortData.type)];
                if (typeToken == null)  // The type field doesn't exist, which would not probably happened.
                {
                    continue;
                }

                actionForTypeToken?.Invoke(typeToken);
            }
        }

        private static void IterateOutputPorts(JObject jObject, Action<JToken> actionForTypeToken)
        {
            JToken output = jObject[TokenKeys.GRAPH_OUTPUT];
            if (output == null)
            {
                return;
            }

            JArray portDatas = (JArray)output[nameof(GraphInputData.portDatas)];
            if (portDatas == null)  // The ports field doesn't exist, which would not probably happened.
            {
                return;
            }

            for (var i = 0; i < portDatas.Count; i++)
            {
                JToken typeToken = portDatas[i][nameof(PortData.type)];
                if (typeToken == null)  // The type field doesn't exist, which would not probably happened.
                {
                    continue;
                }

                actionForTypeToken?.Invoke(typeToken);
            }
        }

        private static void IterateNodes(JObject jObject, Action<JToken> actionForTypeToken)
        {
            JArray nodes = (JArray)jObject[TokenKeys.GRAPH_NODES];
            if (nodes == null)
            {
                return;
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                JToken typeToken = nodes[i][TokenKeys.NODE_TYPE];
                if (typeToken == null)  // The type field doesn't exist, which would not probably happened.
                {
                    continue;
                }

                actionForTypeToken?.Invoke(typeToken);
            }
        }
    }
}
