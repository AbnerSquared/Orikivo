using Discord;

namespace Arcadia.Multiplayer
{
    public class InputResponse
    {
        public string Text { get; internal set; }
        public IEmote Reaction { get; internal set; }

        public ReactionHandling Flag { get; internal set; }
    
        public static implicit operator InputResponse(string text)
            => new InputResponse { Text = text };
    }
}
