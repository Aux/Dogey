using System;
using System.Linq;
using System.Text;

namespace Dogey
{
    public static class StringHelper
    {
        public static int RepeatingChars(string target, int length)
        {
            int total = 0;
            for (int i = 0; i < target.Length - 1; i++)
            {
                char c = target[i];

                bool match = true;
                for (int n = 0; n < length; n++)
                {
                    if (target.ElementAtOrDefault(i + n) != c)
                    {
                        match = false;
                        break;
                    }
                }

                if (match) total++;
            }
            return total;
        }

        public static bool TryParseBoolean(string value, out bool result)
        {
            switch (value.ToLower().Trim())
            {
                case "true":
                case "yes":
                case "t":
                case "y":
                case "1":
                    result = true;
                    return true;
                case "false":
                case "no":
                case "f":
                case "n":
                case "0":
                    result = false;
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        public static string ToBase64(this string value)
        {
            if (value == null) return null;
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
        public static string FromBase64(this string value)
        {
            if (value == null) return null;
            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
