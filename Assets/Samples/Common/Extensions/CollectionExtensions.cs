using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Physalia.Flexi.Samples
{
    public static class CollectionExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int currentIndex = list.Count;
            while (currentIndex > 1)
            {
                currentIndex--;
                int randomIndex = random.Next(currentIndex + 1);

                // Swap
                T temp = list[randomIndex];
                list[randomIndex] = list[currentIndex];
                list[currentIndex] = temp;
            }
        }

        public static T RandomPickOne<T>(this IList<T> list, Random random)
        {
            if (list.Count == 0)
            {
                return default;
            }

            int randomIndex = random.Next(0, list.Count);
            return list[randomIndex];
        }

        public static T RandomPickOne<T>(this IReadOnlyList<T> list, Random random)
        {
            if (list.Count == 0)
            {
                return default;
            }

            int randomIndex = random.Next(0, list.Count);
            return list[randomIndex];
        }

        public static T RandomPickOne<T>(this IEnumerable<T> enumerable, Random random)
        {
            int count = enumerable.Count();
            if (count == 0)
            {
                return default;
            }

            int randomIndex = random.Next(0, count);
            var enumerator = enumerable.GetEnumerator();
            for (var i = 0; i < randomIndex + 1; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }

        public static string ToContentString<T>(this IList<T> list)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < list.Count; i++)
            {
                sb.Append(list[i].ToString());
                if (i != list.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        public static string ToContentString<T>(this IReadOnlyList<T> list)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < list.Count; i++)
            {
                sb.Append(list[i].ToString());
                if (i != list.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        public static string ToContentString<T>(this IEnumerable<T> enumerable)
        {
            var sb = new StringBuilder();

            IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                sb.Append(enumerator.Current.ToString());
                while (enumerator.MoveNext())
                {
                    sb.Append(",");
                    sb.Append(enumerator.Current.ToString());
                }
            }

            return sb.ToString();
        }
    }
}
