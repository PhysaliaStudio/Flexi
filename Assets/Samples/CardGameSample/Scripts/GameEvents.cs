using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class TurnEndEvent : IEventContext
    {
        public Game game;
    }

    public class PlayCardEvent : IEventContext
    {
        public Card card;
        public bool isRemoved;

        public override string ToString()
        {
            return $"Play Card: {card}, Removed: {isRemoved}";
        }
    }

    public class ManaChangeEvent : IEventContext
    {
        public int modifyValue;
        public int newAmount;

        public override string ToString()
        {
            return $"Mana += {modifyValue} => {newAmount}";
        }
    }
    
    public class HealEvent : IEventContext
    {
        public IReadOnlyList<Unit> targets;
        public int amount;

        public override string ToString()
        {
            return $"{targets.ToContentString()} get {amount} healed";
        }
    }

    public class DamageEvent : IEventContext
    {
        public Unit attacker;
        public IReadOnlyList<Unit> targets;
        public int amount;

        public override string ToString()
        {
            return $"{targets.ToContentString()} received {amount} damage";
        }
    }

    public class UnitSpawnedEvent : IEventContext
    {
        public IReadOnlyList<Unit> units;
    }

    public class DeathEvent : IEventContext
    {
        public Unit target;
    }
}
