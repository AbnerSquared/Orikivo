using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia
{
    public class DisplayChannel
    {
        // this is an array of reserved frequencies
        // these channels are always paired into a game server
        public static int[] ReservedFrequencies => new int[2] { -1, 0 };

        internal DisplayChannel() { }

        public DisplayChannel(int frequency, DisplayContent content, TimeSpan? refreshRate = null)
        {
            if (ReservedFrequencies.Contains(frequency))
                throw new Exception("This frequency is reserved.");

            Frequency = frequency;
            RefreshRate = refreshRate ?? TimeSpan.FromSeconds(1);
            Content = content;

            Inputs = new List<IInput>();
        }

        public static List<DisplayChannel> GetReservedChannels()
        {
            var content = new DisplayContent();
            
            
            
            var inputs = new List<IInput>();

            inputs.Add(new TextInput
            {
                Name = "leave",
                OnExecute = delegate (Player player, ServerConnection connection, GameServer server)
                {
                    server.Players.Remove(player);
                }
            });

            inputs.Add(new TextInput
            {
                Name = "join",
                OnExecute = async delegate (Player player, ServerConnection connection, GameServer server)
                {
                    if (server.GetPlayer(player.User.Id) != null)
                    {
                        IDMChannel dm = await player.User.GetOrCreateDMChannelAsync();
                        // this should instead be updated to the display content
                        await dm.SendMessageAsync("You are already in this server!");
                        return;
                    }

                    // likewise, this should update the player list component on the display channel
                    server.Players.Add(player);
                }
            });

            // commands in the lobby:

            // config: opens the config panel
            // for this command, this sets the frequency of the channel of where this was executed to the config channel (-1)

            // join: allows you to join the lobby
            // leave: leaves the lobby

            var channels = new List<DisplayChannel>();
            
            // reserved lobby channel
            channels.Add(new DisplayChannel
            {
                Frequency = 0,
                Content = content,
                Inputs = inputs,
                RefreshRate = TimeSpan.FromSeconds(1)
            });

            // reserved configurations channel
            /*
            channels.Add(new DisplayChannel
            {
                Frequency = -1,
                Content = content,
                Inputs = inputs,
                RefreshRate = TimeSpan.FromSeconds(1)
            });*/

            return channels;
        }

        // what frequency is this display broadcasting to?
        public int Frequency; // default is 0, the lobby.
        // the lobby is always there

        // how fast can this display update? minimum of 1 second.
        public TimeSpan RefreshRate;

        // what is this display currently showing?
        public DisplayContent Content;

        // what can the player currently do in this display channel? (reaction or text)
        public List<IInput> Inputs;

        public override string ToString()
            => Content.ToString();
    }
}
