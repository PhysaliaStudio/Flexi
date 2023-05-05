using System;

namespace Physalia.Flexi
{
    public enum CompareOperator
    {
        EQUAL,
        NOT_EQUAL,
        LESS,
        LESS_OR_EQUAL,
        GREATER,
        GREATER_OR_EQUAL,
    }

    public static class CompareOperatorExtensions
    {
        public static bool Compare<T>(this CompareOperator op, T a, T b) where T : IComparable
        {
            int result = a.CompareTo(b);
            return op switch
            {
                CompareOperator.EQUAL => result == 0,
                CompareOperator.NOT_EQUAL => result != 0,
                CompareOperator.LESS => result < 0,
                CompareOperator.LESS_OR_EQUAL => result <= 0,
                CompareOperator.GREATER => result > 0,
                CompareOperator.GREATER_OR_EQUAL => result >= 0,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
