using System;
using System.Threading.Tasks;

namespace Orikivo
{
    internal static class ItemDictionary
    {
        internal static readonly string PocketLawyer = "entity_pocket_lawyer";
    }

    internal static class ItemType
    {
        internal static ItemInfo PocketLawyer
        {
            get
            {
                ItemInfo info = new ItemInfo();

                info.Id = "entity_pocket_lawyer";
                info.Name = "Pocket Lawyer";
                info.Rarity = ItemRarity.Common;
                info.Group = ItemGroup.Entity;
                info.Summary = "This little guy works against ORS to keep you safe.";
                AccountCriteria toOwnCriteria = AccountCriteria.Empty;
                toOwnCriteria.Debt = 1000;
                info.ToOwn = toOwnCriteria;

                ItemActionInfo actionInfo = new ItemActionInfo();
                actionInfo.OnUse.Custom = GetCustomActionFor(ItemCustomAction.NoDebt);
                actionInfo.MaxUses = 1;
                actionInfo.BreakOnLastUse = true;
                info.ActionInfo = actionInfo;

                ItemMarketInfo marketInfo = new ItemMarketInfo();
                marketInfo.Value = 40;
                marketInfo.SellRate = 0.5;
                info.MarketInfo = marketInfo;

                return info;
            }
        }

        public static Func<OriUser, Task> GetCustomActionFor(ItemCustomAction action)
        {
            switch (action)
            {
                /*case ItemCustomAction.NoDebt*/default:
                    return async (OriUser x) =>
                    {
                        WalletData wallet = x.GetWallet(CurrencyType.Generic);
                        x.GetWallet(CurrencyType.Generic).Give(wallet.Debt);
                    }; 
            }
        }
    }
}
