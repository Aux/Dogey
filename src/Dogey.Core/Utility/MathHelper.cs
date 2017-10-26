using System;
using System.Linq;

namespace Dogey
{
    public static class MathHelper
    {
        public static int GetStringDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        public static bool IsPrime(ulong number)
        {
            if ((number & 1) == 0)
                return number == 2;
            
            for (ulong i = 3; (i * i) <= number; i += 2)
            {
                if ((number % i) == 0)
                    return false;
            }

            return number != 1;
        }
    }
}
