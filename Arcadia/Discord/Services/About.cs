using System.Collections.Generic;
using System.Text;

namespace Arcadia.Modules
{
    public static class About
    {

        public static readonly Dictionary<string, string> Pages = new Dictionary<string, string>
        {
            ["research"] = $""
        };

        public static string View()
        {
            var result = new StringBuilder();
            return result.ToString();
        }

        public static string ViewFor()
        {
            var result = new StringBuilder();
            return result.ToString();
        }
    }
}