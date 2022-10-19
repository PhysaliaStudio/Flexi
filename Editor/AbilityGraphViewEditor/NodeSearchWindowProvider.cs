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
        private AbilityGraphView graphView;

        public void Initialize(AbilityGraphView graphView)
        {
            this.graphView = graphView;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Node")),
            };

            foreach (Assembly assembly in ReflectionUtilities.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Node)))
                    {
                        string typeNamespace = type.Namespace;
                        int index = entries.FindIndex(x => x.content.text == typeNamespace);
                        if (index == -1)
                        {
                            entries.Add(new SearchTreeGroupEntry(new GUIContent(typeNamespace)) { level = 1, userData = type });
                            index = entries.Count - 1;
                        }

                        entries.Insert(index + 1, new SearchTreeEntry(new GUIContent(type.Name)) { level = 2, userData = type });
                    }
                }
            }

            return entries;
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
