using Discord;
using System;
using System.Collections.Generic;

namespace Arcadia
{
    // This is the context used to help handle input
    /// <summary>
    /// Represents the information provided from an <see cref="IInput"/>.
    /// </summary>
    public class InputContext
    {
        public IUser Invoker { get; set; }

        public ServerConnection Connection { get; set; }

        public GameServer Server { get; set; }

        public GameSession Session => Server.Session;

        public PlayerData Player => Server.Session?.GetPlayerData(Invoker.Id);

        public InputResult Input { get; set; }
    }

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

        public KeyType Type => KeyType.Text;

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
            }
            else
            {
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
