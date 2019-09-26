using Discord;
using Discord.WebSocket;
using System.Drawing;

namespace Orikivo
{
    public class CompactUser
    {
        public CompactUser(SocketUser user)
        {
            string s = "e";
            s.Debug("Compact user is now being built.");
            OldAccount a = new DataContainer().GetOrAddAccount(user);
            Avatar = AvatarManager.GetAvatarBitmap(user);
            Username = user.Username;
            Status = user.Status;
            Id = user.Id;
            Activity = user.Activity;
            Level = a.Data.Level;
            Percentile = a.Data.LevelPercentile() / 100;
            Balance = a.Balance;
            Experience = a.Data.Experience;
        }

        public Bitmap Avatar  { get; }
        public ulong Id { get; }
        public string Username { get; }
        public UserStatus Status { get; }
        public IActivity Activity { get; }
        public int Level { get; }
        public double Percentile { get; }
        public ulong Balance { get; }
        public ulong Experience { get; }
    }
}