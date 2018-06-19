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

        public static bool IsPrime(ulong candidate)
        {
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                    return true;
                return false;
            }

            for (ulong i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                    return false;
            }
            return candidate != 1;
        }

        public static DateTime GetDateTime(long value)
        {
            var reference = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var actual = reference.AddMilliseconds(value).ToLocalTime();
            return actual;
        }

        public static double KelvinToFahrenheit(double k)
            => Math.Round(k * 9 / 5 - 459.67, 1);
        public static double KelvinToCelsius(double k)
            => Math.Round(k - 273.15, 1);
    }
}
