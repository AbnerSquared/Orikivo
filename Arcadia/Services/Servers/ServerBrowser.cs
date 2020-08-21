using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Multiplayer;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia.Modules
{
    public static class MessageFormat
    {
        // > {icon} **{header}** {extra}\n> {subtitle}
        public static string Header(string text, string icon = "", string subtitle = "", string extra = "")
        {
            if (!Check.NotNull(text))
                throw new ArgumentException("Expected header text to have a specified value");

            var header = new StringBuilder("> ");

            if (Check.NotNull(icon))
                header.Append($"{icon} ");

            header.Append($"**{text}**");

            if (Check.NotNull(extra))
                header.Append($" {extra}");


            if (!Check.NotNull(subtitle))
                header.Append($"\n> {subtitle}");

            return header.ToString();
        }
    }

    public static class ServerBrowser
    {
        private static readonly string BrowserHeader = "> 🔍 **Server Browser**";

        // Make sure to include servers from which the user was invited to.
        public static string View(in IEnumerable<GameServer> servers, int page = 0, int pageSize = 6)
        {
            var browser = new StringBuilder();

            int pageCount = Paginate.GetPageCount(servers.Count(), pageSize);
            string extra = pageCount > 1 ? $"(Page {page + 1:##,0}/{pageCount:##,0})" : "";

            browser.AppendLine(MessageFormat.Header("Server Browser", "🔍", "View all of the public game servers.", extra));

            foreach (GameServer server in Paginate.GroupAt(servers, page, pageSize))
                browser.AppendLine(WriteServerInfo(server));

            return browser.ToString();
        }

        // counter:
        // **{0}** {1}
        // **{number:##,0}** {Format.TryPluralize("noun", number)}

        // > `{0}` • **{1}** ({2})\n> {3}: {4}
        // > `{id}` • **{name}** ({counter})\n> {game}: {activity}
        public static string WriteServerInfo(GameServer server)
        {
            var lobby = new StringBuilder();

            lobby.AppendLine($"\n> `{server.Id}` • **{server.Name}** (**{server.Players.Count:##,0}** {Format.TryPluralize("player", server.Players.Count)})");
            lobby.AppendLine($"> {WriteGameName(server)}: {WriteActivity(server)}");
            return lobby.ToString();
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