using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class DrawCardEvent : IEventContext
    {
        public IReadOnlyList<Card> cards;
    }

    public class ReturnDiscardPileEvent : IEventContext
    {
        public int count;
    }

    [NodeCategory("Card Game Sample")]
    public class DrawCardNode : DefaultProcessNode
    {
        public Inport<int> countPort;

        protected override FlowState OnExecute()
        {
            Game game = Container.Game;
            int count = countPort.GetValue();

            IReadOnlyList<Card> firstDraw = game.DrawCard(count);
            EnqueueEvent(new DrawCardEvent { cards = firstDraw });

            if (firstDraw.Count < count)
            {
                int returnCount = game.ReturnFromDiscardPile();
                EnqueueEvent(new ReturnDiscardPileEvent { count = returnCount });

                int remainCount = count - firstDraw.Count;
                IReadOnlyList<Card> secondDraw = game.DrawCard(remainCount);
                EnqueueEvent(new DrawCardEvent { cards = secondDraw });
            }

            return FlowState.Success;
        }
    }
}
