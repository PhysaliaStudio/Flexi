using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
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
            return SearchTreeEntriesCache.Get();
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
            else if (searchTreeEntry.userData is string macroKey)
            {
                graphView.CreateMacroNode(MacroLibraryCache.Get(), macroKey, localMousePosition);
                window.SetDirty(true);
                return true;
            }

            return false;
        }
    }
}
