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
        public Inport<Game> gamePort;

        protected override FlowState OnExecute()
        {
            Game game = gamePort.GetValue();
            IReadOnlyList<Card> cards = game.DiscardAllCards();
            EnqueueEvent(new DiscardCardEvent { cards = cards });
            return FlowState.Success;
        }
    }
}
