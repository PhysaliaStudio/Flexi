using System.Collections.Generic;
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
    [CustomEditor(typeof(AbilityAsset))]
    [CanEditMultipleObjects]
    public class AbilityAssetInspector : Editor
    {
        private static readonly int MAX_CHARACTERS = 7000;

        private GUIStyle textStyle;
        private AbilityAsset asset;

        private readonly List<GUIContent> cachedPreviews = new();
        private int foldoutIndex = -1;

        private void OnEnable()
        {
            asset = target as AbilityAsset;
            if (foldoutIndex > 0 && foldoutIndex <= asset.GraphGroups.Count)
            {
                AbilityGraphGroup group = asset.GraphGroups[foldoutIndex];
                CachePreviews(group);
            }
        }

        public override void OnInspectorGUI()
        {
            if (textStyle == null)
            {
                textStyle = "ScriptText";
            }

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            for (var i = 0; i < asset.GraphGroups.Count; i++)
            {
                // Foldout for each group
                AbilityGraphGroup group = asset.GraphGroups[i];
                bool isFoldout = EditorGUILayout.Foldout(foldoutIndex == i, $"Group {i}", true);
                if (isFoldout && foldoutIndex != i)
                {
                    foldoutIndex = i;
                    CachePreviews(group);
                }
                else if (!isFoldout && foldoutIndex == i)
                {
                    foldoutIndex = -1;
                    ClearPreviews();
                }

                if (isFoldout)
                {
                    for (var previewIndex = 0; previewIndex < cachedPreviews.Count; previewIndex++)
                    {
                        RenderPreview(cachedPreviews[previewIndex]);
                    }
                }
            }
        }

        private void RenderPreview(GUIContent content)
        {
            Rect rect = GUILayoutUtility.GetRect(content, textStyle);
            float addedWidth = rect.x;
            rect.x = 0;  // Force align to left
            rect.y += 1;
            rect.width += addedWidth + 5;
            GUI.Box(rect, content, textStyle);
        }

        private void ClearPreviews()
        {
            cachedPreviews.Clear();
        }

        private void CachePreviews(AbilityGraphGroup group)
        {
            cachedPreviews.Clear();

            if (targets.Length > 1)
            {
                var content = new GUIContent($"{targets.Length} Graph Asset");
                cachedPreviews.Add(content);
                return;
            }

            for (var i = 0; i < group.graphs.Count; i++)
            {
                GUIContent content = CreateJsonPreview(group.graphs[i]);
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
