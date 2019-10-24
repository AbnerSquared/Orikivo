using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // TODO: Overhaul criteria checker.
    /// <summary>
    /// A criteria packet that checks a user for all specified values required.
    /// </summary>
    public struct UserCriteria
    {
        public static UserCriteria Empty
            => new UserCriteria
            {
                Merits = new List<string>(),
                Upgrades = new List<(string name, int level)>(),
                Stats = new List<(string, int)>(),
                Items = new List<(string, int)>(),
                Boosters = new List<(string, int)>(),
                Exp = new List<(ExpType, ulong)>(),
                Balance = null,
                Debt = null,
                BoosterCount = null,
                MeritCount = null,
                ItemCount = null,
                UpgradeCount = null
            };

        public List<(string, int)> Stats { get; internal set; } // stats needed to be true

        public List<string> Merits { get; internal set; }
        public int? MeritCount { get; internal set; }

        public List<(string, int)> Upgrades { get; internal set; }
        public int? UpgradeCount { get; internal set; }
        
        public List<(string, int)> Items { get; internal set; }
        public int? ItemCount { get; internal set; }

        public List<(string, int)> Boosters { get; internal set; }
        public int? BoosterCount { get; internal set; }
        
        public List<(ExpType, ulong)> Exp { get; internal set; }
        public ulong? Balance { get; internal set; }
        public ulong? Debt { get; internal set; }

        public bool Check(OriUser user)
        {
            foreach ((string, int) upgrade in Upgrades)
                if (!user.HasUpgradeAt(upgrade.Item1, upgrade.Item2))
                    return false;
            foreach ((string, int) stat in Stats)
                if (!user.MeetsStatCriteria(stat))
                    return false;
            foreach (string merit in Merits)
                if (!user.HasMerit(merit))
                    return false;
            foreach ((string, int) item in Items)
                if (!user.HasItemAt(item.Item1, item.Item2))
                    return false;
            foreach ((string, int) booster in Boosters)
                if (!(user.Boosters.Where(x => x.Key == booster.Item1).Count() >= booster.Item2))
                    return false;
            foreach ((ExpType, ulong) exp in Exp)
                if (exp.Item1 == ExpType.Generic)
                    if (!(user.Exp == exp.Item2))
                        return false;
            if (UpgradeCount.HasValue)
                if (!(user.Upgrades.Count >= UpgradeCount.Value))
                    return false;
            if (ItemCount.HasValue)
                if (!(user.Items.Count >= ItemCount.Value))
                    return false;
            if (BoosterCount.HasValue)
                if (!(user.Boosters.Count >= BoosterCount.Value))
                    return false;
            if (MeritCount.HasValue)
                if (!(user.Merits.Count >= MeritCount.Value))
                    return false;
            if (Balance.HasValue)
                if (!(user.Balance >= Balance.Value))
                    return false;
            if (Debt.HasValue)
                if (!(user.Debt >= Debt.Value))
                    return false;

            return true;
        }
    }
}
