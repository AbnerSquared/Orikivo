using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Text
{
    public static class Paginate
    {
        public static int GetElementOffset(int collectionSize, int pageSize, int page)
        {
            int pageCount = GetPageCount(collectionSize, pageSize);
            page = Clamp(0, pageCount, page);

            return collectionSize * page;
        }

        public static int GetPageCount(int collectionSize, int size)
        {
            return (int) Math.Ceiling(collectionSize / (double) size);
        }

        public static int GetValueCountAtPage(int collectionSize, int size, int page)
        {
            page = Clamp(0, GetPageCount(collectionSize, size) - 1, page);
            return Clamp(0, size, collectionSize - size * page);
        }

        private static int Clamp(int min, int max, int value)
            => value < min
                ? min
                : value > max
                    ? max
                    : value;

        public static IEnumerable<T> GroupAt<T>(in IEnumerable<T> set, int page, int size)
        {
            if (!set.Any())
                return set;

            int pageCount = GetPageCount(set.Count(), size);
            page = Clamp(0, pageCount, page);

            var group = new List<T>();

            foreach (T item in set.Skip(size * page))
            {
                if (group.Count >= size)
                    break;

                group.Add(item);
            }

            return group;
        }

        public static string Write<T>(in IEnumerable<T> elements,
            string formatter = "{0}",
            string separator = "\n",
            string elementFormatter = "{0}",
            string onEmptyElement = null,
            int? characterLimit = null)
        {
            var result = new StringBuilder();

            formatter = string.IsNullOrWhiteSpace(formatter) ? "{0}" : formatter;
            separator = string.IsNullOrWhiteSpace(separator) ? "\n" : separator;
            elementFormatter = string.IsNullOrWhiteSpace(elementFormatter) ? "{0}" : elementFormatter;

            int i = 0;
            // NOTE: -3 excludes the '{0}' specified in the formatter.
            int length = formatter.Length - 3;

            foreach (T element in elements)
            {
                if (i > 0)
                    result.Append(separator);

                // NOTE: onEmptyElement is used if the current element does not have a specified value.
                //       If onEmptyElement stays null, the row is ignored.
                string row = onEmptyElement;

                if (element != null)
                    row = string.Format(elementFormatter, element);

                if (string.IsNullOrWhiteSpace(row))
                    continue;

                if (characterLimit.HasValue)
                {
                    length += row.Length;

                    if (characterLimit - length <= 0)
                        break;
                }

                result.Append(row);
                i++;
            }

            return string.Format(formatter, result);
        }

        public static string WriteCompact<T>(in IEnumerable<T> elements,
            string formatter = "{0}",
            string separator = "\n", Func<T, string> elementFormatter = null,
            string onEmptyElement = null)
        {
            formatter = string.IsNullOrWhiteSpace(formatter) ? "{0}" : formatter;
            separator = string.IsNullOrWhiteSpace(separator) ? "\n" : separator;
            elementFormatter ??= e => e.ToString();

            string inner = string.Join(separator,
                elements
                    .Select(delegate(T element)
                    {
                        string row = element != null ? elementFormatter(element) : onEmptyElement;
                        return string.IsNullOrWhiteSpace(row) ? null : row;
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            return string.Format(formatter, inner);
        }

        public static string Write<T>(in IEnumerable<T> elements,
            string formatter = "{0}",
            string separator = "\n",
            Func<T, string> elementFormatter = null,
            string onEmptyElement = null,
            int? characterLimit = null)
        {
            var result = new StringBuilder();

            formatter = string.IsNullOrWhiteSpace(formatter) ? "{0}" : formatter;
            separator = string.IsNullOrWhiteSpace(separator) ? "\n" : separator;
            elementFormatter ??= e => e.ToString();

            int i = 0;
            // NOTE: -3 excludes the '{0}' specified in the formatter.
            int length = formatter.Length - 3;

            foreach (T element in elements)
            {
                if (i > 0)
                    result.Append(separator);

                // NOTE: 'onEmptyElement' is used if the current element does not have a specified value.
                //       If 'onEmptyElement' stays null, the row is ignored.
                string row = onEmptyElement;

                if (element != null)
                    row = elementFormatter(element);

                if (string.IsNullOrWhiteSpace(row))
                    continue;

                if (characterLimit.HasValue)
                {
                    length += row.Length;

                    if (characterLimit - length <= 0)
                        break;
                }

                result.Append(row);
                i++;
            }

            return string.Format(formatter, result);
        }
    }
}