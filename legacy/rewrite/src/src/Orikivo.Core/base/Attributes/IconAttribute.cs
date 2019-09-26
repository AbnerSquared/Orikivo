using Discord;
using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IconAttribute : Attribute
    {
        public Emoji Icon { get; }

        public IconAttribute(string unicode)
        {
            Icon = new Emoji(unicode);
        }
        public IconAttribute(Emoji emoji)
        {
            Icon = emoji;
        }
    }
}
