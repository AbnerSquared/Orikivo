using Orikivo.Text;
using System.Collections.Generic;
using Orikivo.Text.Pagination;
using System;

namespace Arcadia
{
    // These are used to set placeholder text values in a written piece of text.
    // Hello there, {user}.
    // new TextTag { Alias = "user", Value = reference => reference    }
    public class TextTag<T>
    {
        public string Alias { get; set; }

        public Func<T, string> Writer { get; set; }
    }

    public class UserTag : TextTag<ArcadeUser> { }

    public static class TextBodyExtensions
    {
        public static TextBody WithSection(this TextBody body, string title, string content, string icon = null)
        {
            body.Sections.Add(new TextSection { Title = title, Content = content, Icon = icon });
            return body;
        }

        public static TextBody WithSection(this TextBody body, TextSection section)
        {
            body.Sections.Add(section);
            return body;
        }

        public static TextBody WithHeader(this TextBody body, string title, string group = null, string extra = null, string icon = null, string subtitle = null)
        {
            body.Header = Locale.CreateHeader(title, group, extra, icon, subtitle);
            return body;
        }

        public static TextBody WithHeader(this TextBody body, Header header)
        {
            body.Header = header;
            return body;
        }

        public static TextBody AppendTip(this TextBody body, string tip)
        {
            body.Tooltips.Add(tip);
            return body;
        }
    }

    public class TextBody
    {
        public Language Language { get; set; } = Language.English;

        public List<string> Tooltips { get; set; } = new List<string>();

        public Header Header { get; set; }

        public List<TextSection> Sections { get; set; } = new List<TextSection>();

        public string Build(bool allowTooltips = true)
            => Locale.BuildMessage(this, allowTooltips);
    }

    public class TextBodyOptions
    {
        public Language Language { get; set; } = Language.English;

        public bool ShowTips { get; set; } = true;

        public int? CharacterLimit { get; set; }

        public int? PageCharacterLimit { get; set; } = 2000;
        
        public GroupSplitOptions SplitOptions = GroupSplitOptions.Element;

        public int? MaxSectionsPerPage { get; set; }

        public bool ShowHeaderOnPartialSection { get; set; }

        public SectionSplitOptions SectionOptions { get; set; }
    }

    // This is used to handle how headers are drawn on text bodies
    public enum HeaderOptions
    {
        Relative = 1, // The header is only shown on the first page (the page bar is isolated as its own title)
        Static = 2 // The header is shown across all pages (the page bar is moved to the header's extra content)
    }

    public enum LimitTarget
    {
        Content = 1, // The character limit is applied to the entire content length
        Page = 2, // The character limit is applied to the current page length 
        Element = 3, // The character limit is applied to the current element length
        Row = 4 // The character limit is applied to the current line length
    }

    public enum OverflowHandling
    {
        Clip = 1, // All text over the limit is cut
        Error = 2 // If text is longer than the specified limit, throw an error
    }

    public enum SectionSplitOptions
    {
        Reset = 1, // If a section is cropped, the next page will rewrite the section as new
        ForceHeader = 2, // If a section is cropped, the next page will show the header for the section and finish its content
        Ignore = 3, // If a section is cropped, The remaining text is written out as normal.
    }
}