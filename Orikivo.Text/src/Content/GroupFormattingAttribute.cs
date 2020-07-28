using System;

namespace Orikivo.Text
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GroupFormattingAttribute : Attribute // marks a property as IEnumerable
    {
        public GroupFormattingAttribute(string format, string separator = "", string elementFormat = "")
        {
            Format = format;
            ElementFormat = elementFormat;
            Separator = separator;
        }

        public string Format { get; }
        public string ElementFormat { get; }
        public string Separator { get; }
    }
}
