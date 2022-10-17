using System;
using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public class StatModifierInstance
    {
        private readonly StatModifier data;

        internal IReadOnlyList<StatModifierItem> Items => data.items;

        public StatModifierInstance(StatModifier data)
        {
            this.data = data;
        }
    }

    [Serializable]
    public class StatModifier
    {
        public List<StatModifierItem> items = new();
    }

    [Serializable]
    public class StatModifierItem
    {
        public enum Operator
        {
            SET = 0,
            ADD = 1,
            MUL = 2,
        }

        public int statId;
        public Operator op;
        public int value;
    }
}
