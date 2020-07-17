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

        public Husk Husk
        {
            get => Account?.Husk;
        }

        public HuskBrain Brain
        {
            get => Account?.Brain;
        }

        public OriGuild Server
        {
            get
            {
                Container.TryGetGuild(Guild.Id, out OriGuild s);
                return s;
            }
            set => Container.AddOrUpdateGuild(Guild.Id, value);
        }

        public OriGlobal Global { get; }

        public DesyncContainer Container { get; }

        public IEnumerable<AttachmentData> Attachments { get; internal set; }

        public IEnumerable<OptionData> Options { get; internal set; }

        public DesyncContext(DiscordSocketClient client, DesyncContainer container, SocketUserMessage msg)
            : base(client, msg)
        {
            Logger.Debug("[Debug] -- Constructing command context. --");
            Container = container; // ensured in container.
            Global = Container.Global;
            if (Guild != null)
            {
                Container.GetOrAddGuild(Guild);
                Server.TryUpdateName(Guild.Name);
                Server.TryUpdateOwner(Guild.OwnerId);
                Logger.Debug("[Debug] -- Guild account found or built. --");
            }
            Logger.Debug($"[Debug] -- User {(Account == null ? "does not have an" : "has an")} account. --");
            Options = null;
        }

        // TODO: Create BaseContext, which others can inherit for AttachmentData and OptionData methods.
        public AttachmentData GetAttachment(string name)
            => Attachments.Where(x => x.Name == name).FirstOrDefault();

        public OptionData GetOption(string name)
            => Options?.Where(x => x.Name == name).FirstOrDefault();

        public T GetOptionValue<T>(string name)
            => (T)Options?.Where(x => x.Name == name && x.Type == typeof(T))?.FirstOrDefault()?.Value;

        public T GetOptionOrDefault<T>(string name, T fallback = default(T))
            => GetOptionValue<T>(name) ?? fallback;
    }
}
