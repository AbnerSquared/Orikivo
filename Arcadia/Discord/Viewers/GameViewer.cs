using System;
using Arcadia.Multiplayer;

namespace Arcadia
{
    public class GameViewer : IViewer<GameInfo>
    {
<<<<<<< Updated upstream
        public static readonly int DefaultPageSize = 6;

        public static string ViewGames(ArcadeData data, GameManager gameManager, int page = 0, ArcadeUser user = null)
        {
            string bonusGameId = data.GetOrAssignBonusGame(gameManager);
            var result = new StringBuilder();

            bool allowTooltips = user?.Config?.Tooltips ?? true;

            result.AppendLine($"{Format.Warning($"**{gameManager.GetGame(bonusGameId).Details.Name}** is currently providing bonus experience!")}");

            if (allowTooltips)
            {
                result.AppendLine($"{Format.Tooltip("Type `game <game_id>` to learn more about a game.")}\n");
            }
            else
            {
                result.AppendLine();
            }

            int pageCount = Paginate.GetPageCount(gameManager.Games.Count, 5);
            page = Paginate.ClampIndex(page, pageCount);

            string extra = pageCount > 1 ? $" ({Format.PageCount(page + 1, pageCount)})" : "";


            result.AppendLine($"> **Games**{extra}");

            foreach (GameInfo game in Paginate.GroupAt(gameManager.Games.Values, page, 8))
            {
                string id = game.Details.Name.Equals(game.Id, StringComparison.OrdinalIgnoreCase) ? "" : $"`{game.Id}` ";
                result.AppendLine($"> {id}{Format.Title(game.Details.Name, game.Details.Icon)} ({(game.Details.RequiredPlayers == game.Details.PlayerLimit ? $"**{game.Details.RequiredPlayers}**" : $"**{game.Details.RequiredPlayers}** to **{game.Details.PlayerLimit}**")} players)");
            }

            return result.ToString();
        }

        public static string ViewGame(GameInfo game, int page = 0, ArcadeUser user = null)
        {
            bool allowTooltips = user?.Config?.Tooltips ?? true;

            if (game == null)
                return Format.Warning("An unknown game was specified.");

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
                string title = $"> **Ruleset** (**{game.Options.Count}** {Format.TryPluralize("rule", game.Options.Count)}){extra}\n";

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
            info.AppendLine(Locale.GetHeader(Headers.Invites));

            if (games.Servers.Values.Count(x => x.Invites.Any(i => i.UserId == user.Id)) == 0)
            {
                info.AppendLine("\n> *You have no pending invitations.*");
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
=======
        public string ViewDefault(ArcadeUser invoker)
>>>>>>> Stashed changes
        {
            throw new NotImplementedException();
        }

        public string View(ArcadeUser invoker, string query, int page)
        {
            throw new NotImplementedException();
        }

        public string ViewSingle(ArcadeUser invoker, GameInfo game)
        {
            throw new NotImplementedException();
        }

        public string PreviewSingle(ArcadeUser invoker, GameInfo game)
        {
            throw new NotImplementedException();
        }
    }
}