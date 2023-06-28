using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class AbilityFlowMenu : VisualElement
    {
        private readonly AbilityGraphEditorWindow editorWindow;
        private VisualElement elementParent;

        internal AbilityFlowMenu(AbilityGraphEditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;
        }

        internal void CreateGUI(VisualTreeAsset uiAsset)
        {
            uiAsset.CloneTree(this);

            Button buttonNewGroup = this.Q<Button>("button-new-group");
            buttonNewGroup.clicked += CreateNewGroup;

            elementParent = this.Q<VisualElement>("element-parent");
        }

        internal void Refresh(int currentGroupIndex, int currentGraphIndex, int[] buttonCountsPerGroup)
        {
            elementParent.Clear();

            for (var i = 0; i < buttonCountsPerGroup.Length; i++)
            {
                int groupIndex = i;
                var label = new Label($"Group {groupIndex}");
                label.AddToClassList("group-label");
                label.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {
                    evt.menu.AppendAction("Add Flow", action => { CreateNewGraph(groupIndex); });
                    evt.menu.AppendSeparator();
                    evt.menu.AppendAction("Delete Group", action => { DeleteGroup(groupIndex); });
                }));

                elementParent.Add(label);

                for (var j = 0; j < buttonCountsPerGroup[i]; j++)
                {
                    int graphIndex = j;

                    var button = new Button(() => SelectGraph(groupIndex, graphIndex)) { text = $"Flow {groupIndex}-{graphIndex}" };
                    button.AddToClassList("graph-button");
                    if (groupIndex == currentGroupIndex && graphIndex == currentGraphIndex)
                    {
                        button.AddToClassList("graph-button--selected");
                    }

                    button.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                    {
                        evt.menu.AppendAction("Delete", action => { DeleteGraph(groupIndex, graphIndex); });
                    }));

                    elementParent.Add(button);
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
