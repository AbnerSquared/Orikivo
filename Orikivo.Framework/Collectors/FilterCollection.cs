using System;
using System.Collections;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents a collection of <see cref="FilterMatch"/> values received from a <see cref="MessageCollector"/>.
    /// </summary>
    public class FilterCollection : IEnumerable<FilterMatch>
    {
        /// <summary>
        /// The raw collection of matches.
        /// </summary>
        public List<FilterMatch> Matches { get; set; } = new List<FilterMatch>();

        public IEnumerator<FilterMatch> GetEnumerator()
            => Matches.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Matches.GetEnumerator();

        /// <summary>
        /// Returns the number of matches collected.
        /// </summary>
        public int Count => Matches.Count;

        /// <summary>
        /// Adds a <see cref="FilterMatch"/> to the <see cref="FilterCollection"/>.
        /// </summary>
        internal void Add(FilterMatch match)
        {
            Matches.Add(match);
        }

        /// <summary>
        /// Converts all of the matches into the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TValue">The enclosing <see cref="Type"/> that the <see cref="FilterMatch"/> will convert to.</typeparam>
        /// <param name="converter">The method used to convert the <see cref="FilterMatch"/>.</param>
        public List<TValue> Convert<TValue>(Func<FilterMatch, TValue> converter)
        {
            List<TValue> results = new List<TValue>();

            foreach (FilterMatch match in Matches)
                results.Add(match.Convert(converter));

            return results;
        }

        public FilterMatch this[int i]
        {
            get => Matches[i];
            set => Matches[i] = value;
        }
    }
}
