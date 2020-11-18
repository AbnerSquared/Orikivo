using Discord;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic receiver to a broadcast.
    /// </summary>
    public interface IReceiver
    {
        IUserMessage Message { get; }
        IMessageChannel Channel { get; }
    }
}
