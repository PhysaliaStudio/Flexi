using UnityEditor;
using UnityEngine;

namespace Physalia.Flexi
{
    public enum NotificationIcon
    {
        None,
        Accept,
    }

    public class Notification
    {
        private readonly EditorWindow window;

        private Texture acceptTexture;

        public Notification(EditorWindow window)
        {
            this.window = window;
        }

        public void LoadResources()
        {
            acceptTexture = EditorGUIUtility.Load("Packages/studio.physalia.flexi/Editor/Utilities/accept.png") as Texture;
        }

        public void Show(string text, NotificationIcon icon = NotificationIcon.None)
        {
            // Intentionally left a space before the text.
            window.ShowNotification(new GUIContent(" " + text, GetIconTexture(icon)));
        }

        private Texture GetIconTexture(NotificationIcon icon)
        {
            return icon switch
            {
                NotificationIcon.None => null,
                NotificationIcon.Accept => acceptTexture,
                _ => null,
            };
        }
    }
}
