using System;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphDataFixer
{
    internal class ReplacementArea
    {
        internal event Action replaceClicked;

        private readonly VisualElement rootElement;
        private readonly TextField originalField;
        private readonly TextField modifiedField;

        internal ReplacementArea(VisualElement rootElement)
        {
            this.rootElement = rootElement;
            originalField = rootElement.Q<TextField>("original");
            modifiedField = rootElement.Q<TextField>("modified");

            Button replaceButton = rootElement.Q<Button>("replace-button");
            replaceButton.clicked += () => replaceClicked?.Invoke();
        }

        internal void Show()
        {
            rootElement.visible = true;
        }

        internal void Hide()
        {
            rootElement.visible = false;
        }

        internal string GetOriginal()
        {
            return originalField.value;
        }

        internal string GetModified()
        {
            return modifiedField.value;
        }
    }
}
