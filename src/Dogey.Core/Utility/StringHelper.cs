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
                    if (target?[i + n] != c)
                    {
                        match = false;
                        break;
                    }
                }

                if (match) total++;
            }
            return total;
        }
    }
}
