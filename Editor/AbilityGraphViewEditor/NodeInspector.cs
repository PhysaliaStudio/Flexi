using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public class NodeInspector : VisualElement
    {
        private readonly AbilityGraphEditorWindow window;
        private readonly VisualTreeAsset uiAsset;
        private readonly VisualTreeAsset listViewItemAsset;

        private DynamicPortListView dynamicInportListView;
        private DynamicPortListView dynamicOutportListView;

        private NodeView currentNodeView;

        public NodeInspector(AbilityGraphEditorWindow window, VisualTreeAsset uiAsset, VisualTreeAsset listViewItemAsset) : base()
        {
            this.window = window;
            this.uiAsset = uiAsset;
            this.listViewItemAsset = listViewItemAsset;
            CreateGUI();
        }

        private void CreateGUI()
        {
            uiAsset.CloneTree(this);

            dynamicOutportListView = new DynamicPortListView(window, Direction.Output, listViewItemAsset);
            Add(dynamicOutportListView);
        }

        public void SetNodeView(NodeView nodeView)
        {
            currentNodeView = nodeView;
            dynamicOutportListView.SetNodeView(nodeView);
        }
    }
}
