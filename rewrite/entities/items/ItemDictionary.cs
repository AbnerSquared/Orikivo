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

                info.CanStack = true;
                info.Id = "entity_pocket_lawyer";
                info.Name = "Pocket Lawyer";
                info.Rarity = ItemRarity.Common;
                info.Group = ItemGroup.Entity;
                info.Summary = "This little guy works against ORS to keep you safe.";
                UserCriteria toOwnCriteria = UserCriteria.Empty;
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
                        CurrencyData wallet = x.GetWalletFor(CurrencyType.Generic);
                        x.GetWalletFor(CurrencyType.Generic).Give(wallet.Debt);
                    }; 
            }
        }
    }
}
