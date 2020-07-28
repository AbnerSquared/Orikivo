using System;

namespace Orikivo.Text
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormattingAttribute : Attribute
    {
        public FormattingAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; }
    }
}
