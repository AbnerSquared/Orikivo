using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Desync;
using Orikivo.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// A custom command context that supports JSON container data and accounts.
    /// </summary>
    public class DesyncContext : SocketCommandContext
    {
        public User Account
        {
            get
            {
                Container.TryGetUser(User.Id, out User u);
                return u;
            }
            set => Container.AddOrUpdateUser(User.Id, value);
        }

        public Husk Husk => Account?.Husk;

        public HuskBrain Brain => Account?.Brain;

        public BaseGuild Server
        {
            get
            {
                Container.TryGetGuild(Guild.Id, out BaseGuild s);
                return s;
            }
            set => Container.AddOrUpdateGuild(Guild.Id, value);
        }

        public OriGlobal Global => Container.Global;

        public DesyncContainer Container { get; }

        public IEnumerable<AttachmentData> Attachments { get; internal set; }

        public IEnumerable<OptionData> Options { get; internal set; }

        public DesyncContext(DiscordSocketClient client, DesyncContainer container, SocketUserMessage msg)
            : base(client, msg)
        {
            Logger.Debug("[Debug] -- Constructing command context. --");
            Container = container; // ensured in container.

            if (Guild != null)
            {
                Container.GetOrAddGuild(Guild);
                Server.Synchronize(Guild);
                Logger.Debug("[Debug] -- Guild account found or built. --");
            }
            Logger.Debug($"[Debug] -- User {(Account == null ? "does not have an" : "has an")} account. --");
            Options = null;
        }

        // TODO: Create BaseContext, which others can inherit for AttachmentData and OptionData methods.
        public AttachmentData GetAttachment(string name)
            => Attachments.FirstOrDefault(x => x.Name == name);

        public OptionData GetOption(string name)
            => Options?.FirstOrDefault(x => x.Name == name);

        public T GetOptionValue<T>(string name)
            => (T) Options?.FirstOrDefault(x => x.Name == name && x.Type == typeof(T))?.Value;

        public T GetOptionOrDefault<T>(string name, T fallback = default(T))
            => GetOptionValue<T>(name) ?? fallback;
    }
}
