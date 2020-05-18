using Orikivo.Drawing;
using Orikivo.Drawing.Graphics2D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Defines a location-based precondition that an action is bound to when true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class BindToRegionAttribute : Attribute
    {
        /// <summary>
        /// Binds an action to an array of <see cref="Desync.Region"/> references.
        /// </summary>
        /// <param name="id">The primary ID that represents a <see cref="Desync.Location"/> reference.</param>
        /// <param name="rest">The rest of the IDs, if any.</param>
        public BindToRegionAttribute(string id, params string[] rest)
        {
            Ids = rest.Prepend(id);
        }

        /// <summary>
        /// Binds an action to a <see cref="Desync.Region"/> reference at the specified depth.
        /// </summary>
        /// <param name="id">The primary ID that represents a <see cref="Desync.Location"/> reference.</param>
        /// <param name="depth">The depth at which this action is allowed, if any.</param>
        public BindToRegionAttribute(string id, BindDepth depth = BindDepth.At)
        {
            Id = id;
            Depth = depth;
        }

        /// <summary>
        /// Binds an action to a specified <see cref="RegionType"/>.
        /// </summary>
        /// <param name="type">The type of region that allows for this action.</param>
        /// <param name="depth">The depth at which this action is allowed, if any.</param>
        public BindToRegionAttribute(RegionType type, BindDepth depth = BindDepth.At)
        {
            Region = type;
            Depth = depth;
        }

        /// <summary>
        /// Binds an action to a specified <see cref="LocationType"/>.
        /// </summary>
        /// <param name="type">The type of location that allows for this action.</param>
        /// <param name="depth">The depth at which this action is allowed, if any.</param>
        public BindToRegionAttribute(LocationType type, BindDepth depth = BindDepth.At)
        {
            Region = RegionType.Location;
            Location = type;
            Depth = depth;
        }

        /// <summary>
        /// Binds an action to a specified <see cref="ConstructType"/>.
        /// </summary>
        /// <param name="type">The type of construct that allows for this action.</param>
        public BindToRegionAttribute(ConstructType type)
        {
            Region = RegionType.Location;
            Location = LocationType.Construct;
            Construct = type;
        }

        /// <summary>
        /// Binds an action to a specified <see cref="StructureType"/>.
        /// </summary>
        /// <param name="type">The type of structure that allows for this action.</param>
        public BindToRegionAttribute(StructureType type, InteractionType interaction = InteractionType.View)
        {
            Region = RegionType.Structure;
            Structure = type;
            Interaction = interaction;
        }

        /// <summary>
        /// Binds an action to a specified coordinate in a <see cref="World"/>.
        /// </summary>
        /// <param name="x">The x-position of this coordinate.</param>
        /// <param name="y">The y-position of this coordinate.</param>
        public BindToRegionAttribute(float x, float y, InteractionType interaction = InteractionType.View)
        {
            X = x;
            Y = y;
            Interaction = interaction;
        }

        /// <summary>
        /// Binds an action to a specified perimeter in a <see cref="World"/>.
        /// </summary>
        /// <param name="x">The x-position of the top-left point for this perimeter.</param>
        /// <param name="y">The y-position of the top-left point for this perimeter.</param>
        /// <param name="width">The width of this perimeter.</param>
        /// <param name="height">The height of this perimeter.</param>
        public BindToRegionAttribute(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Binds an action to a specified circle in a <see cref="World"/>.
        /// </summary>
        /// <param name="x">The x-position for the origin of the circle.</param>
        /// <param name="y">The y-position for the origin of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public BindToRegionAttribute(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        // binds an action to the specified id at the specified coordinate
        public BindToRegionAttribute(string id, float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Binds an action to a specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BindToRegionAttribute(string id, float x, float y, float width, float height)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        // binds an action to the specified id at the specified circle
        public BindToRegionAttribute(string id, float x, float y, float radius)
        {
            Id = id;
            X = x;
            Y = y;
            Radius = radius;
        }

        private string Id { get; }

        // is the user in any location that matches an ID here

        private IEnumerable<string> Ids { get; }

        // if a depth is set, is the user, in the location's id
        // or is the user AT the location's id?
        private BindDepth Depth { get; }

        // is the region the user in match this?
        private RegionType? Region { get; }

        // is the location the user in match this
        private LocationType? Location { get; }

        // is the location a construct AND
        // the construct the user is in match this
        private ConstructType? Construct { get; }

        // is the user within a structure's region AND
        // is the structure they are near match this
        private StructureType? Structure { get; }

        // does the user need to see the location or be able to touch the location?
        private InteractionType Interaction { get; }


        // point, perimeter, or circle
        private float? X { get; }
        private float? Y { get; }

        // perimeter
        private float Width { get; }
        private float Height { get; }

        // circle
        private float Radius { get; }

        // TODO: Use Engine.GetVisibleRegions(Husk husk);

        public bool Judge(Husk husk)
        {
            if (Ids?.Count() > 0)
            {
                return Ids.Contains(husk.Location.Id);
            }

            if (Region.HasValue)
            {
                Location location = husk.Location.GetLocation();

                if (Structure.HasValue)
                {
                    var results = location.Filter(Structure.Value);

                    if (results.Any(x => (Interaction == InteractionType.View ? husk.Hitbox.Sight : husk.Hitbox.Reach)
                    .Intersects(x.Perimeter)))
                        return true;
                }

                if (Location.HasValue)
                {
                    if (Construct.HasValue)
                    {
                        if (!Construct.Value.HasFlag((location as Construct).Tag))
                            return false;
                    }

                    if (!Location.Value.HasFlag(location.Type))
                        return false;
                }

                if (!Region.Value.HasFlag(location.Subtype))
                    return false;
            }

            if (X.HasValue && Y.HasValue)
            {
                float x = husk.Location.X;
                float y = husk.Location.Y;

                if (Check.NotNull(Id))
                {
                    if (Id != husk.Location.Id)
                        return false;
                }

                if (Width > 0 && Height > 0)
                {
                    return RegionF.Contains(X.Value, Y.Value, Width, Height, x, y);
                }

                if (Radius > 0)
                {
                    return CircleF.Contains(X.Value, Y.Value, Radius, x, y);
                }

                return (Interaction == InteractionType.View ? husk.Hitbox.Sight : husk.Hitbox.Reach)
                    .Contains(X.Value, Y.Value);
            }

            if (Check.NotNull(Id))
            {
                if (Depth == BindDepth.In)
                {
                    Location location = Engine.World.Find(Id);

                    return location.GetChildren().Any(x => x.Id == husk.Location.Id);
                }

                return Id == husk.Location.Id;
            }

            Console.WriteLine("No eligible regions were matched.");
            return false;
        }
    }
}