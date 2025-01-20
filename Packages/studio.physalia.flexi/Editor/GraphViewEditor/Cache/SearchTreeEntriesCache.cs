using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Physalia.Flexi.GraphViewEditor
{
    internal static class SearchTreeEntriesCache
    {
        internal class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
            {
                Rebuild(didDomainReload);
            }
        }

#if !SHOW_TEST_NODES
        private static readonly HashSet<string> TEST_ASSEMBLY_NAMES = new HashSet<string> {
            "Physalia.Flexi.Editor.Tests",
            "Physalia.Flexi.PerformanceTests",
        };
#endif

        private static Texture2D entryNodeMenuIcon;
        private static Texture2D flowNodeMenuIcon;
        private static Texture2D macroNodeMenuIcon;
        private static Texture2D otherNodeMenuIcon;
        private static readonly Dictionary<Color, Texture2D> customNodeMenuIcons = new();

        private static readonly List<SearchTreeEntry> searchTreeEntries = new();
        private static readonly List<SearchTreeEntry> nodeTreeEntries = new();
        private static readonly List<SearchTreeEntry> macroTreeEntries = new();

        internal static List<SearchTreeEntry> Get()
        {
            return searchTreeEntries;
        }

        private static void Rebuild(bool didDomainReload)
        {
            ReloadAllIcons();
            RebuildSearchTreeEntries(didDomainReload);
        }

        private static void ReloadAllIcons()
        {
            const string IconFolder = "Packages/studio.physalia.flexi/Editor/GraphViewEditor/Icons/";
            entryNodeMenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconFolder + "EntryNode_MenuIcon.png");
            flowNodeMenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconFolder + "FlowNode_MenuIcon.png");
            macroNodeMenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconFolder + "MacroNode_MenuIcon.png");
            otherNodeMenuIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconFolder + "OtherNode_MenuIcon.png");
        }

        private static void RebuildSearchTreeEntries(bool didDomainReload)
        {
            if (didDomainReload)
            {
                RebuildNodeTreeEntries();
            }
            RebuildMacroTreeEntries();

            searchTreeEntries.Clear();
            searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Node")) { level = 0 });
            searchTreeEntries.AddRange(nodeTreeEntries);
            searchTreeEntries.AddRange(macroTreeEntries);
        }

        private static void RebuildNodeTreeEntries()
        {
            nodeTreeEntries.Clear();

            IReadOnlyList<Type> nodeTypes = GetAllNodeTypes();
            NodeTypeSearchTree searchTree = CreateNodeTypeSearchTree(nodeTypes);

            IEnumerator<NodeTypeSearchTree.Node> enumerator = searchTree.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NodeTypeSearchTree.Node node = enumerator.Current;
                if (node.IsLeaf)
                {
                    Texture2D icon = GetNonMacroNodeIcon(node.Type);
                    nodeTreeEntries.Add(new SearchTreeEntry(new GUIContent(node.Text, icon)) { level = node.Level, userData = node.Type });
                }
                else
                {
                    nodeTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(node.Text)) { level = node.Level });
                }
            }
        }

        private static IReadOnlyList<Type> GetAllNodeTypes()
        {
            var nodeTypes = new List<Type>();
            foreach (Assembly assembly in ReflectionUtilities.GetAssemblies())
            {
#if !SHOW_TEST_NODES
                // Hide the test node types
                if (TEST_ASSEMBLY_NAMES.Contains(assembly.GetName().Name))
                {
                    continue;
                }
#endif

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Node)))
                    {
                        if (type.GetCustomAttribute<HideFromSearchWindow>() != null)
                        {
                            continue;
                        }

                        nodeTypes.Add(type);
                    }
                }
            }

            return nodeTypes;
        }

        private static NodeTypeSearchTree CreateNodeTypeSearchTree(IReadOnlyList<Type> nodeTypes)
        {
            var searchTree = new NodeTypeSearchTree();
            for (var i = 0; i < nodeTypes.Count; i++)
            {
                Type type = nodeTypes[i];
                NodeCategory nodeCategory = type.GetCustomAttribute<NodeCategory>();
                if (nodeCategory != null && !string.IsNullOrEmpty(nodeCategory.Menu))
                {
                    string nodeDisplayName = string.IsNullOrEmpty(nodeCategory.Name) ? type.Name : nodeCategory.Name;
                    var path = $"{nodeCategory.Menu}/{nodeDisplayName}";
                    searchTree.Insert(path, type, nodeCategory.Order);
                }
                else
                {
                    searchTree.Insert(type.Name, type, 0);
                }
            }

            return searchTree;
        }

        private static Texture2D GetNonMacroNodeIcon(Type nodeType)
        {
            NodeColor nodeColor = nodeType.GetCustomAttribute<NodeColor>();
            if (nodeColor != null && nodeColor.IsValid)
            {
                Texture2D customIcon = GetOrCreateCustomMenuIcon(nodeColor.Color);
                return customIcon;
            }

            if (typeof(EntryNode).IsAssignableFrom(nodeType))
            {
                return entryNodeMenuIcon;
            }

            if (typeof(FlowNode).IsAssignableFrom(nodeType))
            {
                return flowNodeMenuIcon;
            }

            return otherNodeMenuIcon;
        }

        private static Texture2D GetOrCreateCustomMenuIcon(Color color)
        {
            bool success = customNodeMenuIcons.TryGetValue(color, out Texture2D icon);
            if (success)
            {
                return icon;
            }

            // Create a 32x32 colored icon
            icon = new Texture2D(32, 32);
            for (var x = 0; x < icon.width; x++)
            {
                for (var y = 0; y < icon.height; y++)
                {
                    icon.SetPixel(x, y, color);
                }
            }
            icon.Apply();

            customNodeMenuIcons.Add(color, icon);
            return icon;
        }

        private static void RebuildMacroTreeEntries()
        {
            macroTreeEntries.Clear();

            macroTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Macros")) { level = 1 });

            Dictionary<string, string> macroKeyToNameTable = GetMacroKeyToNameTable();
            foreach (KeyValuePair<string, string> pair in macroKeyToNameTable)
            {
                string key = pair.Key;
                string name = pair.Value;
                macroTreeEntries.Add(new SearchTreeEntry(new GUIContent(name, macroNodeMenuIcon)) { level = 2, userData = key });
            }
        }


        private static Dictionary<string, string> GetMacroKeyToNameTable()
        {
            var macroKeyToNameTable = new Dictionary<string, string>();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(MacroAsset)}");
            var assetPaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                string name = assetPaths[i].Split('/')[^1];
                if (name.EndsWith(".asset"))
                {
                    name = name.Substring(0, name.Length - ".asset".Length);
                }

                string key = name;  // TODO
                macroKeyToNameTable.Add(key, name);
            }

            return macroKeyToNameTable;
        }
    }
}
