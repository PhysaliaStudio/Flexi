using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class AbilityFlowMenu : VisualElement
    {
        private readonly AbilityGraphEditorWindow editorWindow;
        private VisualElement flowButtonParent;

        internal AbilityFlowMenu(AbilityGraphEditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;
        }

        internal void CreateGUI(VisualTreeAsset uiAsset)
        {
            uiAsset.CloneTree(this);

            Button buttonNewFlow = this.Q<Button>("button-new-flow");
            buttonNewFlow.clicked += CreateNewGroup;

            flowButtonParent = this.Q<VisualElement>("flow-button-parent");
        }

        internal void Refresh(int currentGroupIndex, int currentGraphIndex, int[] buttonCountsPerGroup)
        {
            flowButtonParent.Clear();

            for (var i = 0; i < buttonCountsPerGroup.Length; i++)
            {
                int groupIndex = i;
                var label = new Label($"Group {groupIndex}");
                label.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {
                    evt.menu.AppendAction("Add Flow", action => { CreateNewGraph(groupIndex); });
                    evt.menu.AppendSeparator();
                    evt.menu.AppendAction("Delete Group", action => { DeleteGroup(groupIndex); });
                }));
                flowButtonParent.Add(label);

                for (var j = 0; j < buttonCountsPerGroup[i]; j++)
                {
                    int graphIndex = j;
                    var button = new Button(() => SelectGraph(groupIndex, graphIndex))
                    {
                        name = "flow-button",
                        text = $"Flow {groupIndex}-{graphIndex}",
                    };

                    if (groupIndex == currentGroupIndex && graphIndex == currentGraphIndex)
                    {
                        button.AddToClassList("selected");
                    }

                    button.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                    {
                        evt.menu.AppendAction("Delete", action => { DeleteGraph(groupIndex, graphIndex); });
                    }));

                    flowButtonParent.Add(button);
                }
            }
        }

        private void CreateNewGroup()
        {
            editorWindow.CreateNewGroup();
        }

        private void DeleteGroup(int groupIndex)
        {
            editorWindow.DeleteGroup(groupIndex);
        }

        private void CreateNewGraph(int groupIndex)
        {
            editorWindow.CreateNewGraph(groupIndex);
        }

        private void SelectGraph(int groupIndex, int graphIndex)
        {
            editorWindow.SelectGraph(groupIndex, graphIndex);
        }

        private void DeleteGraph(int groupIndex, int graphIndex)
        {
            editorWindow.DeleteGraph(groupIndex, graphIndex);
        }
    }
}
