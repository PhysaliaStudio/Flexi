using UnityEditor;
using UnityEngine;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public static class AssetVersionConverter
    {
        [MenuItem("Tools/Physalia/Convert Graph Assets Version")]
        private static void ConvertFromSelectedObjects()
        {
            var objects = Selection.objects;
            for (var i = 0; i < objects.Length; i++)
            {
                if (objects[i] is not MonoScript && objects[i] is TextAsset textAsset)
                {
                    string json = textAsset.text;
                    AbilityGraph graph = AbilityGraphUtility.Deserialize(textAsset.name, json);
                    if (graph != null)
                    {
                        ConvertFromTextAsset(textAsset);
                    }
                }

                if (objects[i] is AbilityGraphAsset graphAsset && objects[i] is not MacroGraphAsset)
                {
                    ConvertFromGraphAsset(graphAsset);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ConvertFromTextAsset(TextAsset textAsset)
        {
            string filePath = AssetDatabase.GetAssetPath(textAsset);
            string assetPath = filePath.Replace(".json", ".asset");

            var asset = ScriptableObject.CreateInstance<AbilityGraphAsset>();
            asset.Text = textAsset.text;
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        private static void ConvertFromGraphAsset(AbilityGraphAsset graphAsset)
        {
            AbilityGraph graph = AbilityGraphUtility.Deserialize(graphAsset.name, graphAsset.Text);
            AbilityAsset abilityAsset = ScriptableObject.CreateInstance<AbilityAsset>();
            abilityAsset.AddGraphJson(graphAsset.Text);
            for (var i = 0; i < graph.BlackboardVariables.Count; i++)
            {
                abilityAsset.AddBlackboardVariable(graph.BlackboardVariables[i]);
            }

            string assetPath = AssetDatabase.GetAssetPath(graphAsset);
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.CreateAsset(abilityAsset, assetPath);
        }
    }
}
