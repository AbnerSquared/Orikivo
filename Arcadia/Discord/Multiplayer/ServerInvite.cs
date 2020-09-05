using System.Text;
using Orikivo;

namespace Arcadia.Multiplayer
{
    public class ServerInvite
    {
        public static string Write(ServerInvite invite, GameServer server)
        {
            var info = new StringBuilder();

            info.AppendLine($"> 🎲 You have been invited to join a server!");

            invite.Header = $"> `{server.Id}` • **{server.Name}**";
            info.AppendLine($"> `{server.Id}` • **{server.Name}**");



            if (Check.NotNull(invite.Description))
                info.AppendLine($"> *\"{invite.Description}\"*");

            return info.ToString();
        }

        public ServerInvite(ulong userId, string description)
        {
            UserId = userId;
            Description = description;
        }

        // who is available to join this game lobby?
        public ulong UserId { get; }

        // What does the invite say?
        public string Description { get; }

        internal string Header { get; set; }
    }
}
