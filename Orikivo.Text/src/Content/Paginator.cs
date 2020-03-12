using System.Collections;
using System.Collections.Generic;

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
}
