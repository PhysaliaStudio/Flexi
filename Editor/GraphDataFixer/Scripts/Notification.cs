using UnityEditor;
using UnityEngine;

namespace Physalia.Flexi.GraphDataFixer
{
    internal class Notification
    {
        private readonly EditorWindow window;

        private Texture acceptTexture;

        internal Notification(EditorWindow window)
        {
            this.window = window;
        }

        internal void LoadResources()
        {
            acceptTexture = EditorGUIUtility.Load("Packages/studio.physalia.flexi/Editor/GraphDataFixer/EditorResources/accept.png") as Texture;
        }

        // Note: We need a space to seperate the text and the texture.

        internal void NotifyNoneSelected()
        {
            window.ShowNotification(new GUIContent(" You need to select some GraphAssets first."));
        }

        internal void NotifyPass()
        {
            window.ShowNotification(new GUIContent(" All selected GraphAssets passed the validation!", acceptTexture));
        }
    }
}
