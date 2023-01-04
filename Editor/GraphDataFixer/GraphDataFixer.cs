using System;
using System.Collections.Generic;
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
                string graphJson;
                if (assets[i] is MacroAsset macroAsset)
                {
                    graphJson = macroAsset.Text;
                }
                else if (assets[i] is AbilityAsset abilityAsset)
                {
                    graphJson = abilityAsset.GraphJsons[0];
                }
                else
                {
                    continue;
                }

                bool success = Validate(graphJson, ref result);
                if (!success)
                {
                    result.invalidAssets.Add(assets[i]);
                }
            }

            return result;
        }

        private static bool Validate(string graphJson, ref ValidationResult result)
        {
            JObject jObject = JObject.Parse(graphJson);
            JArray nodes = (JArray)jObject["nodes"];
            if (nodes == null)
            {
                return false;
            }

            var hasAnyNodeParsedFailed = false;
            var hasAnyInvalidType = false;

            for (var i = 0; i < nodes.Count; i++)
            {
                JToken typeToken = nodes[i]["_type"];
                if (typeToken == null)  // The '_type' field doesn't exist, which would not probably happened.
                {
                    hasAnyNodeParsedFailed = true;
                    continue;
                }

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

            return !hasAnyNodeParsedFailed && !hasAnyInvalidType;
        }
    }
}
