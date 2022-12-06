using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public class NodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private static readonly string TEST_ASSEMBLY_NAME = "Physalia.AbilityFramework.Editor.Tests";
        private static readonly Type MISSING_NODE_TYPE = typeof(MissingNode);

        private AbilityGraphView graphView;
        private List<SearchTreeEntry> searchTreeEntries;
        private readonly MacroLibrary macroLibrary = new();

        public void Initialize(AbilityGraphView graphView)
        {
            this.graphView = graphView;
            searchTreeEntries = CreateSearchTreeEntries();
        }

        private static List<SearchTreeEntry> CreateSearchTreeEntries()
        {
            // Title
            var entries = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Node")) { level = 0 } };

            // Nodes
            IReadOnlyList<Type> nodeTypes = GetAllNodeTypes();
            NodeTypeSearchTree searchTree = CreateNodeTypeSearchTree(nodeTypes);

            IEnumerator<NodeTypeSearchTree.Node> enumerator = searchTree.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NodeTypeSearchTree.Node node = enumerator.Current;
                if (node.IsLeaf)
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(node.Text)) { level = node.Level, userData = node.Type });
                }
                else
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(node.Text)) { level = node.Level });
                }
            }

            // Macros
            Dictionary<string, string> macroTable = GetMacroTable();

            entries.Add(new SearchTreeGroupEntry(new GUIContent("Macros")) { level = 1 });
            foreach (KeyValuePair<string, string> pair in macroTable)
            {
                string name = pair.Key;
                string guid = pair.Value;
                entries.Add(new SearchTreeEntry(new GUIContent(name)) { level = 2, userData = guid });
            }

            return entries;
        }

        private static IReadOnlyList<Type> GetAllNodeTypes()
        {
            var nodeTypes = new List<Type>();
            foreach (Assembly assembly in ReflectionUtilities.GetAssemblies())
            {
#if !SHOW_TEST_NODES
                // Hide the test node types
                if (assembly.GetName().Name == TEST_ASSEMBLY_NAME)
                {
                    continue;
                }
#endif

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Node)))
                    {
                        // Hide the internal node types
                        if (type == MISSING_NODE_TYPE)
                        {
                            continue;
                        }

                        nodeTypes.Add(type);
                    }
                }
            }

            return nodeTypes;
        }

        private static Dictionary<string, string> GetMacroTable()
        {
            var macroNameToGuidTable = new Dictionary<string, string>();

            string[] guids = AssetDatabase.FindAssets("t:MacroGraphAsset");
            var assetPaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guid);
                string name = assetPaths[i].Split('/')[^1];
                if (name.EndsWith(".asset"))
                {
                    name = name.Substring(0, name.Length - ".asset".Length);
                }

                macroNameToGuidTable.Add(name, guid);
            }

            return macroNameToGuidTable;
        }

        private static NodeTypeSearchTree CreateNodeTypeSearchTree(IReadOnlyList<Type> nodeTypes)
        {
            var searchTree = new NodeTypeSearchTree();
            for (var i = 0; i < nodeTypes.Count; i++)
            {
                Type type = nodeTypes[i];
                NodeCategory nodeCategory = type.GetCustomAttribute<NodeCategory>();
                if (nodeCategory != null)
                {
                    var path = $"{nodeCategory.Name}/{type.Name}";
                    searchTree.Insert(path, type);
                }
                else
                {
                    searchTree.Insert(type.Name, type);
                }
            }

            return searchTree;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            AbilityGraphEditorWindow window = EditorWindow.GetWindow<AbilityGraphEditorWindow>();
            Vector2 worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
            Vector2 localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);

            if (searchTreeEntry.userData is Type type)
            {
                graphView.CreateNewNode(type, localMousePosition);
                window.SetDirty(true);
                return true;
            }
            else if (searchTreeEntry.userData is string guid)
            {
                // Always replace the data, in case users' edit from external.
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                MacroGraphAsset macroGraphAsset = AssetDatabase.LoadAssetAtPath<MacroGraphAsset>(assetPath);
                if (macroGraphAsset == null)
                {
                    Debug.LogError($"Failed to load MacroGraphAsset at {assetPath}");
                    return false;
                }

                if (macroLibrary.ContainsKey(guid))
                {
                    macroLibrary[guid] = macroGraphAsset.Text;
                }
                else
                {
                    macroLibrary.Add(guid, macroGraphAsset.Text);
                }

                graphView.CreateMacroNode(macroLibrary, guid, localMousePosition);
                window.SetDirty(true);
                return true;
            }

            return false;
        }
    }
}
