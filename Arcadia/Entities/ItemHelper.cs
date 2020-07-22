using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    public static class ItemHelper
    {
        /*
        public static long GetUniqueId()
        {
            var offset = new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow.Ticks - offset.Ticks);
        }*/

        public static string ItemOf(ArcadeUser user, string uniqueId)
        {
            if (user.Items.Any(x => x.Data?.Id == uniqueId))
                return "";

            return user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Id;
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
                        Action = user => user.Debt = 0
                    },
                    OwnLimit = 2,
                    MarketCriteria = null,
                    ToOwn = null,
                    ToUnlock = null,
                    ToExpire = null
                },
                new Item
                {
                    Id = "p_gg",
                    Name = "Card Palette: Gamma Green",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It glows with a shade of green similar to uranium."
                    },
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1000,
                    Size = 50,
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Common,
                    OnUse = new ItemAction
                    {
                        Action = user => user.Card.Palette = PaletteType.GammaGreen
                    },
                    MarketCriteria = null,
                    ToOwn = null,
                    ToUnlock = null,
                    ToExpire = null,
                    OwnLimit = 10
                },
                new Item
                {
                    Id = "p_cr",
                    Name = "Card Palette: Crimson",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It thrives in a neon glow of a reddish-purple hue."
                    },
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1000,
                    Size = 50,
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Common,
                    OnUse = new ItemAction
                    {
                        Action = user => user.Card.Palette = PaletteType.Crimson
                    },
                    MarketCriteria = null,
                    ToOwn = null,
                    ToUnlock = null,
                    ToExpire = null,
                    OwnLimit = 10
                },
                new Item
                {
                    Id = "p_wu",
                    Name = "Card Palette: Wumpite",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "Crafted with the shades of a bluish-purple pig-like entity."
                    },
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1500,
                    Size = 75,
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Uncommon,
                    OnUse = new ItemAction
                    {
                        Action = user => user.Card.Palette = PaletteType.Wumpite
                    },
                    MarketCriteria = null,
                    ToOwn = null,
                    ToUnlock = null,
                    ToExpire = null,
                    OwnLimit = 10
                },
                new Item
                {
                    Id = "p_gl",
                    Name = "Card Palette: Glass",
                    Summary = "A palette that can be equipped on a card.",
                    Quotes = new List<string>
                    {
                        "It refracts a mixture of light blue to white light."
                    },
                    Tag = ItemTag.Palette | ItemTag.Decorator,
                    Value = 1500,
                    Size = 75,
                    CanBuy = true,
                    CanSell = true,
                    BypassCriteriaOnGift = true,
                    Rarity =  ItemRarity.Uncommon,
                    OnUse = new ItemAction
                    {
                        Action = user => user.Card.Palette = PaletteType.Glass
                    },
                    MarketCriteria = null,
                    ToOwn = null,
                    ToUnlock = null,
                    ToExpire = null,
                    OwnLimit = 10
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

        public static DateTime? GetLastUsed(ArcadeUser user, string itemId)
        {
            var ticks = user.GetStat(GetCooldownId(itemId));

            if (ticks == 0)
                return null;

            return new DateTime(ticks);
        }

        internal static string GetCooldownId(string itemId)
            => $"{itemId}:last_used";

        public static TimeSpan? GetCooldownRemainder(ArcadeUser user, string itemId)
        {
            var item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.OnUse != null)
            {
                DateTime? lastUsed = GetLastUsed(user, itemId);

                if (item.OnUse.Cooldown.HasValue && lastUsed.HasValue)
                {
                    return (DateTime.UtcNow - lastUsed.Value.Add(item.OnUse.Cooldown.Value));
                }
            }

            return null;
        }

        public static bool CanUse(ArcadeUser user, string itemId, UniqueItemData data = null)
        {
            var item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.OnUse == null)
                return false;

            DateTime? lastUsed = GetLastUsed(user, itemId);

            if (lastUsed.HasValue && item.OnUse.Cooldown.HasValue)
            {
                // if the cooldown has expired, allow use.
                if ((DateTime.UtcNow - lastUsed.Value.Add(item.OnUse.Cooldown.Value)).TotalSeconds >= 0)
                    return true;
                else
                    return false;
            }

            if (data != null)
            {
                if (data.ExpiresOn.HasValue)
                    if (DateTime.UtcNow >= data.ExpiresOn.Value)
                        return false;

                if (data.Durability.HasValue)
                    if (data.Durability.Value == 0)
                        return false;
            }

            return true;
        }

        public static bool IsUnique(string itemId)
            => IsUnique(GetItem(itemId));

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

                if (item.OnUse.Cooldown.HasValue)
                    return true;
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

        public static bool Exists(string itemId)
            => Items.Where(x => x.Id == itemId).Count() > 0;

        public static string NameOf(string itemId)
            => GetItem(itemId)?.Name ?? itemId;

        public static void TakeItem(ArcadeUser user, string itemId, int amount = 1)
            => TakeItem(user, GetItem(itemId), amount);

        public static void TakeItem(ArcadeUser user, Item item, int amount = 1)
        {
            if (HasItem(user, item.Id))
            {
                // This method only works for non-unique items.
                if (IsUnique(item))
                    return;

                if (GetOwnedAmount(user, item) - amount <= 0)
                {
                    var slot = user.Items.First(x => x.Id == item.Id);
                    user.Items.Remove(slot);
                }
                else
                {
                    user.Items.First(x => x.Id == item.Id).StackCount -= amount;
                }
            }
        }

        public static void TakeItem(ArcadeUser user, Item item, string uniqueId)
        {
            if (HasItem(user, item.Id))
            {
                // This method only works for unique items.
                if (!IsUnique(item))
                    return;

                // Slim down to all matching item entries of the same item
                var matching = user.Items.Where(x => x.Id == item.Id);

                var match = matching.FirstOrDefault(x => x.Data.Id == uniqueId);

                if (match == null)
                    throw new Exception("Could not find a unique item with the specified ID.");

                user.Items.Remove(match);
            }
        }

        public static void GiveItem(ArcadeUser user, string itemId, int amount = 1)
            => GiveItem(user, GetItem(itemId), amount);

        public static int GetOwnedAmount(ArcadeUser user, string itemId)
            => GetOwnedAmount(user, GetItem(itemId));

        public static int GetOwnedAmount(ArcadeUser user, Item item)
        {
            if (HasItem(user, item.Id))
            {
                if (IsUnique(item))
                {
                    return user.Items.Where(x => x.Id == item.Id).Count();
                }
                else
                {
                    return user.Items.First().Count;
                }
            }

            return 0;
        }

        public static void GiveItem(ArcadeUser user, Item item, int amount = 1)
        {
            int ownedAmount = GetOwnedAmount(user, item);

            if (item.OwnLimit.HasValue)
            {
                if (ownedAmount >= item.OwnLimit)
                {
                    return; // don't give them the item.
                }
            }

            AddItem(user, item.Id, amount);

            // If they somehow went over, this will fix that
            /*
            if (currentAmount > item.OwnLimit)
            {
                user.Items[item.Id].StackCount = item.OwnLimit;
                currentAmount = item.OwnLimit.Value;
            }
            */
        }

        // this stores an item to the specified user.
        internal static void AddItem(ArcadeUser user, string itemId, int amount = 1)
            => AddItem(user, GetItem(itemId), amount);

        internal static void AddItem(ArcadeUser user, Item item, int amount = 1)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (IsUnique(item))
            {
                for (int i = 0; i < amount; i++)
                {
                    user.Items.Add(new ItemData(item.Id, GetUniqueData(item)));
                }
            }
            else
            {
                var selected = DataOf(user, item.Id);

                if (selected == null)
                {
                    user.Items.Add(new ItemData(item.Id, amount));
                }
                else
                {
                    selected.StackCount += amount;
                }


                //user.Items.First(x => x.Id == item.Id).StackCount += amount;
            }
        }

        public static UniqueItemData GetUniqueData(Item item)
        {
            if (!IsUnique(item))
                throw new Exception("The specified item is not unique.");

            var data = new UniqueItemData()
            {
                Durability = item.OnUse?.Durability,
                TradeCount = null,
                GiftCount = null,
                ExpiresOn = null,
                LastUsed = null
            };

            if (item.TradeLimit.HasValue)
                data.TradeCount = 0;

            if (item.GiftLimit.HasValue)
                data.GiftCount = 0;

            return data;
        }

        public static bool HasItem(ArcadeUser user, string itemId)
            => user.Items.Any(x => x.Id == itemId);

        public static UniqueItemData Peek(ArcadeUser user, string uniqueId)
            => user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Data;

        public static ItemData DataOf(ArcadeUser user, string itemId, string uniqueId = null)
        {
            if (!string.IsNullOrWhiteSpace(uniqueId))
                return user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId);

            return user.Items.FirstOrDefault(x => x.Id == itemId);
        }

        public static void UseItem(ArcadeUser user, string itemId, string uniqueId = null)
        {
            var item = GetItem(itemId);
            bool isBroken = false;

            // if there is no available action.
            if (item.OnUse == null)
                return;

            // if the user doesn't even have an item.
            if (!HasItem(user, itemId))
                return;

            // otherwise, check if the user can use the item
            if (IsUnique(item))
            {
                // If a unique ID wasn't specified, get the unique ID of the one closest to being broken.
                uniqueId ??= GetBestUniqueId(user, itemId);

                // There isn't an available item to use in this case.
                if (string.IsNullOrWhiteSpace(uniqueId))
                    return;

                var data = Peek(user, uniqueId);

                if (!CanUse(user, itemId, data))
                {
                    return;
                }

                // first, check if the item will be deleted on use
                if (item.OnUse.Durability.HasValue)
                {
                    if (item.OnUse.DeleteOnBreak)
                    {
                        // if it's going to be deleted on use, remove it
                        if (data.Durability - 1 <= 0)
                            TakeItem(user, item, uniqueId);
                    }

                    if (data.Durability > 0)
                        data.Durability--; // remove 1 from the durability.

                    if (data.Durability == 0)
                        isBroken = true;
                }
            }
            else
            {
                if (!CanUse(user, itemId))
                    return;

                if (item.OnUse.Durability == 1)
                {
                    if (item.OnUse.DeleteOnBreak)
                        TakeItem(user, item);

                    isBroken = true;
                }
            }

            if (item.OnUse.Cooldown.HasValue)
            {
                // finally, if all of the checks pass, use up the item.
                user.SetStat(GetCooldownId(itemId), DateTime.UtcNow.Ticks);
            }

            // As the final step, invoke the action defined on the item.
            item.OnUse.Action(user);

            // If the item broke, invoke that action too.
            if (isBroken && item.OnUse.OnBreak != null)
                item.OnUse.OnBreak(user);
        }

        private static string GetBestUniqueId(ArcadeUser user, string itemId)
        {
            var item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (!IsUnique(item))
                throw new ArgumentException("The specified item is not unique.");

            // Slim down to all matching item entries of the same item
            var matching = user.Items.Where(x => x.Id == itemId);

            if (matching.Count() == 0)
                return "";

            if (item.OnUse != null)
            {
                if (item.OnUse.Durability.HasValue)
                {
                    matching = matching.OrderBy(x => x.Data.Durability);
                }
            }

            return matching.First().Id;
        }
    }
}
