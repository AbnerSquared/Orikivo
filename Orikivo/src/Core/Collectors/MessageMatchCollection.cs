using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents a collection of <see cref="MessageMatch"/> values received from a <see cref="MessageCollector"/>.
    /// </summary>
    public class MessageMatchCollection : List<MessageMatch>
    {
        /// <summary>
        /// Converts all of the matches into the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TValue">The enclosing <see cref="Type"/> that the <see cref="MessageMatch"/> will convert to.</typeparam>
        /// <param name="converter">The method used to convert the <see cref="MessageMatch"/>.</param>
        public List<TValue> Convert<TValue>(Func<MessageMatch, TValue> converter)
        {
            var results = new List<TValue>();

            foreach (MessageMatch match in this)
                results.Add(match.Convert(converter));

            return results;
        }
    }
}
