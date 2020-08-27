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

        public InputResult TryParse(Input input);

        // this determines if UpdateAsync() should be called whenever this input is executed.
        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }

        public IEnumerable<GameProperty> Args { get; }
    }
}
