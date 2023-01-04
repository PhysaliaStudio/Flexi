using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphDataFixer
{
    public class GraphDataFixerWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset uiAsset;
        [SerializeField]
        private VisualTreeAsset itemAsset;

        private Button validateButton;
        private VisualElement scrollView;

        private ValidationResult result;

        [MenuItem("Tools/Flexi/GraphData Fixer")]
        private static void Open()
        {
            GraphDataFixerWindow window = GetWindow<GraphDataFixerWindow>("GraphData Fixer");
            window.Show();
        }

        private void CreateGUI()
        {
            if (uiAsset == null)
            {
                Logger.Error($"[{nameof(GraphDataFixerWindow)}] Missing UIAsset! Set the correct UIAsset in {nameof(GraphDataFixerWindow)} ScriptAsset might fix this.");
                return;
            }

            uiAsset.CloneTree(rootVisualElement);

            validateButton = rootVisualElement.Q<Button>("validate-button");
            validateButton.clicked += ValidateSelectedAssets;

            Button clearButton = rootVisualElement.Q<Button>("clear-button");
            clearButton.clicked += Clear;

            scrollView = rootVisualElement.Q<ScrollView>();
        }

        private void ValidateSelectedAssets()
        {
            var graphAssets = new List<GraphAsset>();

            Object[] objects = Selection.objects;
            for (var i = 0; i < objects.Length; i++)
            {
                if (objects[i] is GraphAsset graphAsset)
                {
                    graphAssets.Add(graphAsset);
                }
            }

            result = GraphDataFixer.ValidateGraphAssets(graphAssets);
            validateButton.SetEnabled(false);
            ListInvalidTypeNames(result);
        }

        private void ListInvalidTypeNames(ValidationResult result)
        {
            for (var i = 0; i < result.invalidTypeNames.Count; i++)
            {
                GraphDataFixerItem item = new GraphDataFixerItem(itemAsset);
                item.CreateGUI();
                item.SetOriginal(result.invalidTypeNames[i]);
                scrollView.Add(item);
            }
        }

        private void Clear()
        {
            result = null;
            validateButton.SetEnabled(true);
            scrollView.Clear();
        }
    }
}
