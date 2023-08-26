using System;
using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class PlayCardNode : EntryNode
    {
        public class Payload : IEventContext
        {
            public Game game;
            public Player player;
            public Unit owner;
            public Card card;
            public Random random;
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

        public override bool CanExecute(IEventContext payloadObj)
        {
            var payload = payloadObj as Payload;
            if (payload == null)
            {
                return false;
            }

            int mana = payload.player.Mana;
            int cost = payload.card.GetStat(StatId.COST).CurrentValue;
            if (mana < cost)
            {
                return false;
            }

            return true;
        }

        protected override AbilityState DoLogic()
        {
            var payload = GetPayload<Payload>();

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
                    PayCosts(payload);
                }
            }
            else if (state == State.SELECTION)
            {
                state = State.COMPLETE;
                PayCosts(payload);
            }

            gamePort.SetValue(payload.game);
            playerPort.SetValue(payload.player);
            unitPort.SetValue(payload.owner);
            cardPort.SetValue(payload.card);
            return AbilityState.RUNNING;
        }

        private void PayCosts(Payload payload)
        {
            int cost = payload.card.GetStat(StatId.COST).CurrentValue;
            payload.player.Mana -= cost;

            EnqueueEvent(new ManaChangeEvent
            {
                modifyValue = -cost,
                newAmount = payload.player.Mana,
            });

            EnqueueEvent(new PlayCardEvent
            {
                card = payload.card,
            });
        }

        protected override void Reset()
        {
            state = State.INITIAL;
        }
    }
}
