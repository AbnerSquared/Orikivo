using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SubtitleAttribute : Attribute
    {
        public SubtitleAttribute(string subtitle)
        {
            Subtitle = subtitle;
        }

        public string Subtitle { get; }
    }
}
