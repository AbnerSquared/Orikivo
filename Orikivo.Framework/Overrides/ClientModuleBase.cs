using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Orikivo
{
    public abstract class ClientModuleBase<T> : ModuleBase<T>
        where T : SocketCommandContext
    {
        // TODO: Create singular message handles, like the one in game client.

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
        }

        private IDMChannel GetOrCreateDMChannel(IUser user, RequestOptions options = null)
            => user.CreateDMChannelAsync(options).ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// Sends a direct message to the specified user.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(IUser user, string text = "", bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => await GetOrCreateDMChannel(user).SendMessageAsync(text, isTTS, embed, options);

        /// <summary>
        /// Sends a direct message to the user provided in the context.
        /// </summary>
        public async Task<IUserMessage> WhisperAsync(string text = "", bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => await WhisperAsync(Context.User, text, isTTS, embed, options);
    }
}
