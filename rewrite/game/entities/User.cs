using System;
// Import System.Threading.Tasks;
namespace Orikivo
{
    /// <summary>
    /// A generic user that defines their back-end game state.
    /// </summary>
    public class User
    {
        public User(ulong id, string name, ulong guildId, UserTag tags = UserTag.Empty)
        {
            Id = id;
            Name = name;
            ReceiverId = guildId;
            if (!tags.HasFlag(UserTag.Empty))
                Tags |= tags;
            else
                Tags = tags;
            JoinedAt = DateTime.UtcNow;
        }
        public ulong Id { get; }
        public string Name { get; }
        public GameState State { get; internal set; }
        // Incomplete // public UserReceiver Receiver { get; }
        public UserTag Tags { get; internal set; }
        public bool IsHost => Tags.HasFlag(UserTag.Host);
        public bool IsPlaying => Tags.HasFlag(UserTag.Playing);
        public ulong ReceiverId { get; internal set; }
        public DateTime JoinedAt { get; }

        public override string ToString()
            => string.Format($"{(IsHost ? "**{0}**":"{0}")}{(IsPlaying ? "🔹" : "")}", Name);
    }
}
