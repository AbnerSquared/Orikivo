using Discord;
using System;

namespace Arcadia
{
    public interface IInput
    {
        public KeyType Type { get; }
        // what does this key do whenever it is executed?
        public Func<IUser, ServerConnection, GameServer, bool> Criterion { get; set; }
        public Action<IUser, ServerConnection, GameServer> OnExecute { get; set; }

        public InputResult TryParse(Input input);

        // this determines if UpdateAsync() should be called whenever this input is executed.
        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }
    }
}
