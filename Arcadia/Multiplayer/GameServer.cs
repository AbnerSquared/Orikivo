using Discord;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    public class GameServer
    {
        // i'm not a fan of requiring the game manager, find another way to end a session
        private readonly GameManager _manager;

        public GameServer(GameManager manager)
        {
            _manager = manager;
            Id = KeyBuilder.Generate(8);
            DisplayChannels = new List<DisplayChannel>();
            DisplayChannels.AddRange(DisplayChannel.GetReservedChannels());
            Players = new List<Player>();
            Connections = new List<ServerConnection>();
        }

        // the unique id of this lobby
        public string Id;

        // all base displays for the game server
        public List<DisplayChannel> DisplayChannels { get; set; }

        // everyone connected to the lobby
        public List<Player> Players { get; set; }

        public Player Host => Players.First(x => x.Host);

        // all of the channels that this lobby is connected to
        public List<ServerConnection> Connections { get; set; }

        // whenever a message or reaction is sent into any of these channels, attempt to figure out who sent it

        // what are the configurations for this current server?
        public GameServerConfig Config { get; set; }

        // what is currently being played in this server, if a session is active? if this is null, there is no active game.
        public GameSession Session { get; set; }

        public ServerConnection GetConnectionFor(ulong channelId)
            => Connections.First(x => x.ChannelId == x.Channel.Id);

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

        // This gets all of the connections pointing at a user
        public IEnumerable<ServerConnection> GetDirectConnections()
            => Connections.Where(x => x.Type == ConnectionType.Direct);

        public IEnumerable<ServerConnection> GetConnections(GameState state, ConnectionType type = ConnectionType.Guild)
            => Connections.Where(x => type.HasFlag(x.Type) && state.HasFlag(x.State));

        public IEnumerable<ServerConnection> GetConnectionsInState(GameState state)
            => Connections.Where(x => state.HasFlag(x.State));

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
            Dictionary<Player, List<ulong>> playerConnections = new Dictionary<Player, List<ulong>>();

            foreach(Player player in Players)
            {
                List<ulong> channelIds = new List<ulong>();

                foreach (ServerConnection connection in Connections)
                {
                    if (await connection.Channel.GetUserAsync(player.User.Id, CacheMode.AllowDownload) == null)
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
                if ((GameState.Watching | GameState.Playing).HasFlag(connection.State))
                {
                    connection.Frequency = 0;
                    connection.State = GameState.Waiting;
                }
            }

            Session.CancelAllTimers();
            Session = null;

            DisplayContent waiting = GetDisplayChannel(GameState.Waiting).Content;
            DisplayContent editing = GetDisplayChannel(GameState.Editing).Content;

            (waiting.GetComponent("message_box") as ComponentGroup).Append("[Console] The current session has ended.");
            (editing.GetComponent("message_box") as ComponentGroup).Append("[Console] The current session has ended.");

            waiting.GetComponent("message_box").Draw();
            editing.GetComponent("message_box").Draw(Config.Title);
        }

        // this tells the game manager to update all ServerConnection channels bound to this frequency
        public async Task UpdateAsync()
        {
            DisplayChannel channel = null;
            foreach (ServerConnection connection in Connections)
            {
                Console.WriteLine($"{connection.ChannelId} - {connection.State.ToString()}");
                // this way, you don't have to get the same channel again
                channel = connection.State == GameState.Playing ?
                    channel?.Frequency == connection.Frequency ? channel : GetDisplayChannel(connection.Frequency)
                    : GetDisplayChannel(connection.State);

                if (channel == null)
                {
                    await connection.InternalMessage.ModifyAsync($"Could not find a channel at the specified frequency ({connection.Frequency}).");
                }
                else
                {
                    string content = channel.Content.ToString();

                    if (connection.InternalMessage == null)
                    {
                        connection.InternalMessage = await connection.Channel.SendMessageAsync(content);
                        connection.MessageId = connection.InternalMessage.Id;
                        continue;
                    }

                    if (connection.InternalMessage.Content == content)
                        continue;

                    await connection.InternalMessage.ModifyAsync(content);
                }
            }
        }

    }
}
