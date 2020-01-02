using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class GuildDisplayInfo : IDisplayInfo
    {
        internal GuildDisplayInfo(OriGuild guild)
        {
            Name = "Custom";
            GuildName = guild.Name;
            GuildId = guild.Id;
            Summary = $"Custom commands built by members within {GuildName}.";
            Family = new List<ContextValue>();
            Customs = guild.Options.Commands.Select(x => new CustomDisplayInfo(guild, x)).ToList();
        }

        public ContextInfoType Type => ContextInfoType.Custom;
        
        public string Id => $"{GuildId}";
        public string Name { get; }
        public string GuildName { get; }
        public ulong GuildId { get; }

        public List<string> Aliases { get; }
        
        public List<IReport> Reports { get; }
        public List<ContextValue> Family { get; }
        public List<CustomDisplayInfo> Customs { get; }
        public string Summary { get; }

        public string Content => ContextUtils.WriteDisplayContent(this);

        public override string ToString()
            => Content;
    }
}
