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
        private static readonly Type MISSING_NODE_TYPE = typeof(UndefinedNode);

        private AbilityGraphView graphView;
        private List<SearchTreeEntry> searchTreeEntries;

        public void Initialize(AbilityGraphView graphView)
        {
            this.graphView = graphView;
            searchTreeEntries = CreateSearchTreeEntries();
        }

        private static List<SearchTreeEntry> CreateSearchTreeEntries()
        {
            IReadOnlyList<Type> nodeTypes = GetAllNodeTypes();
            NodeTypeSearchTree searchTree = CreateNodeTypeSearchTree(nodeTypes);

            var entries = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Node")) };
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

            return entries;
        }

        private static IReadOnlyList<Type> GetAllNodeTypes()
        {
            var nodeTypes = new List<Type>();
            foreach (Assembly assembly in ReflectionUtilities.GetAssemblies())
            {
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

            var type = searchTreeEntry.userData as Type;
            graphView.CreateNewNode(type, localMousePosition);
            return true;
        }
    }
}
