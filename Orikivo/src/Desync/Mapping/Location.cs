using Orikivo.Drawing;
using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic plot of land in a <see cref="World"/>.
    /// </summary>
    public class Region
    {
        public virtual string Id { get; set; }

        public string Name { get; set; }

        public RegionF Perimeter { get; set; }

        /// <summary>
        /// Gets a floating-point integer that represents the global x-coordinate of this <see cref="Region"/>.
        /// </summary>
        public float Longitude { get; }

        /// <summary>
        /// Gets a floating-point integer that represents the global y-coordinate of this <see cref="Region"/>.
        /// </summary>
        public float Latitude { get; }
    }

    /// <summary>
    /// Represents a generic accessible <see cref="Region"/> in a <see cref="World"/>.
    /// </summary>
    public abstract class Location : Region
    {
        public abstract LocationType Type { get; }

        public Location Parent { get => GetParent(); }

        public virtual List<Location> Children { get; } = new List<Location>();

        // attempt to get this location's parent, if it has one
        private Location GetParent()
        {
            Engine.World.Find(Id);
            throw new NotImplementedException();
        }

        // attempt to find a location within this location.
        public Location Find()
            => throw new NotImplementedException();
    }
}
