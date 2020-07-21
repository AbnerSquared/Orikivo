using System;
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
            if (!string.IsNullOrWhiteSpace(text))
                return true;

            if (!string.IsNullOrWhiteSpace(name))
                    Console.WriteLine($"[CHECK] String '{name}' is either null, empty, or only consists of whitespace.");

            return false;
        }

        /// <summary>
        /// Determines if all specified <see cref="string"/> values are valid.
        /// </summary>
        public static bool NotNull(params string[] strings)
        {
            foreach (string s in strings)
            {
                if (!NotNull(s))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if a specified <see cref="IEnumerable{T}"/> exists or contains at least a single element.
        /// </summary>
        /// <typeparam name="T">The type that is stored within the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to verify.</param>
        /// <param name="name">The name of the <see cref="IEnumerable{T}"/> to reference if the comparison failed.</param>
        public static bool NotNullOrEmpty<T>(IEnumerable<T> source, string name = null)
        {
            if (source == null)
                return false;

            if (source.Any())
                return true;
            
            if (!string.IsNullOrWhiteSpace(name))
                Console.WriteLine($"[CHECK] Enumerable '{name}' does not exist or contain any elements.");

            return false;
            }

        /// <summary>
        /// Determines if a specified <see cref="object"/> is null.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to verify.</param>
        /// <param name="name">The name of the <see cref="object"/> to reference if the comparison failed.</param>
        public static bool NotNull(object obj, string name = null)
        {
            if (obj != null)
                return true;

            if (!string.IsNullOrWhiteSpace(name)) 
                Console.WriteLine($"[CHECK] Object '{name}' does not exist.");

            return false;
        }
    }
}
