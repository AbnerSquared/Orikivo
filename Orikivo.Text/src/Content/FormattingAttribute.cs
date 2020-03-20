using System;

namespace Orikivo.Text
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormattingAttribute : Attribute
    {
        public FormattingAttribute(string format) : base()
        {
            Format = format;
        }

        public string Format { get; }
    }
}
