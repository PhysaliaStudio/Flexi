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
            IReadOnlyList<string> macroAssetPaths = GetAllMacroAssetPaths();

            entries.Add(new SearchTreeGroupEntry(new GUIContent("Macros")) { level = 1 });
            for (var i = 0; i < macroAssetPaths.Count; i++)
            {
                string name = macroAssetPaths[i].Split('/')[^1];
                if (name.EndsWith(".asset"))
                {
                    name = name.Substring(0, name.Length - ".asset".Length);
                }

                entries.Add(new SearchTreeEntry(new GUIContent(name)) { level = 2, userData = macroAssetPaths[i] });
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

        private static IReadOnlyList<string> GetAllMacroAssetPaths()
        {
            string[] guids = AssetDatabase.FindAssets("t:MacroGraphAsset");
            var assetPaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return assetPaths;
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
            else if (searchTreeEntry.userData is string assetPath)
            {
                graphView.CreateMacroNode(assetPath, localMousePosition);
                window.SetDirty(true);
                return true;
            }

            return false;
        }
    }
}
