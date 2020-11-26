using System.Collections.Generic;
using Arcadia.Services;
using Orikivo;
using Orikivo.Desync;

namespace Arcadia
{
    public class Reward
    {
        public Reward()
        {
            ItemIds = new Dictionary<string, int>();
            Money = 0;
            Exp = 0;
        }

        public Dictionary<string, int> ItemIds { get; set; }
        public long Money { get; set; }
        public ulong Exp { get; set; }

        public int Count => GetCount();

        private int GetCount()
        {
            int count = ItemIds?.Count ?? 0;

            if (Money > 0)
                count++;

            if (Exp > 0)
                count++;

            return count;
        }

        public void Apply(ArcadeUser user)
        {
            if (Money > 0)
                user.Give(Money);

            ulong oldExp = user.Exp;

            if (Exp > 0)
               user.Exp += Exp;

            if (user.Config.Notifier.HasFlag(NotifyAllow.Level))
            {
                int oldLevel = ExpConvert.AsLevel(oldExp, user.Ascent);
                int level = user.Level;
                if (level > oldLevel)
                {
                    user.Notifier.Add($"Level up! ({LevelViewer.GetLevel(oldLevel, user.Ascent)} to {LevelViewer.GetLevel(level, user.Ascent)})");
                }
            }

            if (!Check.NotNullOrEmpty(ItemIds))
                return;

            foreach ((string itemId, int amount) in ItemIds)
                ItemHelper.GiveItem(user, itemId, amount);
        }
    }
}