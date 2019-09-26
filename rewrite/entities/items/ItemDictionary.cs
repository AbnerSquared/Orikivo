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
                info.Rarity = ItemRarityType.Common;
                info.Group = ItemGroupType.Entity;
                info.Summary = "This little guy works against ORS to keep you safe.";
                ItemCriteria toOwnCriteria = ItemCriteria.Empty;
                toOwnCriteria.Debt = 1000;
                info.ToOwn = toOwnCriteria;

                ItemActionInfo actionInfo = new ItemActionInfo();
                actionInfo.OnUse = ItemCustomAction.NoDebt;
                actionInfo.MaxUses = 1;
                actionInfo.BreakOnUse = true;
                info.ActionInfo = actionInfo;

                ItemMarketInfo marketInfo = new ItemMarketInfo();
                marketInfo.Value = 40;
                marketInfo.SellRate = 0.5;

                return info;
            }
        }
    }
}
