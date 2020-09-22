using System.Collections.Generic;

namespace Arcadia
{
    public class Guide
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<string> Pages { get; set; } = new List<string>();
    }
}