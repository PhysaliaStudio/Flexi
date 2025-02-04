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

        public int statId;
        public int value;
        public Operator op;

        public static StatModifier Create<T>(T statId, int value, Operator op) where T : Enum
        {
            return new StatModifier
            {
                statId = CastTo<int>.From(statId),
                value = value,
                op = op,
            };
        }

        public static StatModifier Create(int statId, int value, Operator op)
        {
            return new StatModifier
            {
                statId = statId,
                value = value,
                op = op,
            };
        }
    }
}
