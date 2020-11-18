using Discord;
using System;
using System.Collections.Generic;

namespace Arcadia.Multiplayer

{
    // Reaction inputs cannot have any args.
    public class ReactionInput : IInput
    {
        public InputType Type => InputType.Reaction;

        // what does the player need to react with to execute this key?
        public IEmote Emote { get; set; }

        // what method of reaction used is needed to invoke this input? (by default, it is set to any)
        public ReactionHandling Handling { get; set; } = ReactionHandling.Any;

        // Determines if the bot auto-reverts the reaction made on this input (i.e. removed added reaction once read)
        bool RevertOnRead { get; set; }

        public Func<IUser, ServerConnection, GameServer, bool> Criterion { get; set; }

        public Action<InputContext> OnExecute { get; set; }

        public bool UpdateOnExecute { get; set; }

        public bool RequirePlayer { get; set; }

        // Does this specific input need to be placed on all connected displays?
        public bool RequireOnMessage { get; set; }

        public InputResult TryParse(Input input)
        {
            var result = new InputResult();

            if (input.Reaction.Equals(Emote) && Handling.HasFlag(input.Flag))
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
