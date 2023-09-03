using System.Collections.Generic;
using Physalia.Flexi.GraphViewEditor;
using UnityEditor;
using UnityEngine;

namespace Physalia.Flexi.NodeSearcher
{
    public class NodeSearcher : EditorWindow
    {
        public class ResultItem
        {
            public string assetPath;
            public string name;
            public int groupIndex;
            public int graphIndex;
        }

        private readonly List<ResultItem> results = new();
        private readonly List<bool> isGraphChecked = new();

        private string nodeName;
        private Vector2 scrollPosition;

        private static GUIStyle buttonStyle;

        [MenuItem(EditorConst.MenuFolder + "Node Searcher", priority = 1001)]
        public static void Open()
        {
            NodeSearcher window = GetWindow<NodeSearcher>("Node Searcher");
            window.Show();
        }

        private void OnGUI()
        {
            buttonStyle ??= new GUIStyle("Button")
            {
                alignment = TextAnchor.MiddleLeft,
            };

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Keyword: ", GUILayout.Width(70f));
            nodeName = EditorGUILayout.TextField(nodeName);
            if (GUILayout.Button("Search", GUILayout.Width(100f)))
            {
                Clear();
                if (!string.IsNullOrEmpty(nodeName))
                {
                    SearchNodes();
                }
            }
            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    string buttonName = $"{results[i].name}";
                    if (GUILayout.Button(buttonName, buttonStyle, GUILayout.Width(150f)))
                    {
                        AbilityAsset abilityAsset = AssetDatabase.LoadAssetAtPath<AbilityAsset>(results[i].assetPath);
                        if (abilityAsset == null)
                        {
                            Debug.LogError($"Can't find asset at path: {results[i].assetPath}");
                            continue;
                        }

                        AbilityGraphEditorWindow.Open(abilityAsset, results[i].groupIndex, results[i].graphIndex);
                    }
                    GUILayout.Space(10f);
                    EditorGUILayout.LabelField($"{results[i].groupIndex}-{results[i].graphIndex}", GUILayout.Width(30f));
                    GUILayout.Space(10f);
                    isGraphChecked[i] = EditorGUILayout.Toggle(isGraphChecked[i]);
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void Clear()
        {
            results.Clear();
            isGraphChecked.Clear();
        }

        private void SearchNodes()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AbilityAsset)}");
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                AbilityAsset abilityAsset = AssetDatabase.LoadAssetAtPath<AbilityAsset>(assetPath);
                for (int j = 0; j < abilityAsset.GraphGroups.Count; j++)
                {
                    for (int k = 0; k < abilityAsset.GraphGroups[j].graphs.Count; k++)
                    {
                        if (abilityAsset.GraphGroups[j].graphs[k].Contains(nodeName))
                        {
                            results.Add(new ResultItem
                            {
                                assetPath = assetPath,
                                name = abilityAsset.name,
                                groupIndex = j,
                                graphIndex = k,
                            });
                            isGraphChecked.Add(false);
                        }
                    }
                }
            }
        }
    }
}
