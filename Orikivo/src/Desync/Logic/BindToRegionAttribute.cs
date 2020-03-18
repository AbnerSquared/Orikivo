using System;

namespace Orikivo.Desync
{
    public class BindToRegionAttribute : Attribute
    {
        private enum BindType
        {
            Id = 1,
            Ids = 2,
            Type = 3,
            Region = 4,
            Circle = 5,
            IdRegion = 6,
            IdCircle = 7
        }

        // binds an action to the specified ids
        public BindToRegionAttribute(string id, params string[] rest) {}

        // binds an action to the world at the specified depth
        public BindToRegionAttribute(BindDepth depth) { }

        // binds an action to the specified id at the specified depth
        public BindToRegionAttribute(string id, BindDepth depth) { }

        // binds an action at the specified type
        public BindToRegionAttribute(LocationType type) { }

        public BindToRegionAttribute(RegionType type) { }

        public BindToRegionAttribute(RegionType type, BindDepth depth) { }

        // binds an action at the specified type and depth
        public BindToRegionAttribute(LocationType type, BindDepth depth) { }

        // binds an action to a specified type of construct
        public BindToRegionAttribute(ConstructType type) { }

        // binds an action to a specified type of structure
        public BindToRegionAttribute(StructureType type) { }

        // binds an action to the world at the specified coordinate
        public BindToRegionAttribute(float x, float y) { }

        // binds an action to the world at the specified region
        public BindToRegionAttribute(float x, float y, float width, float height) { }

        // binds an action to the world at the specified circle
        public BindToRegionAttribute(float x, float y, float radius) { }

        // binds an action to the specified id at the specified coordinate
        public BindToRegionAttribute(string id, float x, float y) { }

        // binds an action to the specified id at the specified region
        public BindToRegionAttribute(string id, float x, float y, float width, float height) { }

        // binds an action to the specified id at the specified circle
        public BindToRegionAttribute(string id, float x, float y, float radius) { }

        private BindDepth Depth { get; }
        private LocationType Type { get; }
        private ConstructType Construct { get; }
        private StructureType Structure { get; }
        private string Id { get; }
        private float X { get; }
        private float Y { get; }
        private float Width { get; }
        private float Height { get; }
        private float Radius { get; }
    }
}