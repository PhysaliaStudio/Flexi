using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class PlayCardNode : DefaultEntryNode<PlayCardNode.Context>
    {
        public class Context : IEventContext
        {
            public Game game;
            public Player player;
            public Unit owner;
            public Card card;
        }

        private enum State
        {
            INITIAL, SELECTION, COMPLETE,
        }

        public Outport<FlowNode> selectionPort;
        public Outport<Game> gamePort;
        public Outport<Player> playerPort;
        public Outport<Unit> unitPort;
        public Outport<Card> cardPort;

        private State state = State.INITIAL;

        public override FlowNode Next
        {
            get
            {
                if (state == State.SELECTION)
                {
                    IReadOnlyList<Port> connections = selectionPort.GetConnections();
                    return connections.Count > 0 ? connections[0].Node as FlowNode : null;
                }
                else if (state == State.COMPLETE)
                {
                    return base.Next;
                }
                else
                {
                    return null;
                }
            }
        }

        protected override bool CanExecute(Context context)
        {
            int mana = context.player.Mana;
            int cost = context.card.GetStat(StatId.COST).CurrentValue;
            if (mana < cost)
            {
                return false;
            }

            return true;
        }

        protected override FlowState OnExecute(Context context)
        {
            if (state == State.INITIAL)
            {
                if (selectionPort.GetConnections().Count > 0)
                {
                    state = State.SELECTION;
                    PushSelf();
                }
                else
                {
                    state = State.COMPLETE;
                    PayCosts(context);
                }
            }
            else if (state == State.SELECTION)
            {
                state = State.COMPLETE;
                PayCosts(context);
            }

            gamePort.SetValue(context.game);
            playerPort.SetValue(context.player);
            unitPort.SetValue(context.owner);
            cardPort.SetValue(context.card);
            return FlowState.Success;
        }

        private void PayCosts(Context context)
        {
            int cost = context.card.GetStat(StatId.COST).CurrentValue;
            context.player.Mana -= cost;

            EnqueueEvent(new ManaChangeEvent
            {
                modifyValue = -cost,
                newAmount = context.player.Mana,
            });

            EnqueueEvent(new PlayCardEvent
            {
                card = context.card,
            });
        }

        protected override void Reset()
        {
            state = State.INITIAL;
        }
    }
}
