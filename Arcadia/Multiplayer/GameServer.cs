using System;
using Orikivo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orikivo.Framework;

namespace Arcadia.Multiplayer
{
    public class GameServer
    {
        private readonly GameManager _manager;

        public GameServer(GameManager manager)
        {
            _manager = manager;
            Id = KeyBuilder.Generate(8);
            DisplayChannels = DisplayChannel.GetReservedChannels();
            Players = new List<Player>();
            Connections = new List<ServerConnection>();
        }

        public bool IsFull => Config.IsValidGame() && Players.Count >= GameManager.DetailsOf(Config.GameId).PlayerLimit;

        /// <summary>
        /// Represents the unique identifier for this <see cref="GameServer"/>.
        /// </summary>
        public string Id { get; }

        // all base displays for the game server
        public List<DisplayChannel> DisplayChannels { get; }

        // everyone connected to the lobby
        public List<Player> Players { get; }

        public Player Host => Players.First(x => x.Host);

        // all of the channels that this lobby is connected to
        public List<ServerConnection> Connections { get; set; }

        // This is a list of server invites
        // Duplicate invites cannot be initialized
        // Once a player uses an invite, it is removed from the invitation list
        
        public List<ServerInvite> Invites { get; set; }

        // whenever a message or reaction is sent into any of these channels, attempt to figure out who sent it

        // what are the configurations for this current server?
        public ServerProperties Config { get; set; }

        // what is currently being played in this server, if a session is active? if this is null, there is no active game.
        public GameSession Session { get; set; }

        public ServerConnection GetConnectionFor(ulong channelId)
            => Connections.First(x => x.ChannelId == x.Channel.Id);

        public bool HasConnection(ulong channelId)
            => Connections.Any(x => x.ChannelId == channelId);

        public void GroupAll(string group)
        {
            foreach (ServerConnection connection in Connections)
                connection.Group = group;
        }

        public IEnumerable<ServerConnection> GetGroup(string group)
            => Connections.Where(x => x.Group == group);

        public void UngroupAll()
        {
            foreach (ServerConnection connection in Connections)
                connection.Group = null;
        }

        public DisplayChannel GetDisplayChannel(int frequency)
        {
            foreach (DisplayChannel channel in DisplayChannels)
                if (channel.Frequency == frequency)
                    return channel;

            return null;
        }

        public DisplayChannel GetDisplayChannel(GameState state)
        {
            foreach (DisplayChannel channel in DisplayChannels)
                if (channel.State.HasValue)
                    if (channel.State.Value == state)
                        return channel;

            return null;
        }

        public void SetFrequencyForState(GameState state, int frequency)
        {
            foreach (ServerConnection connection in Connections)
            {
                if (state.HasFlag(connection.State))
                    connection.Frequency = frequency;
            }
        }

        public IEnumerable<ServerConnection> GetConnections(GameState state, ConnectionType type = ConnectionType.Guild)
            => Connections.Where(x => type.HasFlag(x.Type) && state.HasFlag(x.State));

        public IEnumerable<ServerConnection> GetConnectionsInState(GameState state)
            => Connections.Where(x => state.HasFlag(x.State));

        // Updates all connections with the specified state to the new state
        public void ChangeState(GameState previous, GameState current)
        {
            foreach (ServerConnection connection in Connections.Where(x => x.State.HasFlag(previous)))
                connection.State = current;
        }

        public Player GetPlayer(ulong id)
        {
            foreach (Player player in Players)
                if (player.User.Id == id)
                    return player;

            return null;
        }

        // this gets all visible channels a player can see in this server
        public async Task<Dictionary<Player, List<ulong>>> GetPlayerConnectionsAsync()
        {
            var playerConnections = new Dictionary<Player, List<ulong>>();

            foreach(Player player in Players)
            {
                var channelIds = new List<ulong>();

                foreach (ServerConnection connection in Connections)
                {
                    if (await connection.Channel.GetUserAsync(player.User.Id) == null)
                        continue;

                    channelIds.Add(connection.ChannelId);
                }

                playerConnections[player] = channelIds;
            }

            return playerConnections;
        }

        public void EndCurrentSession()
        {
            _manager.EndSession(Session);
            DestroyCurrentSession();
        }

        // this ends the current session a server has active
        public void DestroyCurrentSession()
        {
            foreach (ServerConnection connection in Connections)
            {
                // This might be bad in a synchronous method, so find a better spot for it
                // Handle the deletion of all connections created during a session
                if (connection.Origin == OriginType.Session)
                    connection.DestroyAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                else if (connection.CreatedAt > Session.StartedAt && connection.Origin == OriginType.Unknown)
                    connection.DestroyAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                if ((GameState.Watching | GameState.Playing).HasFlag(connection.State))
                {
                    connection.Frequency = 0;
                    connection.State = GameState.Waiting;
                    connection.Group = null;
                    connection.BlockInput = false;
                }
            }

            Connections.RemoveAll(x => x.Origin == OriginType.Session || x.Origin == OriginType.Unknown && x.CreatedAt > Session.StartedAt);
            // If this display is not a reserved channel, remove it
            DisplayChannels.RemoveAll(x => !x.Reserved);
            Session.DisposeQueue();
            Session = null;

            DisplayContent waiting = GetDisplayChannel(GameState.Waiting).Content;
            DisplayContent editing = GetDisplayChannel(GameState.Editing).Content;

            waiting.GetGroup(LobbyVars.Console).Append("[Console] The current session has ended.");
            editing.GetGroup(LobbyVars.Console).Append("[Console] The current session has ended.");

            waiting.GetComponent(LobbyVars.Console).Draw();
            editing.GetComponent(LobbyVars.Console).Draw(Config.Name);
        }

        // this tells the game manager to update all ServerConnection channels bound to this frequency
        public async Task UpdateAsync()
        {
            DisplayChannel channel = null;

            foreach (ServerConnection connection in Connections)
            {
                Logger.Debug($"{connection.ChannelId} - {connection.State.ToString()}");
                // this way, you don't have to get the same channel again
                channel = connection.State == GameState.Playing ?
                        channel?.Frequency == connection.Frequency ?
                            channel
                            : GetDisplayChannel(connection.Frequency)
                        : GetDisplayChannel(connection.State);

                if (channel == null)
                {
                    await connection.InternalMessage.ModifyAsync($"> ⚠️ Could not find a channel at the specified frequency ({connection.Frequency}).");
                }
                else
                {
                    string content = Check.NotNull(connection.ContentOverride) ? connection.ContentOverride :  channel.Content.ToString();

                    if (connection.InternalMessage == null)
                    {
                        connection.InternalMessage = await connection.Channel.SendMessageAsync(content);
                        connection.MessageId = connection.InternalMessage.Id;
                        continue;
                    }

                    // If the existing message is already equal to the content specified
                    if (connection.InternalMessage.Content == content)
                        continue;

                    await connection.InternalMessage.ModifyAsync(content);
                }
            }

            Console.WriteLine($"[{Orikivo.Format.Time(DateTime.UtcNow)}] Server update was called");
        }
    }
}
