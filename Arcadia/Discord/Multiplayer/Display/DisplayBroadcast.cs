using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a text-based display broadcast for a <see cref="GameServer"/>.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class DisplayBroadcast
    {
        public const int WaitingFrequency = 0;
        public const int EditingFrequency = 1;
        public const int WatchingFrequency = 2;

        public static readonly int[] ReservedFrequencies = { WaitingFrequency, EditingFrequency, WatchingFrequency };

        private DisplayBroadcast()
        {
            Reserved = true;
            Inputs = new List<IInput>();
        }

        public DisplayBroadcast(int frequency)
        {
            if (ReservedFrequencies.Contains(frequency))
                throw new Exception("This frequency is reserved.");

            Frequency = frequency;
            Reserved = false;
            Inputs = new List<IInput>();
        }

        /// <summary>
        /// Determines if this <see cref="DisplayBroadcast"/> is reserved.
        /// </summary>
        internal bool Reserved { get; }

        public static List<DisplayBroadcast> GetReservedBroadcasts()
        {
            return new List<DisplayBroadcast>
            {
                new DisplayBroadcast
                {
                    Frequency = WaitingFrequency,
                    State = GameState.Waiting,
                    Content = new DisplayContent
                    {
                        new Component("header", 0)
                        {
                            Formatter = new ComponentFormatter("> `{1}` **{0}**\n> {2} ({3})", true)
                        },
                        new ComponentGroup("console", 1, 6)
                        {
                            Formatter = new ComponentFormatter("```\n{0}\n```", "• {0}", "\n"),
                            Values = new[] { "", "", "", "", "", "" }
                        }
                    }
                },
                new DisplayBroadcast
                {
                    Frequency = EditingFrequency,
                    State = GameState.Editing,
                    Content = new DisplayContent
                    {
                        new ComponentGroup("console", 0, 4)
                        {
                            Formatter = new ComponentFormatter("> Editing **{1}**\n```\n{0}\n```", "• {0}", "\n"),
                            Values = new[] { "", "", "", "" }
                        },
                        new Component("config", 1)
                        {
                            Formatter = new ComponentFormatter("> **Settings**\n• Name: **{0}**\n• Privacy: **{1}**\n• Game Mode: **{2}**\n• Spectator Panel: **{3}**\n• Server Invites: **{4}**\n• Chat: **{5}**", true)
                        },
                        new ComponentGroup("game_config", 2, 0, false)
                        {
                            Formatter = new ComponentFormatter("\n> **{1} Ruleset**\n{0}", "• {0}", "\n", true, true)
                        }
                    }
                }
            };
        }

        // Instead of the frequency system, it could be possible to simply use object references to the server connection
        // and set their references again once initialized?
        public int Frequency { get; private set; }

        public GameState? State { get; private set; }

        public DisplayContent Content { get; set; }

        public List<IInput> Inputs { get; set; }

        public IComponent GetComponent(string id)
        {
            return Content?.GetComponent(id);
        }

        public ComponentGroup GetGroup(string id)
        {
            return Content?.GetGroup(id);
        }

        private string DebuggerDisplay => $"{(Reserved ? "[Reserved] " : "")}{Frequency}{(State.HasValue ? $", {State.ToString()}" : "")}";

        public override string ToString()
            => Content?.ToString();
    }
}
