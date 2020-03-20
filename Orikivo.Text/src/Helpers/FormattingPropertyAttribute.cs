using System;

namespace Orikivo.Text
{
    // used alongside StringMap; might be scrapped
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormattingPropertyAttribute : Attribute
    {
        public string Name { get; }

        public FormattingPropertyAttribute(string name = null)
        {
            Name = name;
        }
    }
}
