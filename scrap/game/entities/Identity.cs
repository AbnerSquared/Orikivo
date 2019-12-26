using System;
// Import System.Threading.Tasks;
namespace Orikivo
{
    /// <summary>
    /// A generic user that defines their back-end game state.
    /// </summary>
    public class Identity
    {
        public Identity(ulong id, string name, ulong guildId, IdentityTag tags = 0)
        {
            Id = id;
            Name = name;
            ReceiverId = guildId;
            if (!(tags == 0))
                Tags = tags;
            JoinedAt = DateTime.UtcNow;
        }
        public ulong Id { get; }
        public string Name { get; }
        public GameState State { get; internal set; }
        // Incomplete // public UserReceiver Receiver { get; }
        public IdentityTag Tags { get; internal set; } = 0;
        public bool IsHost => Tags.HasFlag(IdentityTag.Host);
        public bool IsPlaying => Tags.HasFlag(IdentityTag.Playing);
        public ulong ReceiverId { get; internal set; }
        public DateTime JoinedAt { get; }

        public override string ToString()
            => string.Format($"{(IsHost ? "**{0}**":"{0}")}{(IsPlaying ? "🔹" : "")}", Name);
    }
}
