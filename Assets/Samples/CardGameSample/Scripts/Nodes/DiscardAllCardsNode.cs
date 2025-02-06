using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class DiscardCardEvent : IEventContext
    {
        public IReadOnlyList<Card> cards;
    }

    [NodeCategory("Card Game Sample")]
    public class DiscardAllCardsNode : DefaultProcessNode
    {
        protected override FlowState OnExecute()
        {
            IReadOnlyList<Card> cards = Container.Game.DiscardAllCards();
            EnqueueEvent(new DiscardCardEvent { cards = cards });
            return FlowState.Success;
        }
    }
}
