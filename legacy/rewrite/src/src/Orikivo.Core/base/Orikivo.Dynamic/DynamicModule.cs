using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Linq;

namespace Orikivo.Dynamic
{
    //public class DynamicModule : DynamicModule<OrikivoCommandContext> {}

    public class DynamicModule<T> : ModuleBase<T> where T : OrikivoCommandContext
    {
        public DynamicManager Manager { get; set; }

        public Task<SocketMessage> ListenToReplyAsync(ICriteria<SocketMessage> criteria, TimeSpan? timeout = null)
            => Manager.ListenToReplyAsync(Context, criteria, timeout);

        public Task<SocketMessage> ListenToReplyAsync(bool sourceUser = true, bool sourceChannel = true, TimeSpan? timeout = null)
            => Manager.ListenToReplyAsync(Context, sourceUser, sourceChannel, timeout);

        public Task<IUserMessage> ClearReplyAsync(string content, bool tts = false, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
            => Manager.ClearReplyAsync(Context, content, tts, embed, timeout, options);

        public Task<IUserMessage> PagedReplyAsync(IEnumerable<object> pages, bool sourceUser = true)
        {
            PaginatedMessage message = new PaginatedMessage();
            message.Pages = pages;
            return PagedReplyAsync(message, sourceUser);
        }

        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage message, bool sourceUser = true)
        {
            Criteria<SocketReaction> criteria = new Criteria<SocketReaction>();
            if (sourceUser)
                criteria.Add(new EnsureReactionFromUserCriteria());
            return PagedReplyAsync(message, criteria);
        }

        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage message, ICriteria<SocketReaction> criteria)
            => Manager.SendPaginatedMessageAsync(Context, message, criteria);

        public RuntimeResult Result(string reason = null)
            => new DynamicResult(reason);
    }

    public class PaginatedMessageCallback : IReactionCallback
    {
        public OrikivoCommandContext Context { get; }
        public DynamicManager Manager { get; private set; }
        public IUserMessage Message { get; private set; }

        public RunMode RunMode => RunMode.Sync;
        public ICriteria<SocketReaction> Criteria => _criteria;
        public TimeSpan? Timeout => options.Timeout;

        private readonly ICriteria<SocketReaction> _criteria;
        private readonly PaginatedMessage _message;

        private PaginatedDisplayOptions options => _message.Options;
        private readonly int pages;
        private int page = 1;

        public PaginatedMessageCallback(DynamicManager manager, OrikivoCommandContext context, PaginatedMessage message, ICriteria<SocketReaction> criteria = null)
        {
            Manager = manager;
            Context = context;
            _criteria = criteria ?? new EmptyCriteria<SocketReaction>();
            _message = message;
            pages = _message.Pages.Count();
        }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            IEmote emote = reaction.Emote;

            if (emote.Equals(options.First))
                page = 1;
            else if (emote.Equals(options.Next))
            {
                if (page >= pages)
                    return false;
                ++page;
            }
            else if (emote.Equals(options.Previous))
            {
                if (page <= 1)
                    return false;
                --page;
            }
            else if (emote.Equals(options.Last))
                page = pages;
            else if (emote.Equals(options.Cancel))
            {
                await Message.DeleteAsync().ConfigureAwait(false);
                return true;
            }

            _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await UpdateAsync().ConfigureAwait(false);
            return false;
        }

        public async Task ShowAsync()
        {
            Embed e = BuildPage();
            var message = await Context.Channel.SendMessageAsync(_message.Content, embed: e).ConfigureAwait(false);
            Message = message;
            Manager.AddReactionCallback(message, this);
            _ = Task.Run(async () =>
            {
                await message.AddReactionAsync(options.First);
                await message.AddReactionAsync(options.Previous);
                await message.AddReactionAsync(options.Next);
                await message.AddReactionAsync(options.Last);
                await message.AddReactionAsync(options.Cancel);
            });

            if (Timeout.HasValue && Timeout.Value != null)
            {
                _ = Task.Delay(Timeout.Value).ContinueWith(_ =>
                {
                    Manager.RemoveReactionCallback(message);
                    _ = Message.DeleteAsync();
                });
            }
        }

        protected Embed BuildPage()
        {
            return _message.Parent
                .WithDescription(_message.Pages.ElementAt(page - 1).ToString())
                .WithFooter(f => f.Text = string.Format(options.Footer, page, pages))
                .Build();
        }

        private async Task UpdateAsync()
        {
            Embed e = BuildPage();
            await Message.ModifyAsync(m => m.Embed = e).ConfigureAwait(false);
        }

    }

    public interface IReactionCallback
    {
        RunMode RunMode { get; }
        ICriteria<SocketReaction> Criteria { get; }
        TimeSpan? Timeout { get; }
        OrikivoCommandContext Context { get; }

        Task<bool> HandleCallbackAsync(SocketReaction reaction);
    }

    public class PaginatedMessage
    {
        public IEnumerable<object> Pages { get; set; }
        public string Content { get; set; } = "";
        public EmbedBuilder Parent { get; set; } // the base embed in which all information will be in.
        public PaginatedDisplayOptions Options { get; set; } = PaginatedDisplayOptions.Default;
    }

    // How each embed will look...
    public class PaginatedDisplayOptions
    {
        public static PaginatedDisplayOptions Default = new PaginatedDisplayOptions();
        public string Footer = "Page {0} of {1}";
        public TimeSpan? Timeout { get; set; } = TimeSpan.FromSeconds(20);
        public IEmote First = new Emoji("");
        public IEmote Last = new Emoji("");
        public IEmote Next = new Emoji("");
        public IEmote Previous = new Emoji("");
        public IEmote Cancel = new Emoji("");
    }

    public class DynamicResult : RuntimeResult
    {
        public DynamicResult(string reason = null) : base(null, reason)
        {

        }
    }
}