using Orikivo;

namespace Arcadia
{
    public class ArcadeContainer : BaseContainer<BaseGuild, ArcadeUser>
    {
        public ArcadeContainer()
        {
            Data = JsonHandler.Load<ArcadeData>(@"..\data\global.json") ?? new ArcadeData();

            // This is used to rename and replace all old vars that were stored
            foreach (ArcadeUser user in Users.Values.Values)
                Stats.RenameIds(user);
        }

        public ArcadeData Data { get; }

        public void SaveGlobalData()
        {
            JsonHandler.Save(Data, @"global.json");
        }
    }
}
