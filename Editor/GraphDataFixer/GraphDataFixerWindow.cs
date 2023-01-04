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
        private Button fixButton;
        private VisualElement scrollView;
        private readonly List<GraphDataFixerItem> items = new();

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

            fixButton = rootVisualElement.Q<Button>("fix-button");
            fixButton.clicked += Fix;

            Button clearButton = rootVisualElement.Q<Button>("clear-button");
            clearButton.clicked += Clear;

            scrollView = rootVisualElement.Q<ScrollView>();

            validateButton.SetEnabled(true);
            fixButton.SetEnabled(false);
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

            ValidateGraphAssets(graphAssets);
        }

        private void ValidateGraphAssets(List<GraphAsset> graphAssets)
        {
            result = GraphDataFixer.ValidateGraphAssets(graphAssets);
            ListInvalidTypeNames(result);

            if (result.invalidTypeNames.Count > 0)
            {
                validateButton.SetEnabled(false);
                fixButton.SetEnabled(true);
            }
            else
            {
                validateButton.SetEnabled(true);
                fixButton.SetEnabled(false);
            }
        }

        private void ListInvalidTypeNames(ValidationResult result)
        {
            for (var i = 0; i < result.invalidTypeNames.Count; i++)
            {
                GraphDataFixerItem item = new GraphDataFixerItem(itemAsset);
                item.CreateGUI();
                item.SetOriginal(result.invalidTypeNames[i]);
                scrollView.Add(item);
                items.Add(item);
            }
        }

        private void Fix()
        {
            if (result == null)
            {
                return;
            }

            List<GraphAsset> invalidAssets = result.invalidAssets;
            Dictionary<string, string> fixTable = BuildFixTable();
            GraphDataFixer.FixGraphAssets(result.invalidAssets, fixTable);

            for (var i = 0; i < invalidAssets.Count; i++)
            {
                EditorUtility.SetDirty(invalidAssets[i]);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Clear();
            ValidateGraphAssets(invalidAssets);
        }

        private Dictionary<string, string> BuildFixTable()
        {
            var table = new Dictionary<string, string>();
            for (var i = 0; i < items.Count; i++)
            {
                GraphDataFixerItem item = items[i];
                string origianl = item.GetOriginal();
                string modified = item.GetModified();
                if (string.IsNullOrEmpty(modified))
                {
                    continue;
                }

                table.Add(origianl, modified);
            }

            return table;
        }

        private void Clear()
        {
            result = null;
            validateButton.SetEnabled(true);
            fixButton.SetEnabled(false);
            scrollView.Clear();
            items.Clear();
        }
    }
}
