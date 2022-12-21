using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    /// <remarks>
    /// Largely refer from TextAssetInspector.
    /// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/MonoScriptInspector.cs
    /// </remarks>
    [CustomEditor(typeof(AbilityAsset))]
    [CanEditMultipleObjects]
    public class AbilityAssetInspector : Editor
    {
        private static readonly int MAX_CHARACTERS = 7000;

        private GUIStyle textStyle;
        private AbilityAsset asset;
        private string assetPath;

        private readonly List<GUIContent> cachedPreviews = new();
        private Hash128 lastDependencyHash;

        private void OnEnable()
        {
            asset = target as AbilityAsset;
            assetPath = AssetDatabase.GetAssetPath(asset);
            CachePreviews();
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
                CachePreviews();
                lastDependencyHash = dependencyHash;
            }

            for (var i = 0; i < cachedPreviews.Count; i++)
            {
                RenderPreview(cachedPreviews[i]);
            }
        }

        private void RenderPreview(GUIContent content)
        {
            Rect rect = GUILayoutUtility.GetRect(content, textStyle);
            float addedWidth = rect.x;
            rect.x = 0;
            rect.y -= 3;
            rect.width += addedWidth + 5;
            GUI.Box(rect, content, textStyle);
        }

        private void CachePreviews()
        {
            cachedPreviews.Clear();

            if (targets.Length > 1)
            {
                var content = new GUIContent($"{targets.Length} Graph Asset");
                cachedPreviews.Add(content);
                return;
            }

            for (var i = 0; i < asset.GraphJsons.Count; i++)
            {
                GUIContent content = CreateJsonPreview(asset.GraphJsons[i]);
                cachedPreviews.Add(content);
            }
        }

        private static GUIContent CreateJsonPreview(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new GUIContent();
            }

            string text;
            try
            {
                text = IndentJson(json);
                if (text.Length >= MAX_CHARACTERS)
                {
                    text = text.Substring(0, MAX_CHARACTERS) + "...\n\n<...etc...>";
                }
            }
            catch
            {
                text = "[Error] Json Parse Failed!";
            }

            return new GUIContent(text);
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
