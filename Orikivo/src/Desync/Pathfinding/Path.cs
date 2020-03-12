using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic path between two positions.
    /// </summary>
    public class Path
    {
        public PathNode From { get; set; }
        public List<PathNode> Nodes { get; set; }

        public PathNode To { get; set; }

        internal static string GetFileNameWithoutExtension(string path)
        {
            throw new NotImplementedException();
        }
    }
}
