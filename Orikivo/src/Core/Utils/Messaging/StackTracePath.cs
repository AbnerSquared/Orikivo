using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orikivo
{
    public class StackTracePath
    {
        public static bool TryParse(string stackTrace, out List<StackTracePath> paths)
        {
            paths = new List<StackTracePath>();

            if (string.IsNullOrWhiteSpace(stackTrace))
                return false;

            var regex = new Regex(@"at (.+(?= in)|.+)(?:(?: in (.+):line (\d+))|\n+|$)");

            MatchCollection matches = regex.Matches(stackTrace);

            if (matches.Count == 0)
                return false;

            foreach (Match match in matches.Where(x => x.Success))
            {
                paths.Add(new StackTracePath
                {
                    Source = match.Value,
                    Method = match.Groups[1].Value,
                    Path = match.Groups[2].Value,
                    LineIndex = int.TryParse(match.Groups[3].Value, out int lineIndex) ? lineIndex : (int?) null
                });
            }

            return true;
        }

        public string Source { get; private set; }

        public string Method { get; private set; }

        public string Path { get; private set; }

        public int? LineIndex { get; private set; }

        public override string ToString()
            => Source;
    }
}
