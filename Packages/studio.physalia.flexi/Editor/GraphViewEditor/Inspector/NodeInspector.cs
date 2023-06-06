using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class NodeInspector : VisualElement
    {
        private readonly AbilityGraphEditorWindow window;
        private readonly VisualTreeAsset uiAsset;
        private readonly VisualTreeAsset listViewItemAsset;

        private Label nodeNameLabel;
        private VisualElement dynamicInportGroup;
        private VisualElement dynamicOutportGroup;
        private DynamicPortListView dynamicInportListView;
        private DynamicPortListView dynamicOutportListView;

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

            nodeNameLabel = this.Query<Label>("node-name");

            dynamicInportGroup = this.Query("inport-group");
            dynamicOutportGroup = this.Query("outport-group");

            dynamicInportListView = this.Query<DynamicPortListView>("inport-list-view");
            dynamicInportListView.SetUp(window, Direction.Input, listViewItemAsset);

            dynamicOutportListView = this.Query<DynamicPortListView>("outport-list-view");
            dynamicOutportListView.SetUp(window, Direction.Output, listViewItemAsset);
        }

        public void SetNodeView(NodeView nodeView)
        {
            dynamicInportListView.SetNodeView(nodeView);
            dynamicOutportListView.SetNodeView(nodeView);

            if (nodeView == null)
            {
                visible = false;
                return;
            }

            visible = true;

            Node node = nodeView.NodeData;
            nodeNameLabel.text = node.GetType().Name;

            if (node is GraphInputNode)
            {
                if (dynamicInportGroup.parent == this)
                {
                    Remove(dynamicInportGroup);
                }

                if (dynamicOutportGroup.parent == null)
                {
                    Add(dynamicOutportGroup);
                }
            }
            else if (node is GraphOutputNode)
            {
                if (dynamicInportGroup.parent == null)
                {
                    Add(dynamicInportGroup);
                }

                if (dynamicOutportGroup.parent == this)
                {
                    Remove(dynamicOutportGroup);
                }
            }
            else
            {
                if (dynamicInportGroup.parent == this)
                {
                    Remove(dynamicInportGroup);
                }

                if (dynamicOutportGroup.parent == this)
                {
                    Remove(dynamicOutportGroup);
                }
            }
        }
    }
}
