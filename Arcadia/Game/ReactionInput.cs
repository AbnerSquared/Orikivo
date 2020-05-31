using Discord;
using System;

namespace Arcadia

{
    public class ReactionInput : IInput
    {
        public KeyType Type => KeyType.Reaction;

        // what does the player need to react with to execute this key?
        public IEmote Emote { get; set; }

        // what method of reaction used is needed to invoke this input? (by default, it is set to any)
        public ReactionFlag Flag { get; set; } = ReactionFlag.Any;

        public Func<Player, bool> Criterion { get; set; }

        public Action<Player, GameServer> OnExecute { get; set; }

        public InputResult TryParse(IEmote emote)
        {
            InputResult result = new InputResult();

            if (emote == Emote)
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
