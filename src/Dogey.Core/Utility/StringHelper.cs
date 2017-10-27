namespace Dogey
{
    public static class StringHelper
    {
        public static int RepeatingChars(object target, int length)
        {
            if (target == null)
                return 0;

            try
            {
                var targetString = target.ToString();
                int total = 0;
                for (int i = 0; i < targetString.Length - 1; i++)
                {
                    char c = targetString[i];

                    bool match = true;
                    for (int n = 0; n < length; n++)
                    {
                        if (targetString?[i + n] != c)
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match) total++;
                }
                return total;
            }
            finally { }
        }
    }
}
