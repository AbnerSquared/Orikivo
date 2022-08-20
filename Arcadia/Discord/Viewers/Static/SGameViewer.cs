using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Multiplayer;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public static class SGameViewer
    {
        public static readonly int DefaultPageSize = 6;

        public static string ViewGames(ArcadeData data, GameManager gameManager, int page = 0, ArcadeUser user = null)
        {
            string bonusGameId = data.GetOrAssignBonusGame(gameManager);
            var result = new StringBuilder();

            bool allowTooltips = user?.Config?.Tooltips ?? true;

            result.AppendLine(
                $"{Format.Warning($"**{gameManager.GetGame(bonusGameId).Details.Name}** is currently providing bonus experience!")}");

            if (allowTooltips)
            {
                result.AppendLine($"{Format.Tooltip("Type `game <game_id>` to learn more about a game.")}\n");
            }
            else
            {
                result.AppendLine();
            }

            int pageCount = Paginate.GetPageCount(gameManager.Games.Count, DefaultPageSize);
            page = Paginate.ClampIndex(page, pageCount);

            string extra = pageCount > 1 ? $" ({Format.PageCount(page + 1, pageCount)})" : "";


            result.AppendLine($"> **Games**{extra}");

            foreach (GameInfo game in Paginate.GroupAt(gameManager.Games.Values, page, DefaultPageSize))
            {
                string id = game.Details.Name.Equals(game.Id, StringComparison.OrdinalIgnoreCase)
                    ? ""
                    : $"`{game.Id}` ";
                result.AppendLine(
                    $"> {id}{Format.Title(game.Details.Name, game.Details.Icon)} ({(game.Details.RequiredPlayers == game.Details.PlayerLimit ? $"**{game.Details.RequiredPlayers}**" : $"**{game.Details.RequiredPlayers}** to **{game.Details.PlayerLimit}**")} players)");
            }

            return result.ToString();
        }

        public static string ViewGame(GameInfo game, int page = 0, ArcadeUser user = null)
        {
            if (game == null)
                return Format.Warning("An unknown game was specified.");

            bool allowTooltips = user?.Config?.Tooltips ?? true;
            var result = new StringBuilder();

            if (game.Details.CanSpectate)
            {
                result.AppendLine($"{Format.Warning("This game supports spectating.")}");
            }

            if (allowTooltips)
            {
                result.AppendLine($"{Format.Tooltip($"Type `hostserver {game.Id}` to host a server for this game.")}");
            }

            result.AppendLine();
            result.AppendLine($"> {Format.Title(game.Details.Name, game.Details.Icon)} ({(game.Details.RequiredPlayers == game.Details.PlayerLimit ? $"**{game.Details.RequiredPlayers}**" : $"**{game.Details.RequiredPlayers}** to **{game.Details.PlayerLimit}**")} players)");

            if (Check.NotNull(game.Details.Summary))
            {
                result.AppendLine($"> {game.Details.Summary}");
            }

            result.AppendLine();

            if (game.Options.Count > 0)
            {
                int pageCount = Paginate.GetPageCount(game.Options.Count, 5);
                page = Paginate.ClampIndex(page, pageCount);

                string extra = pageCount > 1 ? $" [{Format.PageCount(page + 1, pageCount)}]" : "";
                string title =
                    $"> **Ruleset** (**{game.Options.Count}** {Format.TryPluralize("rule", game.Options.Count)}){extra}\n";

                result.Append(title);

                foreach (GameOption option in Paginate.GroupAt(game.Options, page, 5))
                {
                    result.AppendLine($"\n> `{option.Id}`\n> **{option.Name}** = `{option.Value.ToString()}`");

                    if (Check.NotNull(option.Summary))
                    {
                        result.AppendLine($"> {option.Summary}");
                    }
                }
            }

            return result.ToString();
        }

        public static string ViewInvites(ArcadeUser user, GameManager games, int page = 0)
        {
            var info = new StringBuilder();


            if (games.Servers.Values.Count(x => x.Invites.Any(i => i.UserId == user.Id)) == 0)
            {
                info.AppendLine(Locale.GetHeader(Headers.Invites));
                info.AppendLine("\n> *You have no pending invitations.*");
                return info.ToString();
            }

            IEnumerable<ServerInvite> invites = games.Servers.Values
                .Where(x => x.Invites.Any(i => i.UserId == user.Id))
                .Select(x => x.Invites.Where(i => i.UserId == user.Id))
                .SelectMany(x => x);


            int pageCount = Paginate.GetPageCount(invites.Count(), 10);
            page = Paginate.ClampIndex(page, pageCount);

            string counter = Format.PageCount(page + 1, pageCount, "({0})", false);

            info.AppendLine(Locale.GetHeader(Headers.Invites, Check.NotNull(counter) ? counter : null));

            foreach (ServerInvite invite in Paginate.GroupAt(invites, page, 10))
                info.AppendLine(
                    $"{invite.Header}"); // {(Check.NotNull(invite.Description) ? $"\n> *\"{invite.Description}\"*" : "")}

            return info.ToString();
        }

        public static string View(in IEnumerable<GameServer> servers, int page = 0, int pageSize = 6)
        {
            var browser = new StringBuilder();

            int pageCount = Paginate.GetPageCount(servers.Count(), pageSize);

            page = Paginate.ClampIndex(page, pageCount);
            string extra = Format.PageCount(page + 1, pageCount, "({0})", false);
            string subtitle = servers.Any() ? $"**{servers.Count():##,0}** available {Format.TryPluralize("server", servers.Count())}" : "There aren't any visible game servers to display.";

            browser.AppendLine(Locale.GetHeader(Headers.Browser, extra, subtitle));

            if (!servers.Any())
            {
                // browser.AppendLine("There aren't any visible game servers to display.");
                return browser.ToString();
            }

            foreach (GameServer server in Paginate.GroupAt(servers, page, pageSize))
                browser.AppendLine(ViewServerInfo(server));

            return browser.ToString();
        }

        public static string ViewServerInfo(GameServer server)
        {
            var lobby = new StringBuilder();

            lobby.AppendLine(
                $"\n> {WriteServerName(server.Id, server.Name)} ({WritePlayerCounter(server.Players.Count)})");
            lobby.AppendLine($"> {(Check.NotNull(server.GetGameDetails().Icon) ? $"{server.GetGameDetails().Icon} " : "")}**{WriteGameName(server)}**: {WriteActivity(server)}");
            return lobby.ToString();
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
            return server.GetGameDetails()?.Name ?? "Unknown Game";
        }

        private static string WriteActivity(GameServer server)
        {
            return server.Session != null ? server.Session.ActivityDisplay : "In Lobby";
        }
    }
}
