using Newtonsoft.Json;

namespace Orikivo.Desync
{
    // TODO: Use private set handler when used.
    /// <summary>
    /// Represents a location cache.
    /// </summary>
    public class Locator
    {
        public Locator() { }

        [JsonConstructor]
        internal Locator(string worldId, string sectorId, string areaId, string constructId, int? constructLayer)
        {
            WorldId = worldId;
            SectorId = sectorId;
            AreaId = areaId;
            ConstructId = constructId;
            ConstructLayer = constructLayer;
        }

        [JsonProperty("world_id")]
        public string WorldId { get; set; }

        [JsonProperty("field_id")]
        public string FieldId { get; set; }

        [JsonProperty("sector_id")]
        public string SectorId { get; set; }

        [JsonProperty("area_id")]
        public string AreaId { get; set; }

        [JsonProperty("build_id")]
        public string ConstructId { get; set; }

        [JsonProperty("build_layer")]
        public int? ConstructLayer { get; set; }

        /// <summary>
        /// Returns the ID of the location at which the <see cref="Husk"/> is currently at.
        /// </summary>
        public string GetInnerId()
        {
            if (Check.NotNull(FieldId))
                return FieldId;
            else if (Check.NotNull(SectorId))
            {
                if (Check.NotNull(AreaId))
                {
                    if (Check.NotNull(ConstructId))
                        return ConstructId;

                    return AreaId;
                }

                return SectorId;
            }

            throw new System.Exception("There is not an available ID to use.");
        }

        /// <summary>
        /// Returns the type of location at which the <see cref="Husk"/> is currently at.
        /// </summary>
        public LocationType GetInnerType()
        {
            if (Check.NotNull(FieldId))
                return LocationType.Field;
            else if (Check.NotNull(SectorId))
            {
                if (Check.NotNull(AreaId))
                {
                    if (Check.NotNull(ConstructId))
                        return LocationType.Construct;

                    return LocationType.Area;
                }

                return LocationType.Sector;
            }

            throw new System.Exception("There is not an available ID to use.");
        }

        /// <summary>
        /// Returns the name of the location at which the <see cref="Husk"/> is currently at.
        /// </summary>
        public string GetInnerName()
        {
            if (Check.NotNull(FieldId))
                return GetField().Name;
            else if (Check.NotNull(SectorId))
            {
                if (Check.NotNull(AreaId))
                {
                    if (Check.NotNull(ConstructId))
                        return GetConstruct().Name;

                    return GetArea().Name;
                }

                return GetSector().Name;
            }

            throw new System.Exception("There is not an available ID to use.");
        }

        public Field GetField()
        {
            if (!Check.NotNull(FieldId))
                throw new System.Exception("The locator is not currently in a field.");

            return WorldEngine.World.GetField(FieldId);
        }

        public Sector GetSector()
        {
            if (!Check.NotNull(SectorId))
                throw new System.Exception("The Locator is not currently in a sector.");

            return WorldEngine.World.GetSector(SectorId);
        }

        public Construct GetConstruct()
        {
            if (!Check.NotNull(ConstructId))
                throw new System.Exception("The Locator is not currently in a construct.");

            return WorldEngine.World.GetSector(SectorId).GetArea(AreaId).GetConstruct(ConstructId);
        }

        public Area GetArea()
        {
            if (!Check.NotNull(AreaId))
                throw new System.Exception("The Locator is not currently in an area.");

            return WorldEngine.World.GetSector(SectorId).GetArea(AreaId);
        }

        // This can be a FIELD, SECTOR, or WORLD
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s relative x-coordinate in a location.
        /// </summary>
        [JsonProperty("x")]
        public float X { get; set; }

        /// <summary>
        /// Represents a <see cref="Husk"/>'s relative y-coordinate in a location.
        /// </summary>
        [JsonProperty("y")]
        public float Y { get; set; }

        public string GetSummary()
        {
            return WorldEngine.GetLocationSummary(WorldId, SectorId, AreaId, ConstructId, ConstructLayer);
        }
    }
}
