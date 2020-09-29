using Orikivo;

namespace Arcadia
{
    public class ArcadeContainer
    {
        public ArcadeContainer()
        {
            Users = new JsonContainer<ArcadeUser>(@"..\data\users\");
            Guilds = new JsonContainer<BaseGuild>(@"..\data\guilds\");
            Data = JsonHandler.Load<ArcadeData>(@"..\data\global.json") ?? new ArcadeData();

            // This is used to rename and replace all old vars that were stored
            foreach (ArcadeUser user in Users.Values.Values)
                Stats.RenameIds(user);
        }

        public JsonContainer<ArcadeUser> Users { get; }
        public JsonContainer<BaseGuild> Guilds { get; }
        public ArcadeData Data { get; }

        public void SaveGlobalData()
        {
            JsonHandler.Save(Data, @"global.json");
        }
    }
}
