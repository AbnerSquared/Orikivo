using Discord;
using System;
using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic <see cref="GameServer"/> input.
    /// </summary>
    public interface IInput
    {
        public InputType Type { get; }

        // what does this key do whenever it is executed?
        public Func<IUser, ServerConnection, GameServer, bool> Criterion { get; set; }

        public Action<InputContext> OnExecute { get; set; }

        public InputResult TryParse(InputResponse input);

        // this determines if UpdateAsync() should be called whenever this input is executed.
        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }
    }

    public interface IControl
    {

    }

    public class GameControls : List<IControl>
    {
        public bool Enabled { get; set; } = true;
    }

    public enum InputStyle
    {
        Switch = 1, // Activated input that can turn on or off
        Button = 2, // Activated input that requires no args
        List = 3, // A list containing a pool of selectable values
        Text = 4 // Raw text input
    }
}
