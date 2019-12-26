using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class FilterCollection
    {
        public List<FilterMatch> Matches { get; set; } = new List<FilterMatch>();

        public int Count => Matches.Count;

        internal void Add(FilterMatch match)
        {
            Matches.Add(match);
        }

        public List<TValue> Convert<TValue>(Func<FilterMatch, TValue> converter)
        {
            List<TValue> results = new List<TValue>();

            foreach (FilterMatch match in Matches)
                results.Add(match.Convert(converter));

            return results;
        }
    }
}
