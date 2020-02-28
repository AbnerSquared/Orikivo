using Discord;
using Discord.WebSocket;

namespace Orikivo
{
    public class UserBaseInfo : EntityInfo
    {
        public UserBaseInfo(IUser user) : base(user)
        {
            Status = user.Status;
            AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarId) ? user.GetDefaultAvatarUrl() : user.GetAvatarUrl();
            Discriminator = user.Discriminator;
        }

        public ActivityBaseInfo Activity { get; }
        public UserStatus Status { get; }
        public string AvatarUrl { get; }
        public string Discriminator { get; }
    }

    public class ActivityBaseInfo
    {
        public ActivityBaseInfo(IActivity activity)
        {
            Name = activity.Name;
            Type = activity.Type;
        }

        public string Name { get; }
        public ActivityType Type { get; }
    }

    public class ActivityInfo : ActivityBaseInfo
    {
        public ActivityInfo(IActivity activity) : base(activity)
        {
            Details = activity.Details;
            Flags = new ActivityFlags(activity.Flags.HasFlag(ActivityProperties.Join),activity.Flags.HasFlag(ActivityProperties.JoinRequest),
                activity.Flags.HasFlag(ActivityProperties.Play), activity.Flags.HasFlag(ActivityProperties.Spectate),
                activity.Flags.HasFlag(ActivityProperties.Sync));
            // ActivityProperties.Instance? Figure out the purpose of this
        }

        public string Details { get; }
        public ActivityFlags Flags { get; }


    }
    public class ActivityFlags
    {
        public ActivityFlags(bool canJoin = false, bool canJoinRequest = false, bool canPlay = false, bool canSpectate = false, bool canSync = false)
        {
            CanJoin = canJoin;
            CanJoinRequest = canJoinRequest;
            CanPlay = canPlay;
            CanSpectate = canSpectate;
            CanSync = canSync;
        }

        public bool CanJoin { get; }
        public bool CanJoinRequest { get; }

        public bool CanSpectate { get; }
        public bool CanPlay { get; }
        public bool CanSync { get; }
    }
}