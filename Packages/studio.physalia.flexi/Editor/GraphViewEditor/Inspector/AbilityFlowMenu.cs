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
            buttonNewFlow.clicked += OnNewGraphButtonClicked;

            flowButtonParent = this.Q<VisualElement>("flow-button-parent");
        }

        private void OnNewGraphButtonClicked()
        {
            editorWindow.CreateNewGraph();
            Refresh();
        }

        internal void Refresh()
        {
            flowButtonParent.Clear();

            int graphCount = editorWindow.GraphCount;
            int currentGraphIndex = editorWindow.CurrentGraphIndex;
            for (var i = 0; i < graphCount; i++)
            {
                int index = i;
                var button = new Button(() => SelectGraph(index))
                {
                    name = "flow-button",
                    text = $"Flow {i}",
                };

                if (i == currentGraphIndex)
                {
                    button.AddToClassList("selected");
                }

                button.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {
                    evt.menu.AppendAction("Delete", action => { DeleteGraph(index); });
                }));

                flowButtonParent.Add(button);
            }
        }

        private void SelectGraph(int index)
        {
            editorWindow.SelectGraph(index);
        }

        private void DeleteGraph(int index)
        {
            editorWindow.DeleteGraph(index);
        }
    }
}
