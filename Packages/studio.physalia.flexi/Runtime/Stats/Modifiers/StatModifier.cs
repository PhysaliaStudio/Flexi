using System;

namespace Physalia.Flexi
{
    [Serializable]
    public struct StatModifier
    {
        public enum Operator
        {
            SET = 0,
            ADD = 1,
            MUL = 2,
        }

        public StatModifier(int statId, int value, Operator op)
        {
            this.statId = statId;
            this.value = value;
            this.op = op;
        }

        public int statId;
        public int value;
        public Operator op;
    }
}
