using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class PlayArea
    {
        private readonly List<Card> hand = new();
        private readonly List<Card> drawPile = new();
        private readonly List<Card> discardPile = new();
        private readonly List<Card> removedPile = new();

        private readonly System.Random random = new();

        public IReadOnlyList<Card> Hand => hand;
        public IReadOnlyList<Card> DrawPile => drawPile;
        public IReadOnlyList<Card> DiscardPile => discardPile;
        public IReadOnlyList<Card> RemovedPile => removedPile;

        public PlayArea(IReadOnlyList<Card> startCards)
        {
            drawPile.AddRange(startCards);
        }

        public void ShuffleDrawPile()
        {
            drawPile.Shuffle(random);
        }

        public List<Card> Draw(int amount)
        {
            List<Card> cache = new List<Card>();
            if (drawPile.Count == 0)
            {
                return cache;
            }

            int actualDraw = Mathf.Min(drawPile.Count, amount);
            for (var i = 0; i < actualDraw; i++)
            {
                Card card = drawPile[drawPile.Count - i - 1];
                hand.Insert(0, card);
                cache.Add(card);
            }
            drawPile.RemoveRange(drawPile.Count - actualDraw, actualDraw);

            return cache;
        }

        public int ReturnFromDiscardPile()
        {
            int count = discardPile.Count;
            drawPile.AddRange(discardPile);
            drawPile.Shuffle(random);
            discardPile.Clear();
            return count;
        }

        public Card RemoveCardFromHand(int index)
        {
            if (index < 0 || index >= hand.Count)
            {
                Debug.LogError($"[{nameof(PlayArea)} IndexOutOfRange: index = {index}]");
                return null;
            }

            Card card = hand[index];
            hand.RemoveAt(index);
            return card;
        }

        public void AddCardToDiscardPile(Card card)
        {
            discardPile.Add(card);
        }

        public IReadOnlyList<Card> DiscardAllCardsFromHand()
        {
            var result = new List<Card>();
            for (var i = hand.Count - 1; i >= 0; i--)
            {
                result.Add(hand[i]);
                MoveCardFromHandToDiscardPile(i);
            }

            return result;
        }

        public void MoveCardFromHandToDiscardPile(Card card)
        {
            int index = hand.IndexOf(card);
            MoveCardFromHandToDiscardPile(index);
        }

        public void MoveCardFromHandToDiscardPile(int index)
        {
            if (index < 0 || index >= hand.Count)
            {
                Debug.LogError($"[{nameof(PlayArea)} IndexOutOfRange: index = {index}]");
                return;
            }

            discardPile.Add(hand[index]);
            hand.RemoveAt(index);
        }

        public void MoveCardFromHandToRemovedPile(Card card)
        {
            int index = hand.IndexOf(card);
            MoveCardFromHandToRemovedPile(index);
        }

        public void MoveCardFromHandToRemovedPile(int index)
        {
            if (index < 0 || index >= hand.Count)
            {
                Debug.LogError($"[{nameof(PlayArea)} IndexOutOfRange: index = {index}]");
                return;
            }

            removedPile.Add(hand[index]);
            hand.RemoveAt(index);
        }
    }
}
