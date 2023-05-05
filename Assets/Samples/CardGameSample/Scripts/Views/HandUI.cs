using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class HandUI : MonoBehaviour
    {
        public event Action<Card> CardSelected;
        public event Action<Unit> TargetSelected;
        public event Action SelectionCanceled;

        [SerializeField]
        private CardUI cardUIPrefab;
        [SerializeField]
        private Transform cardsRoot;
        [SerializeField]
        private float cardGap;
        [SerializeField]
        private SelectionBehaviour selectionBehaviour;

        [Space]
        [SerializeField]
        private float shiftXAmountByFocusedCard = 20f;
        [SerializeField]
        private float shiftXFalloffByFocusedCard = 10f;
        [SerializeField]
        private float scaleOfFocusedCard = 1.5f;

        [Space]
        [SerializeField]
        private int drawCardDelayMilliseconds = 250;

        [Space]
        [SerializeField]
        private Transform resolveFieldRoot;
        [SerializeField]
        private Transform discardRoot;
        [SerializeField]
        private TMP_Text discardCountText;

        [SerializeField]
        private float playFromHandRepositionDuration = 0.25f;
        [SerializeField]
        private float playToDiscardRepositionDuration = 0.1f;

        private float cardWidth;

        private readonly List<CardUI> cardUIs = new();
        private readonly Dictionary<Card, CardUI> modelToViewTable = new();
        private readonly List<CardUI> cardUIsInResolveField = new();

        private int indexOfFocusedCardUI = -1;
        private CardUI selectedCardUI;
        private int discardCount = 0;

        [ContextMenu("Add Card")]
        private void AddFakeCardUI()
        {
            AddCard(null);
        }

        [ContextMenu("Remove Card")]
        private void RemoveOneCardUI()
        {
            RemoveCard(cardUIs.Count - 1);
        }

        private void Start()
        {
            cardWidth = cardUIPrefab.GetRectWidth();
            selectionBehaviour.TargetSelected += OnTargetSelected;
            selectionBehaviour.Canceled += OnSelectionCanceled;
        }

        public async UniTask DrawCardsAnimation(IReadOnlyList<Card> cards)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                AddCard(cards[i]);
                await UniTask.Delay(drawCardDelayMilliseconds);
            }
        }

        public async UniTask DiscardCardsAnimation(IReadOnlyList<Card> cards)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                await DiscardCard(cards[i]);
            }
        }

        public async UniTask DiscardCard(Card card)
        {
            bool success = modelToViewTable.TryGetValue(card, out CardUI cardUI);
            if (success)
            {
                cardUI.Clicked -= SelectCard;
                cardUI.Hovered -= ShowFocusedCard;
                cardUI.Unhovered -= HideFocusedCard;

                cardUIs.Remove(cardUI);
                modelToViewTable.Remove(card);
                cardUI.transform.SetParent(discardRoot, true);
                await cardUI.DoLocalRepositioning(new Vector3(0f, 0f, 0f), 0f, 0.1f, playToDiscardRepositionDuration);

                discardCount++;
                discardCountText.text = discardCount.ToString();
                Destroy(cardUI.gameObject);
            }
        }

        public async UniTask PlayCardAnimation(Card card)
        {
            bool success = modelToViewTable.TryGetValue(card, out CardUI cardUI);
            if (success)
            {
                cardUI.Clicked -= SelectCard;
                cardUI.Hovered -= ShowFocusedCard;
                cardUI.Unhovered -= HideFocusedCard;

                HideFocusedCard(cardUI);

                cardUIs.Remove(cardUI);
                cardUIsInResolveField.Add(cardUI);
                cardUI.transform.SetParent(resolveFieldRoot, true);

                RepositionCards();
                await cardUI.DoLocalRepositioning(new Vector3(0f, 0f, 0f), 0f, 1f, playFromHandRepositionDuration);
                await UniTask.Delay(500);

                modelToViewTable.Remove(card);
                cardUIsInResolveField.Remove(cardUI);
                cardUI.transform.SetParent(discardRoot, true);
                await cardUI.DoLocalRepositioning(new Vector3(0f, 0f, 0f), 0f, 0.1f, playToDiscardRepositionDuration);

                discardCount++;
                discardCountText.text = discardCount.ToString();
                Destroy(cardUI.gameObject);
            }
        }

        public async UniTask ReturnFromDiscardPileAnimation()
        {
            discardCount = 0;
            discardCountText.text = discardCount.ToString();
            await UniTask.Delay(500);
        }

        private void ShowFocusedCard(CardUI cardUI)
        {
            indexOfFocusedCardUI = cardUIs.IndexOf(cardUI);
            RepositionCards();
        }

        private void HideFocusedCard(CardUI cardUI)
        {
            int index = cardUIs.IndexOf(cardUI);
            if (indexOfFocusedCardUI == index)
            {
                indexOfFocusedCardUI = -1;
                RepositionCards();
            }
        }

        private void SelectCard(CardUI cardUI)
        {
            if (cardUI.Card != null)
            {
                CardSelected?.Invoke(cardUI.Card);
            }
        }

        private void ClearSelection()
        {
            selectedCardUI = null;
            RepositionCards();
        }

        public void SetUpCardSelection(Card card, SelectionData selectionData)
        {
            selectedCardUI = modelToViewTable[card];
            selectionBehaviour.SetupTargetingCursor(selectedCardUI, selectionData);

            for (var i = 0; i < cardUIs.Count; i++)
            {
                cardUIs[i].SetInteractable(false);
            }
        }

        private void OnTargetSelected(Unit unit)
        {
            ClearSelection();
            TargetSelected?.Invoke(unit);
        }

        private void OnSelectionCanceled()
        {
            ClearSelection();
            SelectionCanceled?.Invoke();
        }

        private void RepositionCards()
        {
            int numberOfCards = cardUIs.Count;
            for (var i = 0; i < numberOfCards; i++)
            {
                CardUI cardUI = cardUIs[i];
                Vector3 position = CalculateCardPosition(numberOfCards, i);
                float positionShiftX = CalculateCardPositionShift(i);
                position.x += positionShiftX;

                var scale = 1f;
                if (i == indexOfFocusedCardUI)
                {
                    position.y += cardUI.GetRectHeight() * scaleOfFocusedCard * 0.5f;
                    scale = scaleOfFocusedCard;
                }

                _ = cardUI.DoLocalRepositioning(position, 0f, scale, playFromHandRepositionDuration);
            }
        }

        private Vector3 CalculateCardPosition(int numberOfCards, int cardIndex)
        {
            float totalWidth = cardWidth * numberOfCards + cardGap * (numberOfCards - 1);
            float leftMost = -totalWidth * 0.5f;
            float x = leftMost + cardWidth * (cardIndex + 0.5f) + cardGap * (cardIndex - 1);
            return new Vector3(x, 0f, 0f);
        }

        private float CalculateCardPositionShift(int cardIndex)
        {
            if (indexOfFocusedCardUI == -1)
            {
                return 0f;
            }

            if (cardIndex == indexOfFocusedCardUI)
            {
                return 0f;
            }

            int offset = cardIndex - indexOfFocusedCardUI;
            var minus = false;
            if (offset < 0)
            {
                offset = -offset;
                minus = true;
            }

            float shift = shiftXAmountByFocusedCard - shiftXFalloffByFocusedCard * (offset - 1);
            if (shift < 0f)
            {
                return 0f;
            }

            if (minus)
            {
                shift = -shift;
            }
            return shift;
        }

        public void AddCards(IReadOnlyList<Card> cards)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                AddCard(cards[i]);
            }
        }

        public void AddCard(Card card)
        {
            CardUI cardUI = Instantiate(cardUIPrefab, cardsRoot);
            cardUIs.Add(cardUI);

            if (card != null)
            {
                cardUI.Clicked += SelectCard;
                cardUI.Hovered += ShowFocusedCard;
                cardUI.Unhovered += HideFocusedCard;

                cardUI.SetContent(card);
                modelToViewTable.Add(card, cardUI);
            }

            RepositionCards();
        }

        public void RemoveCard(Card card)
        {
            bool success = modelToViewTable.TryGetValue(card, out CardUI cardUI);
            if (success)
            {
                cardUI.Clicked -= SelectCard;
                cardUI.Hovered -= ShowFocusedCard;
                cardUI.Unhovered -= HideFocusedCard;

                cardUIs.Remove(cardUI);
                modelToViewTable.Remove(card);
                Destroy(cardUI.gameObject);
            }

            RepositionCards();
        }

        public void RemoveCard(int index)
        {
            CardUI cardUI = cardUIs[index];
            cardUIs.Remove(cardUI);
            if (cardUI.Card != null)
            {
                modelToViewTable.Remove(cardUI.Card);
            }

            if (indexOfFocusedCardUI == index)
            {
                indexOfFocusedCardUI = -1;
            }

            Destroy(cardUI.gameObject);
            RepositionCards();
        }

        public void RemoveAll()
        {
            for (var i = 0; i < cardUIs.Count; i++)
            {
                Destroy(cardUIs[i].gameObject);
            }

            cardUIs.Clear();
            modelToViewTable.Clear();
        }
    }
}
