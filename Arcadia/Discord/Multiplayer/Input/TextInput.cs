using Discord;
using System;
using System.Collections.Generic;
using Orikivo;
using Orikivo.Framework;
using Orikivo.Text;

namespace Arcadia.Multiplayer
{
    // This is the context used to help handle input

    // an action
    public class TextInput : IInput
    {
        public TextInput() {}

        public TextInput(string name, Action<InputContext> onExecute, bool updateOnExecute = false)
        {
            Name = name;
            OnExecute = onExecute;
            RequirePlayer = true;
            UpdateOnExecute = true;
        }

        public string Name { get; set; }

        public InputType Type => InputType.Text;

        public Func<IUser, ServerConnection, GameServer, bool> Criterion { get; set; }

        public Action<InputContext> OnExecute { get; set; }

        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }

        public bool CaseSensitive { get; set; }

        public List<GameProperty> Args { get; set; }

        public InputResult TryParse(Input input)
        {
            var result = new InputResult();

            result.Source = input.Text;
            string text = input.Text;

            // name parameters
            if (text.StartsWith(Name, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
            {
                Logger.Debug($"Reading input: \"{text}\"");
                result.IsSuccess = true;
                result.Input = this;

                var reader = new StringReader(text);
                reader.Skip(Name.Length);
                reader.SkipWhiteSpace();

                while (reader.CanRead())
                {
                    // TODO: Handle required arguments here.
                    string arg = reader.ReadUnquotedString();
                    reader.SkipWhiteSpace();
                    Logger.Debug($"Parsing argument: \"{arg}\"");

                    if (!string.IsNullOrWhiteSpace(arg))
                        result.Args.Add(arg);
                    else
                    {
                        result.IsSuccess = false;
                        break;
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            Logger.Debug($"Parsed {Name} ({result.IsSuccess})");
            return result;
        }
    }
}
