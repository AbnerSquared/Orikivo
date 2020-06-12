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

            content.Components.Add(new Component
            {
                Id = "header",
                Position = 0,
                Active = true,
                Formatter = new ComponentFormatter
                {
                    BaseFormatter = "**{0}** #{1}\n*{2}* ({3}/{4})",
                    OverrideBaseIndex = true
                }
            });

            content.Components.Add(new ComponentGroup
            {
                Id = "message_box",
                Position = 1,
                Active = true,
                Formatter = new ComponentFormatter
                {
                    Separator = "\n",
                    ElementFormatter = "• {0}",
                    BaseFormatter = "```\n{0}\n```"
                },
                Capacity = 6,
                Values = new string[6] { "", "", "", "", "", "" }
            });
            /*
                Component 1: header
                - BaseFormatter:
                **{0:title}** #{1:id}
                *{2:game}* ({3:player_count}/{4:player_limit})
                
                - Example
                **Abner's Server** #F00G42
                *Werewolf* (1/8 players)    

                Component 2: message_box
                - Note: This is a basic message box that can easily be updated

                - Capacity: 6

                - Separator: \n

                - ElementFormatter:
                * {0}
                
                - BaseFormatter:
                ```
                {0}
                ```
                
                - Example:
                ```
                * Abner has joined
                * [Abner]: what in the world is happening
                *
                *
                *
                *
                ```
                */

            content.Components.Add(new ComponentGroup
            {
                Id = "actions",
                Capacity = 10,
                Formatter = new ComponentFormatter
                {
                    BaseFormatter = "**Actions**\n{0}",
                    Separator = "|",
                    ElementFormatter = "`{0}`",
                    OverrideBaseIndex = true
                },

            });
                /*
                Component 2: actions
                - Note: This component should be linked with the base List<IInput>, to auto-list out all possible inputs from the existing set

                - Capacity: null
                
                - OverrideBaseIndex: true (the list of actions must be specified when drawing the component)

                - Separator: |

                - ElementFormatter:
                `{0}`

                - BaseFormatter:
                **Actions**
                {0}

                - Example:
                **Actions**
                `join`|`leave`
             */
            
            var inputs = new List<IInput>();

            inputs.Add(new TextInput
            {
                Name = "leave",
                UpdateOnExecute = true,
                OnExecute = delegate (IUser user, ServerConnection connection, GameServer server)
                {
                    Player player = server.GetPlayer(user.Id);

                    if (player == null)
                        return;

                    server.Players.Remove(player);

                    var newHost = server.Players.OrderBy(x => x.JoinedAt).FirstOrDefault();

                    if (newHost == null)
                        return;

                    server.GetDisplayChannel(0).Content.GetComponent("header").Draw(server.Config.Title, server.Id, server.Config.GameId, server.Players.Count, "infinite players");
                    (server.GetDisplayChannel(0).Content.GetComponent("message_box") as ComponentGroup).Append($"[Console] {player.User.Username} has left");
                    (server.GetDisplayChannel(0).Content.GetComponent("message_box") as ComponentGroup).Append($"[Console] {newHost.User.Username} is now the new host");
                    server.GetDisplayChannel(0).Content.GetComponent("message_box").Draw();
                }
            });

            inputs.Add(new TextInput
            {
                Name = "join",
                UpdateOnExecute = true,
                OnExecute = delegate (IUser user, ServerConnection connection, GameServer server)
                {
                    Player player = server.GetPlayer(user.Id);

                    if (player != null)
                    {
                        (server.GetDisplayChannel(0).Content.GetComponent("message_box") as ComponentGroup).Append($"[To {user.Username}] You are already in this lobby!");
                        server.GetDisplayChannel(0).Content.GetComponent("message_box").Draw();

                        return;
                    }

                    // likewise, this should update the player list component on the display channel
                    server.Players.Add(new Player { User = user, JoinedAt = DateTime.UtcNow, Host = false, Playing = false });
                    (server.GetDisplayChannel(0).Content.GetComponent("message_box") as ComponentGroup).Append($"{user.Username} has joined");
                    server.GetDisplayChannel(0).Content.GetComponent("header").Draw(server.Config.Title, server.Id, server.Config.GameId, server.Players.Count, "infinite players");
                    server.GetDisplayChannel(0).Content.GetComponent("message_box").Draw();
                }
            });

            // commands in the lobby:

            // config: opens the config panel
            // for this command, this sets the frequency of the channel of where this was executed to the config channel (-1)
            // you can toggle components to be active or inactive
            // likewise, you can re-render them in a simple input execution
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
        public int Frequency { get; set; } // default is 0, the lobby.
        // the lobby is always there

        // how fast can this display update? minimum of 1 second.
        public TimeSpan RefreshRate { get; set; }

        // what is this display currently showing?
        public DisplayContent Content { get; set; }

        // what can the player currently do in this display channel? (reaction or text)
        public List<IInput> Inputs { get; set; }

        // now to make a version that incorporates arguments
        public override string ToString()
            => Content.ToString();
    }
}
