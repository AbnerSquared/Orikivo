using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Text
{
    public class Paginator<T>
    {
        public Paginator(int pageSize)
        {
            PageSize = pageSize;
        }

        public T DefaultValue { get; set; } // used if the remaining values are missing.

        private IList Elements { get; set; }
        public int PageSize { get; set; }

        public int Count { get; }

        public Page<T> PageAt(int index)
        {
            return new Page<T>();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }


    public class Page<T>
    {
        // the offset from the original Paginator
        public int Offset { get; }
        public IReadOnlyList<T> Elements { get; }
    }

    public class Page
    {
        // System.Collections
        public IList Elements { get; }
    }

    public class Paginator
    {
        public Paginator(int pageSize)
        {

        }

        public object DefaultValue { get; set; }
        private IList Elements { get; set; }

        public int PageSize { get; set; }
        public int Count { get; }
        public Page PageAt(int index)
        {
            return new Page();
        }

        public string ToString(string format, string separator, string elementFormat)
        {
            return base.ToString();
        }
    }

    public static class Paginate
    {
        public static int GetPageCount(int collectionSize, int size)
        {
            return (int)Math.Ceiling((double)collectionSize / size);
        }

        public static int GetValueCountAtPage(int collectionSize, int size, int page)
        {
            page = Clamp(0, GetPageCount(collectionSize, size) - 1, page);
            return Clamp(0, size, collectionSize - (size * page));
        }

        private static int Clamp(int min, int max, int value)
            => value < min
            ? min
            : value > max
            ? max
            : value;

        public static IEnumerable<T> GroupAt<T>(IEnumerable<T> set, int page, int size)
        {
            int pageCount = GetPageCount(set.Count(), size);

            if (page < 0 || page >= pageCount)
                page = 0;

            if (set.Count() == 0)
                return set;

            var remainder = set.Skip(size * page);
            var group = new List<T>();

            for (int i = 0; i < size; i++)
            {
                if (remainder.Count() - 1 < i)
                    continue;
                else
                    group.Add(remainder.ElementAt(i));
            }

            return group;
        }
    }
}
