using System;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using Orikivo.Desync;

namespace Orikivo
{
    public abstract class BaseModule<T> : ModuleBase<T>
        where T : SocketCommandContext
    {
        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
        }

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
                    Timeout = timeout ?? TimeSpan.FromSeconds(30),
                    Session = session
                };

                bool Filter(SocketMessage message, int index)
                {
                    return message.Author.Id == Context.User.Id && message.Channel.Id == Context.Channel.Id;
                }

                await collector.MatchAsync(Filter, options);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        /// <summary>
        /// Sends a direct message to the specified user.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(IUser user, string text = "", bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => await GetOrCreateDMChannel(user).SendMessageAsync(text, isTTS, embed, options);

        //public async Task<IUserMessage> WhisperAsync(IUser user, Bitmap image, string path, string text = "", bool isTTS = false, Embed embed = null, RequestOptions = null)
        //    => await GetOrCreateDMChannel(user).SendImageAsync(image, path, text, isTTS, embed, options);

        /// <summary>
        /// Sends a direct message to the specified user.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(IUser user, Message message, RequestOptions options = null)
        {
            if (Check.NotNull(message.AttachmentUrl))
                return await GetOrCreateDMChannel(user).SendFileAsync(message.AttachmentUrl, message.Text, message.IsTTS, message.Embed, options);
            else
                return await WhisperAsync(user, message.Text, message.IsTTS, message.Embed, options);
        }

        /// <summary>
        /// Sends a direct message to the user provided in the context.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(string text = "", bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => await WhisperAsync(Context.User, text, isTTS, embed, options);

        /// <summary>
        /// Sends a direct message to the user provided in the context.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(Message message, RequestOptions options = null)
            => await WhisperAsync(Context.User, message, options);

        public async Task<IUserMessage> ReplyAsync(Message message, RequestOptions options = null)
            => await Context.Channel.SendMessageAsync(message, options);
    }
}
