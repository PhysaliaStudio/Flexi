using UnityEditor;
using UnityEngine;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public static class AssetVersionConverter
    {
        [MenuItem("Tools/Physalia/Convert Graph Assets (From json TextAsset)")]
        private static void ConvertFromSelectedObjects()
        {
            var objects = Selection.objects;
            for (var i = 0; i < objects.Length; i++)
            {
                if (objects[i] is not MonoScript && objects[i] is TextAsset textAsset)
                {
                    string json = textAsset.text;
                    AbilityGraph graph = AbilityGraphEditorIO.Deserialize(json);
                    if (graph != null)
                    {
                        ConvertFromTextAsset(textAsset);
                    }
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
    }
}
