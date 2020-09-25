using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Multiplayer;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia.Modules
{
    public static class GameViewer
    {
        public static readonly int DefaultPageSize = 6;

        public static string ViewInvites(ArcadeUser user, GameManager games, int page = 0)
        {
            var info = new StringBuilder();

            info.AppendLine($"> 🎲 **Server Invites**");
            info.AppendLine($"> Come and join the party!");

            if (games.Servers.Values.Count(x => x.Invites.Any(i => i.UserId == user.Id)) == 0)
            {
                info.AppendLine("\nYou have no pending invitations.");
                return info.ToString();
            }

            IEnumerable<ServerInvite> invites = games.Servers.Values
                .Where(x => x.Invites.Any(i => i.UserId == user.Id))
                .Select(x => x.Invites.Where(i => i.UserId == user.Id))
                .SelectMany(x => x);

            foreach (ServerInvite invite in Paginate.GroupAt(invites, page, 10))
                info.AppendLine($"{invite.Header}"); // {(Check.NotNull(invite.Description) ? $"\n> *\"{invite.Description}\"*" : "")}

            return info.ToString();
        }

        public static string View(in IEnumerable<GameServer> servers, int page = 0, int pageSize = 6)
        {
            var browser = new StringBuilder();

            int pageCount = Paginate.GetPageCount(servers.Count(), pageSize);
            string extra = pageCount > 1 ? $"({WritePageIndex(page + 1, pageCount)})" : "";

            browser.AppendLine(Locale.GetHeader(Headers.Browser, extra));

            foreach (GameServer server in Paginate.GroupAt(servers, page, pageSize))
                browser.AppendLine(ViewServerInfo(server));

            return browser.ToString();
        }

        public static string ViewServerInfo(GameServer server)
        {
            var lobby = new StringBuilder();

            lobby.AppendLine($"\n> {WriteServerName(server.Id, server.Name)} ({WritePlayerCounter(server.Players.Count)})");
            lobby.AppendLine($"> {WriteGameName(server)}: {WriteActivity(server)}");
            return lobby.ToString();
        }

        private static string WritePageIndex(int page, int pageCount)
        {
            return $"Page {page:##,0}/{pageCount:##,0}";
        }

        private static string WriteServerName(string id, string name)
        {
            return $"`{id}` • **{name}**";
        }

        private static string WritePlayerCounter(int count)
        {
            return $"**{count:##,0}** {Format.TryPluralize("player", count)}";
        }

        private static string WriteGameName(GameServer server)
        {
            return GameManager.DetailsOf(server.GameId)?.Name ?? "Unknown Game";
        }

        private static string WriteActivity(GameServer server)
        {
            return server.Session != null ? server.Session.ActivityDisplay : "In Lobby";
        }
    }
}
