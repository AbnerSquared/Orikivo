using System;

namespace Orikivo
{
    // instead of throwing, just checks if the object matches
    public static class Checks
    {
        public static bool NotNull(string str, string name = null)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                if (!string.IsNullOrWhiteSpace(name))
                    Console.WriteLine($"-- {name} is an invalid string. --");
                return false;
            }

            return true;
        }
    }
}
