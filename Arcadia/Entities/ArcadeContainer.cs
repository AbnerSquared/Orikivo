using System.Collections.Generic;
using Newtonsoft.Json;
using Orikivo;
using Orikivo.Desync;

namespace Arcadia
{
    public class ArcadeData
    {
        public ArcadeData()
        {
            Catalogs = new Dictionary<string, ItemCatalogData>();
        }

        [JsonConstructor]
        internal ArcadeData(Dictionary<string, ItemCatalogData> catalogs)
        {
            Catalogs = catalogs ?? new Dictionary<string, ItemCatalogData>();
        }

        [JsonProperty("catalogs")]
        public Dictionary<string, ItemCatalogData> Catalogs { get; }
    }


    public class ArcadeContainer
    {
        public ArcadeContainer()
        {
            Users = new JsonContainer<ArcadeUser>(@"..\data\users\");
            Guilds = new JsonContainer<BaseGuild>(@"..\data\guilds\");
            Data = JsonHandler.Load<ArcadeData>(@"..\data\global.json");
        }

        public JsonContainer<ArcadeUser> Users { get; }
        public JsonContainer<BaseGuild> Guilds { get; }

        public ArcadeData Data { get; }
    }
}
