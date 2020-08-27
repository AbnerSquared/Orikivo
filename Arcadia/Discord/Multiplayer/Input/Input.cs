using Discord;

namespace Arcadia.Multiplayer
{
    public class Input
    {
        public string Text { get; internal set; }
        public IEmote Reaction { get; internal set; }

        public ReactionHandling Flag { get; internal set; }
    
        public static implicit operator Input(string text)
            => new Input { Text = text };
    }
}
