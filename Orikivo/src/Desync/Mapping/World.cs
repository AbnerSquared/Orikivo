using Orikivo.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // All Locations in a World MUST have a unique ID.
    // If there is a location with an identical ID, it will instead be referred by its
    // longitude and latitude.
    /// <summary>
    /// Represents the central land for an <see cref="Engine"/>.
    /// </summary>
    public class World
    {
        public string Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Defines the image reference that is used for the <see cref="Desync.Map"/> in this <see cref="World"/>.
        /// </summary>
        public Sprite Map { get; set; }

        public List<Field> Fields { get; set; }

        public List<Sector> Sectors { get; set; }

        /// <summary>
        /// Represents the size of this <see cref="World"/>.
        /// </summary>
        public Vector2 Boundary { get; set; }

        public List<Barrier> Barriers { get; set; }

        /// <summary>
        /// Determines the travel time ratio for this <see cref="World"/>.
        /// </summary>
        public float Scale { get; set; }

        public Sector GetSector(string id)
        {
            return Sectors.First(x => x.Id == id);
        }

        public Field GetField(string id)
            => Fields.First(x => x.Id == id);

        /// <summary>
        /// Attempts to find the specified <see cref="Location"/>. If no results were found, this returns null.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Location"/> to find.</param>
        public Location Find(string id)
            => GetLocations().FirstOrDefault(x => x.Id == id);

        // gets all locations of a specified type
        public IEnumerable<Location> Select(LocationType type)
            => GetLocations().Where(x => x.Type.HasFlag(type));

        public IEnumerable<TLocation> Select<TLocation>()
            where TLocation : Location
            => GetLocations().Where(x => x is TLocation).Select(x => x as TLocation);

        // attempt to get the parent of a specified location id.
        public Location Backtrack(string id)
            => GetLocations().FirstOrDefault(x => x.Children.Any(y => y.Id == id));

        internal List<Location> GetLocations()
        {
            List<Location> locations = new List<Location>();

            if (Fields?.Count > 0)
            {
                locations.AddRange(Fields);
                
                foreach (Field field in Fields)
                    locations.AddRange(field.Structures);
            }

            if (Sectors?.Count > 0)
            {
                locations.AddRange(Sectors);
                
                foreach (Sector sector in Sectors)
                {
                    locations.AddRange(sector.Areas);
                    locations.AddRange(sector.Structures);
                }
            }

            return locations;
        }
    }
}
