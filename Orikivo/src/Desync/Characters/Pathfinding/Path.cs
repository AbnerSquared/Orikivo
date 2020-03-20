using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic connection between two coordinates.
    /// </summary>
    public class Path
    {
        public List<PathNode> Nodes { get; set; }
    }
}
