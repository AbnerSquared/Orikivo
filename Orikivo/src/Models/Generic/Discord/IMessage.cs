using System;

namespace Orikivo.Models.Discord
{
    public interface IMessage : IModel<ulong>
    {
        string Content { get; }

        DateTime Timestamp { get; }
    }
}