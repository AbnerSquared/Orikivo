using System.Collections.Generic;

namespace Orikivo.Models.Discord
{
    public interface IGuild : IModel<ulong>
    {
        string Name { get; }
        IEnumerable<ulong> RoleIds { get; }
        IEnumerable<ulong> ChannelIds { get; }
    }
}