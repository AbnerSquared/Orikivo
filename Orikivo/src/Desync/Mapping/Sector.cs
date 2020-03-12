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

        private string _worldId;

        private Vector2 GetSize(SectorScale scale)
            => scale switch
            {
                SectorScale.Small => new Vector2(SmallLength),
                SectorScale.Medium => new Vector2(MediumLength),
                SectorScale.Large => new Vector2(LargeLength),
                _ => throw new System.ArgumentException("Invalid sector scale specified.")
            };

        private RegionF GetRegion(Vector2 position, SectorScale scale)
        {
            Vector2 size = GetSize(scale);
            return new RegionF(position.X, position.Y, size.X, size.Y);
        }

        public Sector(Vector2 position, SectorScale scale)
        {
            Scale = scale;
            Region = GetRegion(position, scale);
        }

        // TODO: Implement inheritdoc
        /// <inheritdoc/>
        public override LocationType Type => LocationType.Sector;

        public RegionF Region { get; }

        public SectorScale Scale { get; }

        // This is used to determine how to enter the sector.
        public Vector2 Entrance { get; set; }

        public List<Area> Areas { get; set; }


        /// <summary>
        /// Represents the map for this <see cref="Sector"/>.
        /// </summary>
        public Sprite Map { get; set; }

        // a list of other decoratives in a sector
        // NOTE: structures cannot intersect areas.
        public List<Structure> Structures { get; set; }

        // a list of npcs at a specified location.
        public List<(Vector2 Position, Npc Npc)> Npcs { get; set; }

        public Area GetArea(string id)
        {
            return Areas.First(x => x.Id == id);
        }
    }
}
