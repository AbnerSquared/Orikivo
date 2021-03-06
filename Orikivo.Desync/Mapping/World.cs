﻿using Orikivo.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // All Locations in a World MUST have a unique ID.
    // If there is a location with an identical ID, it will instead be referred by its
    // longitude and latitude.
    /// <summary>
    /// Represents a massive central body of land for an <see cref="Engine"/>.
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

        public List<Route> Routes { get; set; }

        /// <summary>
        /// Represents the size of this <see cref="World"/>.
        /// </summary>
        public Vector2 Perimeter { get; set; }

        /// <summary>
        /// Determines the travel time ratio for this <see cref="World"/>.
        /// </summary>
        public float Scale { get; set; }

        public Sector GetSector(string id)
            => Sectors?.FirstOrDefault(x => x.Id == id);

        public Field GetField(string id)
            => Fields?.FirstOrDefault(x => x.Id == id);

        /// <summary>
        /// Attempts to find the specified <see cref="Location"/>. If no results were found, this returns null.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Location"/> to find.</param>
        public Location Find(string id)
            => GetChildren().FirstOrDefault(x => x.Id == id);

        // gets all locations of a specified type
        public IEnumerable<Location> Filter(LocationType type)
            => GetChildren().Where(x => x.Type.HasFlag(type));

        public IEnumerable<TLocation> Filter<TLocation>()
            where TLocation : Location
            => GetChildren().Where(x => x is TLocation).Select(x => x as TLocation);
            
        internal List<Location> GetChildren(bool includeInnerChildren = true)
        {
            List<Location> locations = new List<Location>();

            if (Fields?.Count > 0)
            {
                locations.AddRange(Fields);

                if (includeInnerChildren)
                {
                    foreach (Field field in Fields)
                    {
                        locations.AddRange(field.GetChildren());
                    }
                }
            }

            if (Sectors?.Count > 0)
            {
                locations.AddRange(Sectors);

                if (includeInnerChildren)
                {
                    foreach (Sector sector in Sectors)
                    {
                        locations.AddRange(sector.GetChildren());
                    }
                }
            }

            return locations;
        }
    }
}
