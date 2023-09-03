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

        private Notification notification;

        private Button validateButton;
        private Button fixButton;
        private ReplacementArea replacementArea;
        private ScrollViewArea scrollViewArea;

        private ValidationResult result;

        [MenuItem(EditorConst.MenuFolder + "GraphData Fixer", priority = 1002)]
        private static void Open()
        {
            GraphDataFixerWindow window = GetWindow<GraphDataFixerWindow>("GraphData Fixer");
            window.Show();
        }

        private void OnEnable()
        {
            notification = new Notification(this);
            notification.LoadResources();
        }

        private void CreateGUI()
        {
            bool checkResult = CheckUiAssets();
            if (!checkResult)
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

            VisualElement replacementAreaElement = rootVisualElement.Q<VisualElement>("quick-replacement-area");
            replacementArea = new ReplacementArea(replacementAreaElement);
            replacementArea.replaceClicked += ReplaceTypeNames;

            VisualElement scrollViewAreaElement = rootVisualElement.Q<VisualElement>("scroll-view-area");
            scrollViewArea = new ScrollViewArea(itemAsset, scrollViewAreaElement);

            Clear();
        }

        private bool CheckUiAssets()
        {
            string folderPath = "Packages/studio.physalia.flexi/Editor/GraphDataFixer/EditorResources/";

            if (uiAsset == null)
            {
                uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "GraphDataFixerWindow.uxml");
                if (uiAsset == null)
                {
                    return false;
                }
            }

            if (itemAsset == null)
            {
                itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "GraphDataFixerItem.uxml");
                if (itemAsset == null)
                {
                    return false;
                }
            }

            return true;
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

            if (graphAssets.Count == 0)
            {
                notification.NotifyNoneSelected();
                return;
            }

            ValidateGraphAssets(graphAssets);
        }

        private void ValidateGraphAssets(List<GraphAsset> graphAssets)
        {
            if (graphAssets.Count == 0)
            {
                return;
            }

            result = GraphDataFixer.ValidateGraphAssets(graphAssets);
            scrollViewArea.ListInvalidTypeNames(result.invalidTypeNames);

            if (result.invalidTypeNames.Count > 0)
            {
                validateButton.SetEnabled(false);
                fixButton.SetEnabled(true);
                replacementArea.Show();
                scrollViewArea.Show();
            }
            else
            {
                validateButton.SetEnabled(true);
                fixButton.SetEnabled(false);
                notification.NotifyPass();
            }
        }

        private void ReplaceTypeNames()
        {
            string original = replacementArea.GetOriginal();
            string modified = replacementArea.GetModified();

            IReadOnlyList<GraphDataFixerItem> items = scrollViewArea.Items;
            for (var i = 0; i < items.Count; i++)
            {
                GraphDataFixerItem item = items[i];
                string newTypeName = item.GetOriginal().Replace(original, modified);
                item.SetModified(newTypeName);
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
            IReadOnlyList<GraphDataFixerItem> items = scrollViewArea.Items;
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
            replacementArea.Hide();
            scrollViewArea.Clear();
            scrollViewArea.Hide();
        }
    }
}
