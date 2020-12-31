using System.Collections.Generic;

namespace Arcadia
{
    public class Guide
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        // Represents the entire guide as one string
        // The guide is split by `<#>`, which denotes a new section.
        // The name of a section can be specified by writing the title after the hash-tag
        // In order to write '>' in a name, you need to escape it first '\>'
        // Example: <#Section Name>Section Content
        public string Content { get; set; }
        public List<string> Pages { get; set; } = new List<string>();
    }
}