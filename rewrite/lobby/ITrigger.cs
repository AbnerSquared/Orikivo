using System;
using System.Collections.Generic;

namespace Orikivo
{
    // the generic structure for a trigger.
    public interface ITrigger<T> where T : ITriggerContext
    {
        bool CanParse(string context);
        T Parse(string context);
    }

    public interface ITriggerContext
    {

    }

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
        public UserTag Tags { get; internal set; }
        public bool IsHost => Tags.HasFlag(UserTag.Host);
        public ulong ReceiverId { get; internal set; }
        public DateTime JoinedAt { get; }
    }

    public class UserGameData
    {
        public ulong Id { get; } // user id
        public List<GameAttribute> Attributes { get; }
    }


}
