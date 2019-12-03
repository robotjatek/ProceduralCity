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
    }
}
