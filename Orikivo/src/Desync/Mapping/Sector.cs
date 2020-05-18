using Orikivo.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a permanent safe zone in a <see cref="World"/>.
    /// </summary>
    public class Sector : Location
    {
        public const float SmallLength = 64.0f;
        public const float MediumLength = 128.0f;
        public const float LargeLength = 256.0f;

        private Vector2 GetSize(SectorScale scale)
            => scale switch
            {
                SectorScale.Small => new Vector2(SmallLength),
                SectorScale.Medium => new Vector2(MediumLength),
                SectorScale.Large => new Vector2(LargeLength),
                _ => throw new System.ArgumentException("Invalid sector scale specified.")
            };

        private RegionF GetPerimeter(Vector2 position, SectorScale scale)
        {
            Vector2 size = GetSize(scale);
            return new RegionF(position.X, position.Y, size.X, size.Y);
        }

        public Sector(Vector2 position, SectorScale scale)
        {
            Scale = scale;
            Perimeter = GetPerimeter(position, scale);
        }

        public override LocationType Type => LocationType.Sector;

        public override ChildType GetAllowedChildren()
            => ChildType.Area | ChildType.Construct | ChildType.Structure | ChildType.Region;

        public List<Region> Regions { get; set; }

        public List<Route> Routes { get; set; }

        public SectorScale Scale { get; }

        // This is used to determine how to enter the sector.
        public Vector2 Entrance { get; set; }

        public List<Area> Areas { get; set; }

        /// <summary>
        /// Represents the map for this <see cref="Sector"/>.
        /// </summary>
        public Sprite Map { get; set; }

        public List<Construct> Constructs { get; set; }

        // a list of other decoratives in a sector
        // NOTE: structures cannot intersect areas.
        public List<Structure> Structures { get; set; }

        // TODO: Remove references to NPCs.
        // a list of npcs at a specified location.
        public List<(Vector2 Position, Npc Npc)> Npcs { get; set; }

        public Area GetArea(string id)
            => Areas?.FirstOrDefault(x => x.Id == id);

        public Construct GetConstruct(string id)
            => Constructs?.FirstOrDefault(x => x.Id == id);

        public Structure GetStructure(string id)
            => Structures?.FirstOrDefault(x => x.Id == id);

        public override List<Location> GetChildren(bool includeInnerChildren = true)
        {
            var children = new List<Location>();

            if (Areas?.Count > 0)
            {
                children.AddRange(Areas);

                if (includeInnerChildren)
                {
                    foreach (Area area in Areas)
                    {
                        children.AddRange(area.GetChildren());
                    }
                }
            }

            if (Constructs?.Count > 0)
            {
                children.AddRange(Constructs);

                if (includeInnerChildren)
                {
                    foreach(Construct construct in Constructs)
                    {
                        if (construct.Tag == ConstructType.Highrise)
                        {
                            children.AddRange(construct.GetChildren());
                        }
                    }
                }
            }

            return children;
        }

        public override List<Region> GetRegions()
        {
            var regions = new List<Region>();
            regions.AddRange(GetChildren(false));

            if (Regions?.Count > 0)
                regions.AddRange(Regions);

            if (Structures?.Count > 0)
                regions.AddRange(Structures);

            return regions;
        }
    }
}
