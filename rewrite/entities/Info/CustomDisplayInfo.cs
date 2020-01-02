using System.Collections.Generic;

namespace Orikivo
{
    public class CustomDisplayInfo : IDisplayInfo
    {
        public CustomDisplayInfo(OriGuild guild, GuildCommand command)
        {
            GuildName = guild.Name;
            GuildId = guild.Id;
            Name = command.Name;
            Author = command.Author;
            Family = new List<ContextValue>() { new ContextValue(command) };
        }

        public ContextInfoType Type => ContextInfoType.Custom;

        public string GuildName { get; }
        public ulong GuildId { get; }

        public string Id => $"{GuildId}.{Name}"; // 00.yoshkill
        public string Name { get; }

        public List<string> Aliases { get; }
        
        public List<IReport> Reports { get; }

        public List<ContextValue> Family { get; }

        public string Summary { get; }

        public Author Author { get; }

        public string Content => ContextUtils.WriteDisplayContent(this);

        public bool HasImage { get; }

        public override string ToString()
            => Content;
    }
}
