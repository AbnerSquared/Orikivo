using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Text
{
    public class Paginator<T>
    {
        private readonly string _defaultValue;
        public Paginator(IEnumerable<T> elements, int pageSize, string defaultValue = null)
        {
            Elements = elements.ToList();
            PageSize = pageSize;
            _defaultValue = defaultValue;
        }

        public IReadOnlyList<T> Elements { get; }
        public int PageSize { get; }

        public int PageCount => Paginate.GetPageCount(Elements.Count, PageSize);

        public Page<T> PageAt(int index)
        {
            return new Page<T>(Paginate.GetElementOffset(Elements.Count, PageSize, index), Paginate.GroupAt(Elements, index, PageSize), _defaultValue);
        }

        public string ToString(int page, string formatter = "{0}", string separator = "\n", string elementFormatter = "{0}", int? characterLimit = null)
        {
            return PageAt(page).ToString(formatter, separator, elementFormatter, characterLimit);
        }
    }
}
