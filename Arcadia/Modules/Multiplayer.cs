using Discord.Commands;
using Orikivo;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia
{
    [Name("Multiplayer")]
    [Summary("Come play with others.")]
    public class Multiplayer : OriModuleBase<OriCommandContext>
    {
        private readonly GameManager _games;
        private readonly Old.GameManager _gameManager;

        public Multiplayer(GameManager games, Old.GameManager gameManager)
        {
            _games = games;
            _gameManager = gameManager;
        }

        [Command("servers")]
        [Summary("View all currently open game servers.")]
        public async Task ViewServersAsync()
        {
            if (_games.Servers.Count == 0)
            {
                await Context.Channel.SendMessageAsync("There aren't any public game servers to show.");
                return;
            }

            var servers = new StringBuilder();
            foreach ((string key, GameServer server) in _games.Servers)
            {
                servers.AppendLine($"{server.Id} | {server.Config.Title} ({server.Players.Count} {OriFormat.TryPluralize("player", server.Players.Count)})");
            }

            await Context.Channel.SendMessageAsync(servers.ToString());
        }

        [Command("hostserver")]
        [Summary("Host a new game server.")]
        public async Task HostServerAsync()
        {
            if (_games.ReservedUsers.ContainsKey(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync("You are already in a server. You must leave the current one before you can host.");
                return;
            }

            await _games.CreateServerAsync(Context.User, Context.Channel);
        }

        [Command("joinserver")]
        [Summary("Join an existing game server.")]
        public async Task JoinServerAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                await Context.Channel.SendMessageAsync("You have to specify an ID.");
                return;
            }

            await _games.JoinServerAsync(Context.User, Context.Channel, id);
        }

        /*
        [Command("leaveserver")]
        [Summary("Leave the current server you are in.")]
        public async Task LeaveServerAsync()
        {

        }*/

        [Access(AccessLevel.Dev)]
        [Command("destroyserver")]
        [Summary("Destroys the specified server.")]
        public async Task DestroyServerAsync(string id)
        {
            if (_games.Servers.ContainsKey(id))
            {
                await _games.DestroyServerAsync(_games.Servers[id]);
                await Context.Channel.SendMessageAsync("The specified server has been destroyed.");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Could not find the specified server.");
            }
        }
        /*
        [RequireUser]
        [Command("games")]
        [Summary("Returns a list of all visible **Games**.")]
        public async Task ShowLobbiesAsync([Summary("The page index for the list.")]int page = 1) // utilize a paginator.
            => await Context.Channel.SendMessageAsync(_gameManager.IsEmpty ? $"> **Looks like there's nothing here.**"
                : string.Join('\n', _gameManager.Games.Values.Select(x => x.ToString())));
        

        
        [RequireUser]
        [Command("joingame"), Alias("jg")]
        [Summary("Join an open **Lobby**.")]
        public async Task JoinLobbyAsync([Summary("A string pointing to a specific **Game**.")]string id)
        {
            Old.Game game = _gameManager[id];
            if (game == null)
                await Context.Channel.SendMessageAsync(_gameManager.ContainsUser(Context.User.Id) ?
                    "**Wait a minute...**\n> You are already in a game." : $"**No luck.**\n> I couldn't find any games matching #**{id}**.");
            else
            {
                if (game.ContainsUser(Context.User.Id))
                    await Context.Channel.SendMessageAsync($"**???**\n> You are already in this game.");
                else
                {
                    await _gameManager.AddUserAsync(Context, id);
                    await Context.Channel.SendMessageAsync($"**Success!**\n> You have joined {game.Lobby.Name}. [{game.Receivers.First(x => x.Id == Context.Guild.Id).Mention}]");
                }
            }
        }
        

        
    [Command("creategame"), Alias("crg")]
    [Summary("Create a **Game**.")]
    [RequireUser]
    public async Task StartLobbyAsync([Summary("The **GameMode** to play within the **Game**.")]Old.GameMode mode)
    {
        if (_gameManager.ContainsUser(Context.Account.Id))
        {
            await Context.Channel.SendMessageAsync($"**Wait a minute...**\n> You are already in a game.");
            return;
        }
        try
        {
            Old.Game game = await _gameManager.CreateGameAsync(Context, new Old.GameConfig(mode, $"{Context.User.Username}'s Lobby")).ConfigureAwait(false);
            await Context.Channel.SendMessageAsync($"**Success!**\n> {game.Lobby.Name} has been created. [{game.Receivers[0].Mention}]");
            await _gameManager.StartGameSessionAsync(game.Id);
        }
        catch (Exception ex)
        {
            await Context.Channel.CatchAsync(ex);
        }
    }

        
         //[Command("charfill")]
        public async Task CharFillAsync(char fill, char empty, int width, float percent)
        {
            CharFill filler = new CharFill { FillChar = fill, EmptyChar = empty, Width = width, Percent = percent };
            int filled = (int)MathF.Floor(RangeF.Convert(0.0f, 1.0f, 0, width, percent));

            StringBuilder sb = new StringBuilder();

            sb.Append(filler.FillChar, filled);
            sb.Append(filler.EmptyChar, filler.Width - filled);

            await Context.Channel.SendMessageAsync(sb.ToString());
        }
         
        //[Command("wolfnode")]
        public async Task CreateWolfNodeAsync(string sessionName, string sessionId, string privacy, string gameMode, string password = null)
        {
            WerewolfInfoNode node = new WerewolfInfoNode { SessionName = sessionName,
            SessionId = sessionId, Privacy = privacy, GameMode = gameMode, Password = password };

            await Context.Channel.SendMessageAsync(node.ToString());
        }

        //[Command("consolenode")]
        public async Task CreateConsoleNodeAsync(string sessionName, string sessionId, string privacy, string gameMode, string message, string author = null)
        {
            WerewolfInfoNode node = new WerewolfInfoNode { SessionName = sessionName,
            SessionId = sessionId, Privacy = privacy, GameMode = gameMode,
            Messages = new List<MessageNode> {
                new MessageNode { Author = "Orikivo", Message = "vroom" },
                new MessageNode { Message = message, Author = author } }
            };

            await Context.Channel.SendMessageAsync(node.ToString());
        }
        
        */
    }
}