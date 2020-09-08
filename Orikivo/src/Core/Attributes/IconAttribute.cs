using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IconAttribute : Attribute
    {
        public IconAttribute(string icon)
        {
            Icon = icon;
        }

        public string Icon { get; }
    }
}
