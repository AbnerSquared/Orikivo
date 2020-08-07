using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Multiplayer
{
    // TODO: Remove RefreshRate from DisplayChannel and implement it into ServerConnection
    /// <summary>
    /// Represents a text-based display broadcast for a <see cref="GameServer"/>.
    /// </summary>
    public class DisplayBroadcast
    {
        // this is an array of reserved frequencies
        // these channels are always paired into a game server

        // 2: Watching channel
        // 1: Waiting channel
        // 0: Editing channel
        public static int[] ReservedFrequencies => new[] { 2, 1, 0 };

        internal DisplayBroadcast() { }

        public DisplayBroadcast(int frequency, DisplayContent content)
        {
            if (ReservedFrequencies.Contains(frequency))
                throw new Exception("This frequency is reserved.");

            Frequency = frequency;
            Content = content;
            Reserved = false;
            Inputs = new List<IInput>();
        }

        internal bool Reserved { get; private set; }

        public static List<DisplayBroadcast> GetReservedBroadcasts()
        {
            var waiting = new DisplayContent
            {
                Components = new List<IComponent>
                {
                    new Component
                    {
                        Id = "header",
                        Position = 0,
                        Active = true,
                        Formatter = new ComponentFormatter
                        {
                            BaseFormatter = "**{0}** #{1}\n*{2}* ({3})",
                            OverrideBaseValue = true
                        }
                    },

                    new ComponentGroup
                    {
                        Id = "console",
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
                    }
                }
            };

            var editing = new DisplayContent
            {
                Components = new List<IComponent>
                {
                    new ComponentGroup
                    {
                        Id = "console",
                        Formatter = new ComponentFormatter
                        {
                            Separator = "\n",
                            ElementFormatter = "• {0}",
                            BaseFormatter = "**Editing {1}**\n```\n{0}\n```",
                            OverrideBaseValue = false
                        },
                        Capacity = 4,
                        Values = new string[4] { "", "", "", "" },
                        Active = true,
                        Position = 0
                    },

                    new Component
                    {
                        Id = "config",
                        Formatter = new ComponentFormatter
                        {
                            BaseFormatter = "**Config**\n> **Title**: `{0}`\n> **Privacy**: `{1}`\n> **Game**: `{2}`",
                            OverrideBaseValue = true
                        },
                        Active = true,
                        Position = 1
                    },

                    new ComponentGroup
                    {
                        Id = "game_config",
                        Capacity = 8,
                        Formatter = new ComponentFormatter
                        {
                            BaseFormatter = "**{1} Config**\n{0}",
                            ElementFormatter = "> {0}",
                            OverrideBaseValue = true,
                            Separator = "\n"
                        },
                        Active = false,
                        Position = 2
                    }
                }
            };

            var channels = new List<DisplayBroadcast>();
            
            // reserved watching channel
            channels.Add(new DisplayBroadcast
            {
                Frequency = 0,
                State = GameState.Waiting,
                Content = waiting,
                Inputs = new List<IInput>(),
                Reserved = true
            });

            // reserved editing channel
            channels.Add(new DisplayBroadcast
            {
                Frequency = 1,
                State = GameState.Editing,
                Content = editing,
                Inputs = new List<IInput>(),
                Reserved = true
            });

            // reserved watching channel
            return channels;
        }

        // Instead of the frequency system, it could be possible to simply use object references to the server connection
        // and set their references again once initialized?


        // what frequency is this display broadcasting to?
        public int Frequency { get; set; } // default is 0, the lobby.
        // the lobby is always there

        public GameState? State { get; set; } = null; // default is null

        // what is this display currently showing?
        public DisplayContent Content { get; set; }

        // what can the player currently do in this display channel? (reaction or text)
        public List<IInput> Inputs { get; set; }

        public IComponent GetComponent(string id)
        {
            if (Content == null)
                return null;

            foreach (IComponent component in Content.Components)
            {
                if (component.Id == id)
                    return component;
            }

            return null;
        }

        // now to make a version that incorporates arguments
        public override string ToString()
            => Content.ToString();
    }
}
