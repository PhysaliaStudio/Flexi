using UnityEngine;

namespace Physalia.Flexi.Samples
{
    public static class UIExtensions
    {
        public static RectTransform GetRectTransform(this Component component)
        {
            return component.transform as RectTransform;
        }

        public static float GetRectWidth(this Component component)
        {
            if (component.transform is RectTransform rectTransform)
            {
                return rectTransform.rect.width;
            }

            return 0f;
        }

        public static float GetRectHeight(this Component component)
        {
            if (component.transform is RectTransform rectTransform)
            {
                return rectTransform.rect.height;
            }

            return 0f;
        }

        public static Vector2 WorldToLocalPointInRectangle(this Camera camera, RectTransform rectTransform, Vector3 worldPoint)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, camera, out Vector2 localPoint);
            return localPoint;
        }
    }
}
