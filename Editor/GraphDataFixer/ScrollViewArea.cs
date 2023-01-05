using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphDataFixer
{
    internal class ScrollViewArea
    {
        private readonly VisualTreeAsset itemAsset;

        private readonly VisualElement rootElement;
        private readonly VisualElement scrollView;
        private readonly List<GraphDataFixerItem> items = new();

        internal IReadOnlyList<GraphDataFixerItem> Items => items;

        internal ScrollViewArea(VisualTreeAsset itemAsset, VisualElement rootElement)
        {
            this.itemAsset = itemAsset;

            this.rootElement = rootElement;
            scrollView = rootElement.Q<ScrollView>();
        }

        internal void Show()
        {
            rootElement.visible = true;
        }

        internal void Hide()
        {
            rootElement.visible = false;
        }

        internal void ListInvalidTypeNames(IReadOnlyList<string> typeNames)
        {
            for (var i = 0; i < typeNames.Count; i++)
            {
                GraphDataFixerItem item = new GraphDataFixerItem(itemAsset);
                item.CreateGUI();
                item.SetOriginal(typeNames[i]);
                scrollView.Add(item);
                items.Add(item);
            }
        }

        internal void Clear()
        {
            scrollView.Clear();
            items.Clear();
        }
    }
}
