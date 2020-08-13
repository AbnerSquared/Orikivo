using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Multiplayer;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia.Modules
{
    public static class ServerBrowser
    {
        // Make sure to include servers from which the user was invited to.
        public static string View(in IEnumerable<GameServer> servers, int page = 0, int pageSize = 6)
        {
            var browser = new StringBuilder();

            browser.Append($"> 🔍 **Server Browser**");

            int pageCount = Paginate.GetPageCount(servers.Count(), pageSize);

            if (pageCount > 1)
                browser.Append($" (Page {page + 1:##,0}/{pageCount:##,0})");

            browser.AppendLine();
            browser.AppendLine("> View all of the public game servers.");

            foreach (GameServer server in Paginate.GroupAt(servers, page, pageSize))
                browser.AppendLine(WriteServerInfo(server));

            return browser.ToString();
        }

        public static string WriteServerInfo(GameServer server)
        {
            var lobby = new StringBuilder();

            lobby.AppendLine($"\n> `{server.Id}` • **{server.Name}** (**{server.Players.Count:##,0}** {Format.TryPluralize("player", server.Players.Count)})");
            lobby.AppendLine($"> {WriteGameName(server)}: {WriteActivity(server)}");
            return lobby.ToString();
        }

        private static string WriteGameName(GameServer server)
        {
            return GameManager.Games.ContainsKey(server.GameId) ? GameManager.DetailsOf(server.GameId).Name : "Unknown Game";
        }

        private static string WriteActivity(GameServer server)
        {
            return server.Session != null ? server.Session.ActivityDisplay : "In Lobby";
        }
    }
}