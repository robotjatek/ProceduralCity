using System;
using System.Collections.Generic;

namespace ProceduralCity.Extensions
{
    static class EnumerableExtensions
    {
        public static int CalculateHash(this IEnumerable<int> enumerable)
        {
            var hash = 17;
            foreach (var n in enumerable)
            {
                hash = 31 * hash + n;
            }

            return hash;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> a)
        {
            foreach (T item in enumerable)
            {
                a(item);
            }
        }
    }
}
