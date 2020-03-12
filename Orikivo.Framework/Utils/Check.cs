using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents a soft handler for possible exceptions.
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Determines if a specified <see cref="string"/> is valid.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to verify.</param>
        /// <param name="name">The name of the <see cref="string"/> to reference if the comparison failed.</param>
        public static bool NotNull(string text, string name = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (!string.IsNullOrWhiteSpace(name))
                    Console.WriteLine($"-- The specified string '{name}' is invalid. --");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if all specified <see cref="string"/> values are valid.
        /// </summary>
        public static bool NotNull(params string[] strings)
        {
            foreach (string s in strings)
                if (!NotNull(s))
                    return false;

            return true;
        }

        /// <summary>
        /// Determines if a specified <see cref="IEnumerable{T}"/> contains at least a single element.
        /// </summary>
        /// <typeparam name="T">The type that is stored within the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to verify.</param>
        /// <param name="name">The name of the <see cref="IEnumerable{T}"/> to reference if the comparison failed.</param>
        /// <returns></returns>
        public static bool NotNullOrEmpty<T>(IEnumerable<T> source, string name = null)
        {
            if (source == null)
                return false;

            if (source?.Count() == 0) // Maybe check if all inner values are null as well.
            {
                if (!string.IsNullOrWhiteSpace(name))
                    Console.WriteLine("-- {name} -- is either null or empty.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if a specified <see cref="object"/> is null.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to verify.</param>
        /// <param name="name">The name of the <see cref="object"/> to reference if the comparison failed.</param>
        /// <returns></returns>
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
