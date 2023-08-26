using System;
using System.Linq.Expressions;

// This class is for resolving the issue of boxing when casting from generic Enum type to int.
// Reference: https://stackoverflow.com/a/23391746
namespace Physalia.Flexi
{
    /// <summary>
    /// Class to cast to type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    public static class CastTo<T>
    {
        private static class Cache<S>
        {
            public static readonly Func<S, T> Caster = Get();

            private static Func<S, T> Get()
            {
                ParameterExpression parameter = Expression.Parameter(typeof(S));
                UnaryExpression canConvert = Expression.ConvertChecked(parameter, typeof(T));
                return Expression.Lambda<Func<S, T>>(canConvert, parameter).Compile();
            }
        }

        /// <summary>
        /// Casts <see cref="S"/> to <see cref="T"/>.
        /// This does not cause boxing for value types. Useful in generic methods.
        /// </summary>
        /// <typeparam name="S">Source type to cast from. Usually a generic type.</typeparam>
        /// <remarks>The FIRST call will allocate GC for each type.</remarks>
        public static T From<S>(S s)
        {
            return Cache<S>.Caster(s);
        }
    }
}
