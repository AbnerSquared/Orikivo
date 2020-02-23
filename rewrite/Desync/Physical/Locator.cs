using Newtonsoft.Json;

namespace Orikivo.Unstable
{
    // TODO: Use private set handler when used.
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

        public string GetInnerId()
        {
            if (Checks.NotNull(FieldId))
                return FieldId;
            else if (Checks.NotNull(SectorId))
            {
                if (Checks.NotNull(AreaId))
                {
                    if (Checks.NotNull(ConstructId))
                        return ConstructId;

                    return AreaId;
                }

                return SectorId;
            }

            throw new System.Exception("There is not an available ID to use.");
        }

        public LocationType GetInnerType()
        {
            if (Checks.NotNull(FieldId))
                return LocationType.Field;
            else if (Checks.NotNull(SectorId))
            {
                if (Checks.NotNull(AreaId))
                {
                    if (Checks.NotNull(ConstructId))
                        return LocationType.Construct;

                    return LocationType.Area;
                }

                return LocationType.Sector;
            }

            throw new System.Exception("There is not an available ID to use.");
        }

        public string GetInnerName()
        {
            if (Checks.NotNull(FieldId))
                return GetField().Name;
            else if (Checks.NotNull(SectorId))
            {
                if (Checks.NotNull(AreaId))
                {
                    if (Checks.NotNull(ConstructId))
                        return GetConstruct().Name;

                    return GetArea().Name;
                }

                return GetSector().Name;
            }

            throw new System.Exception("There is not an available ID to use.");
        }

        public Field GetField()
        {
            if (!Checks.NotNull(FieldId))
                throw new System.Exception("The locator is not currently in a field.");

            return WorldEngine.World.GetField(FieldId);
        }

        public Sector GetSector()
        {
            if (!Checks.NotNull(SectorId))
                throw new System.Exception("The Locator is not currently in a sector.");

            return WorldEngine.World.GetSector(SectorId);
        }

        public Construct GetConstruct()
        {
            if (!Checks.NotNull(ConstructId))
                throw new System.Exception("The Locator is not currently in a construct.");

            return WorldEngine.World.GetSector(SectorId).GetArea(AreaId).GetConstruct(ConstructId);
        }

        public Area GetArea()
        {
            if (!Checks.NotNull(AreaId))
                throw new System.Exception("The Locator is not currently in an area.");

            return WorldEngine.World.GetSector(SectorId).GetArea(AreaId);
        }

        // This can be a FIELD, SECTOR, or WORLD
        [JsonProperty("id")]
        public string Id { get; set; }

        // Husk.Position => World Position
        // Husk.Location.X => Sector Position
        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("scale")]
        public MapScale Scale { get; set; }
        public string GetSummary()
        {
            return WorldEngine.GetLocationSummary(WorldId, SectorId, AreaId, ConstructId, ConstructLayer);
        }
    }
}
