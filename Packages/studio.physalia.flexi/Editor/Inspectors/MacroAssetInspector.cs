using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Physalia.Flexi
{
    /// <remarks>
    /// Largely refer from TextAssetInspector.
    /// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/MonoScriptInspector.cs
    /// </remarks>
    [CustomEditor(typeof(MacroAsset))]
    [CanEditMultipleObjects]
    public class MacroAssetInspector : Editor
    {
        private static readonly int MAX_CHARACTERS = 7000;

        private GUIStyle textStyle;
        private MacroAsset asset;
        private string assetPath;

        private Hash128 lastDependencyHash;
        private GUIContent cachedPreview;

        private void OnEnable()
        {
            asset = target as MacroAsset;
            assetPath = AssetDatabase.GetAssetPath(asset);
            CachePreview();
        }

        public override void OnInspectorGUI()
        {
            if (textStyle == null)
            {
                textStyle = "ScriptText";
            }

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            Hash128 dependencyHash = AssetDatabase.GetAssetDependencyHash(assetPath);
            if (lastDependencyHash != dependencyHash)
            {
                CachePreview();
                lastDependencyHash = dependencyHash;
            }

            Rect rect = GUILayoutUtility.GetRect(cachedPreview, textStyle);
            float addedWidth = rect.x;
            rect.x = 0;
            rect.y -= 3;
            rect.width += addedWidth + 5;
            GUI.Box(rect, cachedPreview, textStyle);
        }

        private void CachePreview()
        {
            if (targets.Length > 1)
            {
                cachedPreview = new GUIContent($"{targets.Length} Graph Asset");
                return;
            }

            if (string.IsNullOrEmpty(asset.Text))
            {
                cachedPreview = new GUIContent();
                return;
            }

            string text;
            try
            {
                text = IndentJson(asset.Text);
                if (text.Length >= MAX_CHARACTERS)
                {
                    text = text.Substring(0, MAX_CHARACTERS) + "...\n\n<...etc...>";
                }
            }
            catch
            {
                text = "[Error] Json Parse Failed!";
            }

            cachedPreview = new GUIContent(text);
        }

        private static string IndentJson(string json)
        {
            using var sr = new StringReader(json);
            using var sw = new StringWriter();
            var jtr = new JsonTextReader(sr);
            var jtw = new JsonTextWriter(sw) { Formatting = Formatting.Indented };
            jtw.WriteToken(jtr);
            return sw.ToString();
        }
    }
}
