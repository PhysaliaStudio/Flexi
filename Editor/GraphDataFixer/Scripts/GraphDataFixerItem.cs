using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphDataFixer
{
    public class GraphDataFixerItem : VisualElement
    {
        private readonly VisualTreeAsset itemAsset;

        private TextField originalField;
        private TextField modifiedField;

        public GraphDataFixerItem(VisualTreeAsset itemAsset)
        {
            this.itemAsset = itemAsset;
        }

        public void CreateGUI()
        {
            itemAsset.CloneTree(this);
            originalField = this.Q<TextField>("original");
            modifiedField = this.Q<TextField>("modified");
        }

        public void SetOriginal(string original)
        {
            originalField.SetValueWithoutNotify(original);
        }

        public void SetModified(string modified)
        {
            modifiedField.SetValueWithoutNotify(modified);
        }

        public string GetOriginal()
        {
            return originalField.value;
        }

        public string GetModified()
        {
            return modifiedField.value;
        }
    }
}
