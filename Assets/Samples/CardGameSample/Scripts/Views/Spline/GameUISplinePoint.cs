using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameUISplinePoint : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        private Tween currentPunchTween;

        public void SetColor(Color color)
        {
            image.color = color;
        }

        public void SetScreenPosition(RectTransform parent, Vector2 screenPoint, Camera worldCamera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, screenPoint, worldCamera, out Vector3 position);
            this.GetRectTransform().position = position;
        }

        public void PulsePoint(float pulseSize)
        {
            if (currentPunchTween != null)
            {
                currentPunchTween.Kill(true);
            }

            Vector3 punch = new Vector3(pulseSize, pulseSize, pulseSize);
            currentPunchTween = transform.DOPunchScale(punch, 0.22f, 0, 0.2f);
        }
    }
}
