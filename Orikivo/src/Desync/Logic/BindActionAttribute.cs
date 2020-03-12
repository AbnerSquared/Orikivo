using System;

namespace Orikivo.Desync
{
    public class BindActionAttribute : Attribute
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
        public BindActionAttribute(string id, params string[] rest) {}

        // binds an action to the world at the specified depth
        public BindActionAttribute(BindDepth depth) { }

        // binds an action to the specified id at the specified depth
        public BindActionAttribute(string id, BindDepth depth) { }

        // binds an action at the specified type
        public BindActionAttribute(LocationType type) { }

        // binds an action at the specified type and depth
        public BindActionAttribute(LocationType type, BindDepth depth) { }

        // binds an action to a specified type of construct
        public BindActionAttribute(ConstructType type) { }

        // binds an action to a specified type of structure
        public BindActionAttribute(StructureType type) { }

        // binds an action to the world at the specified coordinate
        public BindActionAttribute(float x, float y) { }

        // binds an action to the world at the specified region
        public BindActionAttribute(float x, float y, float width, float height) { }

        // binds an action to the world at the specified circle
        public BindActionAttribute(float x, float y, float radius) { }

        // binds an action to the specified id at the specified coordinate
        public BindActionAttribute(string id, float x, float y) { }

        // binds an action to the specified id at the specified region
        public BindActionAttribute(string id, float x, float y, float width, float height) { }

        // binds an action to the specified id at the specified circle
        public BindActionAttribute(string id, float x, float y, float radius) { }

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