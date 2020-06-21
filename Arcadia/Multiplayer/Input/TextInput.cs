using Discord;
using System;
using System.Collections.Generic;

namespace Arcadia

{
    // an action
    public class TextInput : IInput
    {
        // what does the player need to type to execute this key?
        public string Name { get; set; }

        public KeyType Type => KeyType.Text;

        public Func<IUser, ServerConnection, GameServer, bool> Criterion { get; set; }

        public Action<IUser, ServerConnection, GameServer> OnExecute { get; set; }

        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }

        public List<GameProperty> Args { get; set; }

        IEnumerable<GameProperty> IInput.Args => Args;

        public InputResult TryParse(Input input)
        {
            InputResult result = new InputResult();
            // name parameters
            if (input.Text.StartsWith(Name))
            {
                result.IsSuccess = true;
                result.Input = this;
            }
            else
            {
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
