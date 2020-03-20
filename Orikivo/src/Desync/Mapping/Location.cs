using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic accessible <see cref="Region"/> in a <see cref="World"/>.
    /// </summary>
    public abstract class Location : Region
    {
        public abstract LocationType Type { get; }
        public override RegionType Subtype => RegionType.Location;

        public Sprite Exterior { get; set; }

        // attempt to get this location's parent, if it has one
        public Location GetParent()
        {
            return Engine.World?.GetChildren()?
                .FirstOrDefault(x => x.GetChildren(false).Any(x => x.Id == Id));
        }

        public virtual List<Location> GetChildren(bool includeInnerChildren = true)
            => Type switch
            {
                LocationType.World => throw new NotSupportedException("A LocationType of World cannot be referenced in a Location class."),
                _ => new List<Location>()
            };

        public virtual List<Region> GetRegions()
            => new List<Region>();

        public Locator GetLocator()
        {
            var origin = Perimeter.Origin;

            return new Locator()
            {
                WorldId = Engine.World.Id,
                Id = Id,
                X = origin.X,
                Y = origin.Y
            };
        }

        // attempt to get the width/height/x/y for a location in it's global position.

        // attempt to find a location within this location.
        public Location Find()
            => throw new NotImplementedException();
    }
}
