using System;

namespace Physalia.AbilitySystem
{
    [Serializable]
    public class AbilityEffect
    {
        public enum Operator
        {
            SET = 0,
            ADD = 1,
            MUL = 2,
        }

        public int StatId;
        public Operator Op;
        public int Value;
    }
}
