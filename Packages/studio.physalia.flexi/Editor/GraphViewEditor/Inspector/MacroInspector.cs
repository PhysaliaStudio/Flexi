using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class MacroInspector : VisualElement
    {
        private readonly AbilityGraphEditorWindow window;
        private readonly VisualTreeAsset uiAsset;
        private readonly VisualTreeAsset listViewItemAsset;

        private DynamicPortListView inputPortListView;
        private DynamicPortListView outputPortListView;

        public MacroInspector(AbilityGraphEditorWindow window, VisualTreeAsset uiAsset, VisualTreeAsset listViewItemAsset) : base()
        {
            this.window = window;
            this.uiAsset = uiAsset;
            this.listViewItemAsset = listViewItemAsset;
            CreateGUI();
        }

        private void CreateGUI()
        {
            uiAsset.CloneTree(this);

            inputPortListView = this.Query<DynamicPortListView>("input-port-list-view");
            inputPortListView.SetUp(window, Direction.Output, listViewItemAsset);

            outputPortListView = this.Query<DynamicPortListView>("output-port-list-view");
            outputPortListView.SetUp(window, Direction.Input, listViewItemAsset);
        }

        public void SetMacroGraphView(AbilityGraphView graphView)
        {
            NodeView graphInputNodeView = graphView.GetNodeView(graphView.Data.GraphInputNode);
            inputPortListView.SetNodeView(graphInputNodeView);

            NodeView graphOutputNodeView = graphView.GetNodeView(graphView.Data.GraphOutputNode);
            outputPortListView.SetNodeView(graphOutputNodeView);
        }
    }
}
