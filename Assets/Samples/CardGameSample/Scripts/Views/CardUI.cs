using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Physalia.Flexi.Samples.CardGame
{
    public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<CardUI> Clicked;
        public event Action<CardUI> Hovered;
        public event Action<CardUI> Unhovered;

        [SerializeField]
        private Canvas cardCanvas;
        [SerializeField]
        private TMP_Text cardCost;
        [SerializeField]
        private TMP_Text cardName;
        [SerializeField]
        private TMP_Text cardText;

        [Space]
        [SerializeField]
        private Ease repositionEaseType = Ease.OutExpo;

        [Space]
        [SerializeField]
        private Transform targetingSplineStartTransform;

        private Card card;
        private bool interactable;
        private Sequence repositioningSequence;

        public Card Card => card;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (interactable)
            {
                Clicked?.Invoke(this);
                Debug.Log("Clicked", this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable)
            {
                eventData.Use();
                Hovered?.Invoke(this);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (interactable)
            {
                eventData.Use();
                Unhovered?.Invoke(this);
            }
        }

        public void SetContent(Card card)
        {
            this.card = card;
            cardCost.text = card.GetStat(StatId.COST).CurrentValue.ToString();
            cardName.text = card.Name;
            cardText.text = card.Text;
        }

        public void SetInteractable(bool interactable)
        {
            this.interactable = interactable;
        }

        public Tween DoLocalRepositioning(Vector3 position, float rotation, float scale, float duration)
        {
            if (repositioningSequence != null)
            {
                repositioningSequence.Kill(false);
            }

            repositioningSequence = DOTween.Sequence();
            repositioningSequence.Insert(0f, this.GetRectTransform().DOAnchorPos(position, duration, false).SetEase(repositionEaseType));
            repositioningSequence.Insert(0f, transform.DOLocalRotate(new Vector3(0f, 0f, rotation), duration, RotateMode.Fast).SetEase(repositionEaseType));
            repositioningSequence.Insert(0f, transform.DOScale(scale, duration).SetEase(repositionEaseType));
            repositioningSequence.OnComplete(() => SetInteractable(true));

            return repositioningSequence;
        }

        public Vector3 GetTargetingSplineStartWorldPosition()
        {
            return targetingSplineStartTransform.position;
        }
    }
}
