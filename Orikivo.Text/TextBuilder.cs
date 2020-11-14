using System.Collections.Generic;
using System.Text;
using Orikivo.Text.Pagination;

namespace Orikivo.Text
{
    public class TextBuilder
    {
        private readonly StringBuilder _builder;

        public TextBuilder()
        {
            _builder = new StringBuilder();
        }

        public TextBuilder(int pageCapacity)
        {

        }

        // This is the content that is always prepended to the start of the text.
        public string BaseContent { get; set; }

        // The maximum size of each page
        public int PageCapacity { get; }

        public TextSplitOptions SplitOptions { get; set; }

        public IEnumerable<string> Build()
        {
            if (PageCapacity == -1)
            {
                return new List<string>
                {
                    _builder.ToString()
                };
            }

            var collection = new List<string>();

            foreach (string element in Paginate.GetPages(_builder.ToString(), PageCapacity, SplitOptions, BaseContent.Length))
            {
                collection.Add($"{BaseContent}\n{element}");
            }

            return Paginate.GetPages(collection, PageCapacity, BaseContent.Length);
        }
    }
}
