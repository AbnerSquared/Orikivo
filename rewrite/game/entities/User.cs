using System;

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
            if (tags != UserTag.Empty)
                Tags |= tags;
            else
                Tags = tags;
            JoinedAt = DateTime.UtcNow;
        }
        public ulong Id { get; }
        public string Name { get; }
        // only update on game start | game end
        public GameState State { get; internal set; }
        // private IDMChannel Channel { get; private set; }
        public UserTag Tags { get; internal set; }
        public bool IsHost => Tags.HasFlag(UserTag.Host);
        public ulong ReceiverId { get; internal set; }
        public DateTime JoinedAt { get; }
    }
}
