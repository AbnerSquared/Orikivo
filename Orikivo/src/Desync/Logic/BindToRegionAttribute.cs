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
        private enum BindType
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
            Bind = BindType.Ids;
        }

        // binds an action to the world at the specified depth
        public BindToRegionAttribute(BindDepth depth)
        {
            Bind = BindType.WorldDepth;
        }

        // binds an action to the specified id at the specified depth
        public BindToRegionAttribute(string id, BindDepth depth)
        {
            Bind = BindType.IdDepth;
        }

        // binds an action at the specified type
        public BindToRegionAttribute(LocationType type)
        {
            Bind = BindType.Location;
        }

        public BindToRegionAttribute(RegionType type)
        {
            Bind = BindType.Region;
        }

        public BindToRegionAttribute(RegionType type, BindDepth depth)
        {
            Bind = BindType.RegionDepth;
        }

        // binds an action at the specified type and depth
        public BindToRegionAttribute(LocationType type, BindDepth depth)
        {
            Bind = BindType.LocationDepth;
        }

        // binds an action to a specified type of construct
        public BindToRegionAttribute(ConstructType type)
        {
            Bind = BindType.Construct;
        }

        // binds an action to a specified type of structure
        public BindToRegionAttribute(StructureType type)
        {
            Bind = BindType.Structure;
        }

        // binds an action to the world at the specified coordinate
        public BindToRegionAttribute(float x, float y)
        {
            Bind = BindType.WorldCoordinate;
        }

        // binds an action to the world at the specified region
        public BindToRegionAttribute(float x, float y, float width, float height)
        {
            Bind = BindType.WorldPerimeter;
        }

        // binds an action to the world at the specified circle
        public BindToRegionAttribute(float x, float y, float radius)
        {
            Bind = BindType.WorldCircle;
        }

        // binds an action to the specified id at the specified coordinate
        public BindToRegionAttribute(string id, float x, float y)
        {
            Bind = BindType.IdCoordinate;
        }

        // binds an action to the specified id at the specified region
        public BindToRegionAttribute(string id, float x, float y, float width, float height)
        {
            Bind = BindType.IdPerimeter;
        }

        // binds an action to the specified id at the specified circle
        public BindToRegionAttribute(string id, float x, float y, float radius)
        {
            Bind = BindType.IdCircle;
        }

        private BindType Bind { get; }

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