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
        /// <summary>
        /// Represents the base type of this <see cref="Location"/>.
        /// </summary>
        public abstract LocationType Type { get; }

        public override RegionType Subtype => RegionType.Location;

        /// <summary>
        /// Represents the image used to showcase this <see cref="Location"/>, if any.
        /// </summary>
        public Sprite Exterior { get; set; }

        /// <summary>
        /// Attempts to return the parent of this <see cref="Location"/>, if it has one.
        /// </summary>
        public Location GetParent()
        {
            return Engine.World?.GetChildren()?
                .FirstOrDefault(x => x.GetChildren(false).Any(x => x.Id == Id));
        }

        /// <summary>
        /// Attempts to return the base parent of this <see cref="Location"/>, if it has one.
        /// </summary>
        public Location GetBaseParent()
        {
            Location parent = GetParent();

            if (parent == null)
                return parent;

            while (parent != null)
            {
                var outerParent = parent.GetParent();

                if (outerParent == null)
                    break;

                parent = outerParent;
            }

            return parent;
        }

        [Flags]
        public enum ChildType
        {
            Structure = 1,
            Region = 2,
            Biome = 4,
            Area = 8,
            Construct = 16,
            Floor = 32,
            Barrier = 64
        }

        /// <summary>
        /// Returns a bitwise flag that represents all of the possible children this <see cref="Location"/> can store.
        /// </summary>
        public virtual ChildType GetAllowedChildren()
            => 0;

        /// <summary>
        /// Returns all of the children in this <see cref="Location"/>.
        /// </summary>
        /// <param name="includeInnerChildren">Determines if any inner children in the base children should be included.</param>
        public virtual List<Location> GetChildren(bool includeInnerChildren = true)
            => Type switch
            {
                LocationType.World => throw new NotSupportedException("A LocationType of World cannot be referenced in a Location class."),
                _ => new List<Location>()
            };

        /// <summary>
        /// Returns all of the generic regions in this <see cref="Location"/>.
        /// </summary>
        public virtual List<Region> GetRegions()
            => new List<Region>();

        /// <summary>
        /// Returns a cached version of this <see cref="Location"/>.
        /// </summary>
        public Locator GetLocator()
        {
            var origin = Shape.Origin;

            return new Locator()
            {
                WorldId = Engine.World.Id,
                Id = Id,
                X = origin.X,
                Y = origin.Y
            };
        }

        /// <summary>
        /// Searches for an inner <see cref="Region"/> by ID. If a <see cref="Region"/> could not be found, this returns null.
        /// </summary>
        public Region Find(string id)
        {
            TryFind(id, out Region region);
            return region;
        }

        /// <summary>
        /// Attempts to search for an inner <see cref="Region"/> by its ID.
        /// </summary>
        public bool TryFind(string id, out Region region)
        {
            region = null;
            var regions = GetRegions();

            foreach(Region value in regions)
            {
                if (value.Id == id)
                {
                    region = value;
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<Region> Filter(RegionType type)
        {
            return GetRegions()
                .Where(x => type.HasFlag(x.Subtype));
        }

        public IEnumerable<Region> Filter(ConstructType type)
        {
            if (!GetAllowedChildren().HasFlag(ChildType.Construct))
                throw new Exception("This location does not carry any children of type 'Construct'.");

            return GetRegions()
                .Where(x => x.Subtype == RegionType.Location)
                .Where(x => (x as Location).Type == LocationType.Construct)
                .Where(x => type.HasFlag((x as Construct).Tag));
        }

        public IEnumerable<Region> Filter(StructureType type)
        {
            if (!GetAllowedChildren().HasFlag(ChildType.Structure))
                throw new Exception("This location does not carry any children of type 'Structure'.");

            return GetRegions()
                .Where(x => x.Subtype == RegionType.Structure)
                .Where(x => type.HasFlag((x as Structure).Type));
        }
    }
}
