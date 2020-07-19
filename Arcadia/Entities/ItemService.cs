using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    internal static class Items
    {
        internal static readonly string PocketLawyer = "su_pl";
    }

    public static class ItemHelper
    {
        public static long GetUniqueId()
        {
            var offset = new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow.Ticks - offset.Ticks);
        }

        public static List<Item> Items =>
            new List<Item>
            {
                new Item
                {
                    Id = "su_pl",
                    Name = "Summon: Pocket Lawyer",
                    Summary = "A summon that completely wipes all debt from a user.",
                    Quotes = new List<string>
                    {
                        "With my assistance, ORS doesn't stand a chance."
                    },
                    Rarity = ItemRarity.Myth,
                    Tag = ItemTag.Summon,
                    Value = 2500,
                    CanBuy = false,
                    CanSell = true,
                    CanDestroy = true,
                    TradeLimit = 0,
                    GiftLimit = 1,
                    BypassCriteriaOnGift = true,
                    Size = 100,
                    OnUse = new ItemAction
                    {
                        Durability = 1,
                        Cooldown = TimeSpan.FromHours(48),
                        DeleteOnBreak = true,
                        OnUse = user => user.Debt = 0
                    },
                    OwnLimit = 3,
                    MarketCriteria = null,
                    ToOwn = null,
                    ToUnlock = null,
                    ToExpire = null
                }
            };

        public static bool CanGift(string itemId, UniqueItemData data)
        {
            var item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.GiftLimit.HasValue)
            {
                if (data.GiftCount.GetValueOrDefault(0) >= item.GiftLimit.Value)
                    return false;
            }

            return true;
        }

        public static bool CanTrade(string itemId, UniqueItemData data)
        {
            var item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.TradeLimit.HasValue)
            {
                if (data.TradeCount.GetValueOrDefault(0) >= item.TradeLimit.Value)
                    return false;
            }

            return true;
        }

        public static bool IsUnique(Item item)
        {
            if (item.TradeLimit.HasValue)
            {
                // does this item have a specified trade limitation?
                if (item.TradeLimit > 0)
                    return true;
            }

            if (item.GiftLimit.HasValue)
            {
                // does this item have a specified gift limitation?
                if (item.GiftLimit > 0)
                    return false; // TODO: Revert to true when ready
            }

            if (item.OnUse != null)
            {
                if (item.OnUse.Durability.HasValue)
                {
                    // can it be used more than once?
                    if (item.OnUse.Durability > 1)
                        return true;

                    // is this item left behind when the durability is broken?
                    if (!item.OnUse.DeleteOnBreak)
                        return true;
                }
            }

            return false;
        }

        public static Item GetItem(string id)
        {
            var items = Items.Where(x => x.Id == id);

            if (items.Count() > 1)
                throw new ArgumentException("There are more than one items with the specified ID.");

            return items.FirstOrDefault();
        }

        public static string NameOf(string itemId)
            => GetItem(itemId)?.Name ?? itemId;

        public static void TakeItem(ArcadeUser user, string itemId, int amount = 1)
            => TakeItem(user, GetItem(itemId), amount);

        public static void TakeItem(ArcadeUser user, Item item, int amount = 1)
        {
            if (user.Items.ContainsKey(item.Id))
            {
                // TODO: Handle unique item data removal
                if (user.Items[item.Id].Count - amount <= 0)
                {
                    user.Items.Remove(item.Id);
                }
                else
                {
                    user.Items[item.Id].StackCount -= amount;
                }
            }
        }

        public static void GiveItem(ArcadeUser user, string itemId, int amount = 1)
            => GiveItem(user, GetItem(itemId), amount);

        public static void GiveItem(ArcadeUser user, Item item, int amount = 1)
        {
            int currentAmount = 0;

            if (user.Items.ContainsKey(item.Id))
                currentAmount = user.Items[item.Id].Count;

            if (item.OwnLimit.HasValue)
            {
                if (currentAmount >= item.OwnLimit)
                {
                    return; // don't give them the item.
                }
            }

            if (currentAmount == 0)
            {
                user.Items[item.Id] = new ItemData { StackCount = amount };
            }
            else
            {
                user.Items[item.Id].StackCount += amount;
            }

            // If they somehow went over, this will fix that
            /*
            if (currentAmount > item.OwnLimit)
            {
                user.Items[item.Id].StackCount = item.OwnLimit;
                currentAmount = item.OwnLimit.Value;
            }
            */
        }

        public static void UseItem(ArcadeUser user, string itemId)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemService
    {
        public List<Item> Items { get; set; }

        public IEnumerable<Item> Search(ItemTag tag)
        {
            return Items.Where(x => x.Tag.HasFlag(tag));
        }

        public Item GetItem(string id)
        {
            var items = Items.Where(x => x.Id == id);

            if (items.Count() > 1)
                throw new ArgumentException("There are more than one items with the specified ID.");

            return items.FirstOrDefault();
        }
    }
}
