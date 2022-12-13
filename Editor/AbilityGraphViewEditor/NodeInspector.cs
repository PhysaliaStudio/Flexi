using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public class NodeInspector : VisualElement
    {
        private readonly AbilityGraphEditorWindow window;
        private readonly VisualTreeAsset uiAsset;
        private readonly VisualTreeAsset listViewItemAsset;

        private VisualElement dynamicInportGroup;
        private VisualElement dynamicOutportGroup;
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

            dynamicInportGroup = this.Query("inport-group");
            dynamicOutportGroup = this.Query("outport-group");

            dynamicInportListView = this.Query<DynamicPortListView>("inport-list-view");
            dynamicInportListView.SetUp(window, Direction.Input, listViewItemAsset);

            dynamicOutportListView = this.Query<DynamicPortListView>("outport-list-view");
            dynamicOutportListView.SetUp(window, Direction.Output, listViewItemAsset);
        }

        public void SetNodeView(NodeView nodeView)
        {
            currentNodeView = nodeView;
            dynamicInportListView.SetNodeView(nodeView);
            dynamicOutportListView.SetNodeView(nodeView);

            if (currentNodeView == null)
            {
                visible = false;
            }
            else
            {
                visible = true;
            }
        }
    }
}
