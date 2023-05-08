using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
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
