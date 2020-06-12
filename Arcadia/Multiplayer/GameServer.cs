using Discord;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcadia
{
    public class GameServer
    {
        public GameServer()
        {
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


        public DisplayChannel GetDisplayChannel(int frequency)
        {
            foreach (DisplayChannel channel in DisplayChannels)
                if (channel.Frequency == frequency)
                    return channel;

            return null;
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
            Dictionary<Player, List<ulong>> playerConnections = new Dictionary<Player, List<ulong>>();

            foreach(Player player in Players)
            {
                List<ulong> channelIds = new List<ulong>();

                foreach (ServerConnection connection in Connections)
                {
                    if (await connection.InternalChannel.GetUserAsync(player.User.Id, CacheMode.AllowDownload) == null)
                        continue;

                    channelIds.Add(connection.ChannelId);
                }

                playerConnections[player] = channelIds;
            }

            return playerConnections;
        }

        // this ends the current session a server has active
        public async Task EndSessionAsync()
        {
            // this resets all channel frequencies to the lobby
            // this sets all players that are playing back to normal
            // this would make the game session null
            // this would close off all possible player channel connections
        }

        // this tells the game manager to update all ServerConnection channels bound to this frequency
        public async Task UpdateAsync()
        {
            DisplayChannel channel = null;
            foreach (ServerConnection connection in Connections)
            {
                // this way, you don't have to get the same channel again
                channel = channel?.Frequency == connection.Frequency ? channel : GetDisplayChannel(connection.Frequency);

                if (channel == null)
                {
                    await connection.InternalMessage.ModifyAsync($"Could not find a channel at the specified frequency ({connection.Frequency}).");
                }
                else
                {
                    await connection.InternalMessage.ModifyAsync(channel.Content.ToString());
                }
            }
        }

    }
}
