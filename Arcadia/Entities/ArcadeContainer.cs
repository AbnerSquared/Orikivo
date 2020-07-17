using Orikivo;
using Orikivo.Desync;

namespace Arcadia
{
    public class ArcadeContainer
    {
        public ArcadeContainer()
        {
            Users = new JsonContainer<ArcadeUser>(@"..\data\users\");
            Guilds = new JsonContainer<BaseGuild>(@"..\data\guilds\");
        }

        public JsonContainer<ArcadeUser> Users { get; }
        public JsonContainer<BaseGuild> Guilds { get; }
    }
}
