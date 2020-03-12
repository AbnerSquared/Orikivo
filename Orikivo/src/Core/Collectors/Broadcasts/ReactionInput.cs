using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{

    public interface ICriterion<in T>
    {

    }

    public abstract class ReactionCriterion : ICriterion<SocketReaction>
    {
        public async virtual Task<bool> JudgeAsync(SocketReaction reaction)
            => true;
    }

    public abstract class MessageCriterion : ICriterion<IMessage>
    {
        public async virtual Task<bool> JudgeAsync(IMessage message)
            => true;
    }

    // a generic emote input
    public interface IEmoteInput<TEmote> where TEmote : IEmote
    {
        TEmote Input { get; set; }

    }

    // a unicode emoji input
    public class EmojiInput : IEmoteInput<Emoji>
    {
        public Emoji Input { get; set; }
        public ReactionCriterion Criterion { get; set; }
    }

    // a custom emote input
    public class EmoteInput : IEmoteInput<Emote>
    {
        public Emote Input { get; set; }
        public ReactionCriterion Criterion { get; set; }
    }

    // generic input controller
    public interface IEmoteController<TEmote> where TEmote : IEmote
    {
        IUserMessage Message { get; }
        IReadOnlyList<IEmoteInput<TEmote>> Inputs { get; }
    }
    /*
    public class BaseEmoteController : IEmoteController<IEmote>
    {
        public const int MaxReactionCount = 20;
        protected readonly BaseSocketClient _client;

        public IUserMessage Message { get; }

    }
    
    // for unicode emotes
    public class EmojiController : BaseEmoteController, IEmoteController<Emoji>
    {
        public IReadOnlyList<EmojiInput> Inputs { get; }
        IReadOnlyList<IEmoteInput<Emoji>> IEmoteController<Emoji>.Inputs => Inputs;

        // no reactionInputs can have the same emoji.
    }

    // for custom emotes
    public class EmoteController : BaseEmoteController, IEmoteController<Emote>
    {
        public IReadOnlyList<EmoteInput> Inputs { get; }
        IReadOnlyList<IEmoteInput<Emote>> IEmoteController<Emote>.Inputs => Inputs;
    }

    public class ReactionInputAction
    {

    }
    */

    public class MessageBroadcast
    {
        private readonly IMessageChannel _channel;
        private IUserMessage _focus;

        public async Task ModifyAsync()
        {

        }

        public async Task ReplaceAsync()
        {

        }
    }

    public class MessageReceiver
    {
        private readonly IMessageChannel _channel;
        private IUserMessage _focus;

        internal async Task ModifyAsync(IUserMessage _focus)
        {

        }

        public async Task DeleteAsync()
        {

        }
    }
}
