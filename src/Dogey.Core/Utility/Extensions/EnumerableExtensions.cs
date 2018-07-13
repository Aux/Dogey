using System;
using System.Collections.Generic;

namespace Dogey
{
    public static class EnumerableExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var r = new Random();

            int count = list.Count;
            while (count > 1)
            {
                count--;
                int next = r.Next(count + 1);
                T value = list[next];
                list[next] = list[count];
                list[count] = value;
            }
        }
    }
}
