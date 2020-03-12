using System;

namespace Orikivo.Text
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GroupFormattingAttribute : Attribute // marks a property as IEnumerable
    {
        public GroupFormattingAttribute(string format, string separator = "", string elementFormat = "") : base()
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
