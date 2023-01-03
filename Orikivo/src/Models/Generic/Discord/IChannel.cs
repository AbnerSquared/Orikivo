using System.Collections.Generic;

namespace Orikivo.Models.Discord
{
    public interface IChannel : IModel<ulong>
    {
        string Name { get; }
        IEnumerable<IMessage> Messages { get; }
    }
}