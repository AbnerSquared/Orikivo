using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Text.Pagination
{
    public static class Paginate
    {
        public static int GetElementOffset(int collectionSize, int pageSize, int page)
        {
            int pageCount = GetPageCount(collectionSize, pageSize);
            page = ClampIndex(page, pageCount);

            return pageSize * page;
        }

        public static int GetPageCount(int collectionSize, int size)
        {
            return (int) Math.Ceiling(collectionSize / (double) size);
        }

        public static int CountAtPage(int collectionSize, int pageSize, int page)
        {
            int offset = GetElementOffset(collectionSize, pageSize, page);

            return Clamp(0, pageSize, collectionSize - offset);
        }

        public static int ClampIndex(int page, int pageCount)
        {
            return page < 0 ? 0 : page >= pageCount ? pageCount - 1 : page;
        }

        private static int Clamp(int min, int max, int value)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static IEnumerable<T> GroupAt<T>(in IEnumerable<T> set, int page, int groupSize)
        {
            if (!set?.Any() ?? true)
                return set;

            int pageCount = GetPageCount(set.Count(), groupSize);
            page = ClampIndex(page, pageCount);

            var group = new List<T>();

            foreach (T item in set.Skip(groupSize * page))
            {
                if (group.Count >= groupSize)
                    break;

                group.Add(item);
            }

            return group;
        }

        public static string WriteAt<T>(in IEnumerable<T> set,
            int page,
            int groupSize,
            string formatter = "{0}",
            string separator = "\n",
            string elementFormatter = "{0}",
            string onEmptyElement = null,
            int? characterLimit = null)
        {
            return Write(GroupAt(set, page, groupSize), formatter, separator, elementFormatter, onEmptyElement, characterLimit);
        }

        public static string WriteAt<T>(in IEnumerable<T> set,
            int page,
            int groupSize,
            string formatter = "{0}",
            string separator = "\n",
            Func<T, string> selector = null,
            string onEmptyElement = null,
            int? characterLimit = null)
        {
            return Write(GroupAt(set, page, groupSize), formatter, separator, selector, onEmptyElement, characterLimit);
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

            if (!formatter.Contains("{0}"))
                return formatter;

            separator = string.IsNullOrWhiteSpace(separator) ? "\n" : separator;
            elementFormatter = string.IsNullOrWhiteSpace(elementFormatter) ? "{0}" : elementFormatter;

            int i = 0;
            int length = formatter.Length - 3;

            foreach (T element in elements)
            {
                if (i > 0)
                    result.Append(separator);

                string value = onEmptyElement;

                if (element != null)
                    value = string.Format(elementFormatter, element);

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (characterLimit.HasValue)
                {
                    length += value.Length;

                    if (characterLimit - length <= 0)
                        break;
                }

                result.Append(value);
                i++;
            }

            return string.Format(formatter, result);
        }

        public static string Write<T>(in IEnumerable<T> elements,
            string formatter = "{0}",
            string separator = "\n",
            Func<T, string> selector = null,
            string onEmptyElement = null,
            int? characterLimit = null)
        {
            var result = new StringBuilder();

            formatter = string.IsNullOrWhiteSpace(formatter) ? "{0}" : formatter;

            if (!formatter.Contains("{0}"))
                return formatter;

            separator = string.IsNullOrWhiteSpace(separator) ? "\n" : separator;
            selector ??= e => e.ToString();

            int i = 0;
            int length = formatter.Length - 3;

            foreach (T element in elements)
            {
                if (i > 0)
                    result.Append(separator);

                string value = onEmptyElement;

                if (element != null)
                    value = selector(element);

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (characterLimit.HasValue)
                {
                    length += value.Length;

                    if (characterLimit - length <= 0)
                        break;
                }

                result.Append(value);
                i++;
            }

            return string.Format(formatter, result);
        }
    }
}
