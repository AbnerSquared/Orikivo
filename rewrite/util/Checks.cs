using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// A polite variant of Catch, only returning a Boolean value defining if the specified objects meet a requirement.
    /// </summary>
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

        public static bool NotNullOrEmpty<T>(IEnumerable<T> source, string name = null)
        {
            if (source?.Count() == 0) // Maybe check if all inner values are null as well.
            {
                if (!string.IsNullOrWhiteSpace(name))
                    Console.WriteLine("-- {name} -- is either null or empty.");
                return false;
            }
            return true;
        }

        public static bool NotNull(object obj, string name = null)
        {
            if (obj == null)
            {
                if (!string.IsNullOrWhiteSpace(name))
                    Console.WriteLine($"!= {obj} is null.");
                return false;
            }

            return true;
        }
    }
}
