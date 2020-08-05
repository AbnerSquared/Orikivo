using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Text
{
    public class Page<T>
    {
        private readonly string _defaultValue;

        internal Page(int offset, IEnumerable<T> elements, string defaultValue = null)
        {
            Elements = elements.ToList(); // This might need to be tweaked to prevent .ToList().
            Offset = offset;
            _defaultValue = defaultValue;
        }

        public IReadOnlyList<T> Elements { get; }

        public int Offset { get; }

        public string ToString(string formatter, string separator, string elementFormatter, int? characterLimit = null)
        {
            var result = new StringBuilder();

            var i = 0;
            var len = 0;
            foreach (T element in Elements)
            {
                if (i > 0)
                    result.Append(separator);

                string value = string.Format(elementFormatter, element);

                if (characterLimit.HasValue)
                {
                    len += value.Length;

                    if (characterLimit.Value - len <= 0)
                        break;
                }

                result.Append(value);
                i++;
            }
            
            return string.Format(formatter, result);
        }

        public string ToString(PageFormatter<T> format)
        {
            var result = new StringBuilder();

            var i = 0;
            int len = format.BaseFormatter.Length - 3; // {0} is the -3 part
            foreach (T element in Elements)
            {
                if (i > 0)
                    result.Append(format.Separator);

                string value = format.ElementFormatter(element);

                if (format.CharacterLimit.HasValue)
                {
                    len += value.Length;

                    if (format.CharacterLimit.Value - len <= 0)
                        break;
                }

                result.Append(value);
                i++;
            }

            return string.Format(format.BaseFormatter, result);
        }
    }
}