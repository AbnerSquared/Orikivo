using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Defines a location-based precondition that an action is bound to when true.
    /// </summary>
    public class BindToRegionAttribute : Attribute
    {
        internal enum BindType
        {
            Ids,
            WorldDepth,
            IdDepth,
            Region,
            Location,
            RegionDepth,
            LocationDepth,
            Construct,
            Structure,
            WorldCoordinate,
            WorldPerimeter,
            WorldCircle,
            IdCoordinate,
            IdPerimeter,
            IdCircle
        }

        // binds an action to the specified ids
        public BindToRegionAttribute(string id, params string[] rest)
        {
            var ids = new string[rest.Length + 1];

            ids[0] = id;

            for (int i = 0; i < rest.Length; i++)
            {
                ids[i + 1] = rest[i];
            }

            Ids = ids;
            Type = BindType.Ids;
        }

        // binds an action to the world at the specified depth
        public BindToRegionAttribute(BindDepth depth)
        {
            Depth = depth;
            Type = BindType.WorldDepth;
        }

        // binds an action to the specified id at the specified depth
        public BindToRegionAttribute(string id, BindDepth depth)
        {
            Id = id;
            Depth = depth;
            Type = BindType.IdDepth;
        }

        // binds an action at the specified type
        public BindToRegionAttribute(LocationType type)
        {
            Location = type;
            Type = BindType.Location;
        }

        public BindToRegionAttribute(RegionType type)
        {
            Region = type;
            Type = BindType.Region;
        }

        public BindToRegionAttribute(RegionType type, BindDepth depth)
        {
            Region = type;
            Depth = depth;
            Type = BindType.RegionDepth;
        }

        // binds an action at the specified type and depth
        public BindToRegionAttribute(LocationType type, BindDepth depth)
        {
            Location = type;
            Depth = depth;
            Type = BindType.LocationDepth;
        }

        // binds an action to a specified type of construct
        public BindToRegionAttribute(ConstructType type)
        {
            Construct = type;
            Type = BindType.Construct;
        }

        // binds an action to a specified type of structure
        public BindToRegionAttribute(StructureType type)
        {
            Structure = type;
            Type = BindType.Structure;
        }

        // binds an action to the world at the specified coordinate
        public BindToRegionAttribute(float x, float y)
        {
            X = x;
            Y = y;
            Type = BindType.WorldCoordinate;
        }

        // binds an action to the world at the specified region
        public BindToRegionAttribute(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Type = BindType.WorldPerimeter;
        }

        // binds an action to the world at the specified circle
        public BindToRegionAttribute(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
            Type = BindType.WorldCircle;
        }

        // binds an action to the specified id at the specified coordinate
        public BindToRegionAttribute(string id, float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
            Type = BindType.IdCoordinate;
        }

        // binds an action to the specified id at the specified region
        public BindToRegionAttribute(string id, float x, float y, float width, float height)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Type = BindType.IdPerimeter;
        }

        // binds an action to the specified id at the specified circle
        public BindToRegionAttribute(string id, float x, float y, float radius)
        {
            Id = id;
            X = x;
            Y = y;
            Radius = radius;
            Type = BindType.IdCircle;
        }

        internal BindType Type { get; }

        private string Id { get; }
        private string[] Ids { get; }
        private BindDepth? Depth { get; }
        private LocationType? Location { get; }
        private RegionType? Region { get; }
        private ConstructType? Construct { get; }
        private StructureType? Structure { get; }
        private float X { get; }
        private float Y { get; }
        private float Width { get; }
        private float Height { get; }
        private float Radius { get; }

        // TODO: Use Engine.GetVisibleRegions(Husk husk);

        public bool Judge(Husk husk, HuskBrain brain)
        {
            switch(Type)
            {
                default:
                    return true;
            }
        }

        public bool Judge(string id)
            => throw new NotImplementedException();

        public bool Judge(RegionType region)
            => throw new NotImplementedException();

        public bool Judge(Region region)
            => throw new NotImplementedException();

        public bool Judge(LocationType location)
            => throw new NotImplementedException();

        public bool Judge(Location location)
            => throw new NotImplementedException();

        public bool Judge(ConstructType construct)
            => throw new NotImplementedException();

        public bool Judge(Construct construct)
            => throw new NotImplementedException();

        public bool Judge(StructureType structure)
            => throw new NotImplementedException();

        public bool Judge(Structure structure)
            => throw new NotImplementedException();

        public bool Judge(RegionF region)
            => throw new NotImplementedException();

        public bool Judge(CircleF circle)
            => throw new NotImplementedException();

        public bool Judge(Locator locator)
            => throw new NotImplementedException();

        public bool Judge(string id, RegionF region)
            => throw new NotImplementedException();

        public bool Judge(string id, CircleF circle)
            => throw new NotImplementedException();

    }
}