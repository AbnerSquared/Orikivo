using Discord;
using System;
using System.Collections.Generic;
using Orikivo;

namespace Arcadia.Multiplayer
{
    // This is the context used to help handle input

    // an action
    public class TextInput : IInput
    {
        public TextInput() {}

        public TextInput(string name, Action<InputContext> onExecute)
        {
            Name = name;
            OnExecute = onExecute;
            RequirePlayer = true;
        }

        // what does the player need to type to execute this key?
        public string Name { get; set; }

        public InputType Type => InputType.Text;

        public Func<IUser, ServerConnection, GameServer, bool> Criterion { get; set; }

        public Action<InputContext> OnExecute { get; set; }

        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }

        public bool CaseSensitive { get; set; }

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

                var reader = new StringReader(input.Text);
                reader.Skip(Name.Length);

                while (reader.CanRead())
                {
                    // TODO: Handle required arguments here.
                    string arg = reader.ReadString();
                    result.Args.Add(arg);
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
