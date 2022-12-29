using Discord;
using System;

namespace Arcadia.Multiplayer
{
    // This is the context used to help handle input
    public class ButtonInput : IInput
    {
        public InputType Type => InputType.Button;

        public Func<IUser, ServerConnection, GameServer, bool> Criterion { get; set; }

        public Action<InputContext> OnExecute { get; set; }

        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }

        public InputResult TryParse(InputResponse input)
        {
            throw new NotImplementedException();
        }
    }
}
