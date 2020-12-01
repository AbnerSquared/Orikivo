using System;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Discord.Addons.Collectors;
using Discord.WebSocket;

namespace Orikivo
{
    public abstract class BaseModule<TContext, TContainer, TGuild, TUser> : ModuleBase<TContext>
        where TGuild : BaseGuild
        where TUser : BaseUser
        where TContainer : BaseContainer<TGuild, TUser>
        where TContext : BaseCommandContext<TContainer, TGuild, TUser>
    {
        private IDMChannel GetOrCreateDMChannel(IUser user)
            => user.GetOrCreateDMChannelAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        protected async Task StartSessionAsync(MessageSession session, TimeSpan? timeout = null)
        {
            try
            {
                var collector = new MessageCollector(Context.Client);

                var options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = timeout ?? TimeSpan.FromSeconds(30)
                };

                bool Filter(SocketMessage message, int index)
                {
                    return message.Author.Id == Context.User.Id && message.Channel.Id == Context.Channel.Id;
                }

                await collector.RunSessionAsync(session, Filter, options);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        /// <summary>
        /// Sends a direct message to the specified user.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(IUser user, string text = "", bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null)
        {
            return await GetOrCreateDMChannel(user).SendMessageAsync(text, isTTS, embed, options, allowedMentions);
        }

        /// <summary>
        /// Sends a direct message to the specified user.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(IUser user, Message message, RequestOptions options = null, AllowedMentions allowedMentions = null)
        {
            if (Check.NotNull(message.AttachmentUrl))
                return await GetOrCreateDMChannel(user).SendFileAsync(message.AttachmentUrl, message.Text, message.IsTTS, message.Embed, options);

            return await WhisperAsync(user, message.Text, message.IsTTS, message.Embed, options, allowedMentions);
        }

        /// <summary>
        /// Sends a direct message to the user provided in the context.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(string text = "", bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null)
            => await WhisperAsync(Context.User, text, isTTS, embed, options, allowedMentions);

        /// <summary>
        /// Sends a direct message to the user provided in the context.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(Message message, RequestOptions options = null, AllowedMentions allowedMentions = null)
            => await WhisperAsync(Context.User, message, options, allowedMentions);

        public new async Task<IUserMessage> ReplyAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null)
        {
            return await Context.Channel.SendMessageAsync(Context.Account, text, isTTS, embed, options, allowedMentions);
        }

        public async Task<IUserMessage> ReplyAsync(Embed embed = null, string text = null, bool isTTS = false, RequestOptions options = null, AllowedMentions allowedMentions = null)
        {
            return await Context.Channel.SendMessageAsync(Context.Account, text, isTTS, embed, options, allowedMentions);
        }

        public async Task<IUserMessage> ReplyAsync(Message message, RequestOptions options = null, AllowedMentions allowedMentions = null)
        {
            return await Context.Channel.SendMessageAsync(message, options, allowedMentions);
        }

        public async Task<IUserMessage> ReplyAsync(MessageContent content, RequestOptions options = null, AllowedMentions allowedMentions = null)
        {
            return await Context.Channel.SendMessageAsync(content, options, allowedMentions);
        }
    }
}
