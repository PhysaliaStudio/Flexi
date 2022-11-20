using UnityEditor;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    /// <remarks>
    /// Largely reference TextAssetInspector.
    /// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/MonoScriptInspector.cs
    /// </remarks>
    [CustomEditor(typeof(AbilityGraphAsset))]
    [CanEditMultipleObjects]
    public class AbilityGraphAssetInspector : Editor
    {
        private static readonly int MAX_CHARACTERS = 7000;

        private GUIStyle textStyle;
        private AbilityGraphAsset asset;
        private string guid;

        private Hash128 lastDependencyHash;
        private GUIContent cachedPreview;

        private void OnEnable()
        {
            asset = target as AbilityGraphAsset;
            string assetPath = AssetDatabase.GetAssetPath(asset);
            guid = AssetDatabase.AssetPathToGUID(assetPath);
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

            Hash128 dependencyHash = AssetDatabase.GetAssetDependencyHash(guid);
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
            string text;
            if (targets.Length > 1)
            {
                text = $"{targets.Length} Graph Asset";
            }
            else
            {
                text = asset.Text;
                if (text.Length >= MAX_CHARACTERS)
                {
                    text = text.Substring(0, MAX_CHARACTERS) + "...\n\n<...etc...>";
                }
            }

            cachedPreview = new GUIContent(text);
        }
    }
}
