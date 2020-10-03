using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Text.Pagination
{
    public enum TextSplitOptions
    {
        Character = 1,
        Word = 2,
        Line = 3
    }

    public enum GroupSplitOptions
    {
        Character = 1,
        Word = 2,
        Row = 3,
        Element = 4
    }

    public static class Paginate
    {
        // What is the offset length for this collection on the specified index?
        public static int GetCollectionOffset(int collectionSize, int groupSize, int index)
        {
            int pageCount = GetPageCount(collectionSize, groupSize);
            index = ClampIndex(index, pageCount);

            return groupSize * index;
        }

        // How many groups are in this collection?
        public static int GetPageCount(int collectionSize, int groupSize)
        {
            return (int) Math.Ceiling(collectionSize / (double) groupSize);
        }

        public static int GetSplitCount(IEnumerable<string> collection, int characterLimit, int baseLength = 0)
        {
            if (baseLength < 0)
            {
                baseLength = 0;
            }

            int length = baseLength;
            int count = 0;

            foreach (string value in collection)
            {
                int valueLength = value?.Length ?? 0;

                if (length + valueLength > characterLimit)
                {
                    length = baseLength + valueLength;
                    count++;
                    continue;
                }

                length += valueLength;
            }

            return count;
        }

        public static int CountAtPage(int collectionSize, int pageSize, int page)
        {
            int offset = GetCollectionOffset(collectionSize, pageSize, page);

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
            {
                return set;
            }

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
            {
                return formatter;
            }

            separator = string.IsNullOrWhiteSpace(separator) ? "\n" : separator;
            elementFormatter = string.IsNullOrWhiteSpace(elementFormatter) ? "{0}" : elementFormatter;
            int length = formatter.Length - 3;

            string WriteElement(T x)
            {
                return x != null ? string.Format(elementFormatter, x) : onEmptyElement;
            }

            WriteElements(ref result, ref length, elements.Select(WriteElement), separator, characterLimit);
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
            {
                return formatter;
            }

            separator = string.IsNullOrWhiteSpace(separator) ? "\n" : separator;
            selector ??= e => e.ToString();
            int length = formatter.Length - 3;

            string WriteElement(T x)
            {
                return x != null ? selector(x) : onEmptyElement;
            }

            WriteElements(ref result, ref length, elements.Select(WriteElement), separator, characterLimit);
            return string.Format(formatter, result);
        }

        private static void WriteElements(ref StringBuilder writer, ref int length, in IEnumerable<string> sections, string separator, int? characterLimit = null)
        {
            int i = 0;

            foreach (string section in sections.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                length += section.Length;

                if (characterLimit.HasValue && characterLimit - length <= 0)
                {
                    return;
                }

                if (i > 0)
                {
                    writer.Append(separator);
                }

                writer.Append(section);
                i++;
            }
        }
    }
}
