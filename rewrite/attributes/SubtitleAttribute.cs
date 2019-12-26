using System;

namespace Orikivo
{
    /// <summary>
    /// Marks a module to contain a subtitle that is read as a quick description.
    /// </summary>
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
