using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Graphics;
using JetBrains.Annotations;
using Orikivo;

namespace Arcadia
{
    // TODO: Implement item attribute reading and data population
    public static class ItemHelper
    {
        public static readonly DateTime UniqueIdOffset = new DateTime(2020, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc);

        public static bool Exists(string itemId)
            => Assets.Items.Any(x => x.Id == itemId);

        public static bool GroupExists(string id)
            => Check.NotNull(id)
            && (Assets.Groups.Any(x => x.Id == id)
            || Assets.Groups.Any(x => x.Icon?.Equals(id) ?? false));

        public static string GetPreview(string itemId, int amount)
        {
            string icon = IconOf(itemId) ?? "•";
            string name = Check.NotNull(icon) ? GetBaseName(itemId) : NameOf(icon);
            string counter = amount > 1 ? $" (x**{amount:##,0}**)" : "";
            return $"`{itemId}` {icon} **{name}**{counter}";
        }

        public static ItemData CreateData(Item item, int amount = 1, string sealId = null)
        {
            if (!Check.NotNull(sealId))
            {
                return new ItemData(item.Id, amount);
            }

            if (!Exists(sealId))
                throw new ArgumentException("Could not find an item reference for the specified seal ID");

            return new ItemData(item.Id, false, amount, IsUnique(item) ? CreateUniqueData(item) : null, new ItemSealData(sealId));
        }

        // item.Value > 0
        // TODO: Include user to determine if they can buy the item based on their current progress
        public static bool CanBuy(Item item)
        {
            return Assets.Shops.Any(x => x.Catalog.Entries.Any(c => c.ItemId == item.Id));
        }

        public static bool DeleteItem(ArcadeUser user, ItemData data)
        {
            return user.Items.RemoveAll(x => x.TempId == data.TempId) > 0;
        }

        public static bool CanDelete(ItemData data)
        {
            if (!Exists(data.Id))
                return true;

            return GetItem(data.Id).Tags.HasFlag(ItemTag.Disposable);
        }

        public static bool CanSell(Item item)
        {
            return item.Value > 0
                && Assets.Shops.Any(s => s.Sell && s.AllowedSellGroups.Contains(item.GroupId));
        }

        // This just reads the boost multiplier
        public static DateTime? GetLastUsed(ArcadeUser user, string itemId, string uniqueId = null)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage == null)
                throw new Exception("Expected item usage to be specified but returned null");

            string statGroup = item.Usage?.CooldownTarget switch
            {
                CooldownTarget.Instance => uniqueId ?? throw new Exception("Expected a unique item reference but was unspecified"),
                CooldownTarget.Item => itemId,
                CooldownTarget.Group => item.GroupId ?? throw new Exception("Expected an item group but was empty"),
                CooldownTarget.Global => "global",
                _ => null
            };

            if (statGroup == null)
                return null;

            if (statGroup == uniqueId)
            {
                ItemData data = GetFromInventory(user, itemId, uniqueId);
                return data.Data.LastUsed;
            }

            var ticks = user.GetVar(GetCooldownId(statGroup));

            if (ticks == 0)
                return null;

            return new DateTime(ticks);
        }

        public static TimeSpan? GetCooldownRemainder(ArcadeUser user, string itemId, string uniqueId = null)
        {
            Item item = GetItem(itemId);

            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage != null)
            {
                DateTime? lastUsed = GetLastUsed(user, itemId, uniqueId);

                if (item.Usage.Cooldown.HasValue && lastUsed.HasValue)
                {
                    return DateTime.UtcNow - lastUsed.Value.Add(item.Usage.Cooldown.Value);
                }
            }

            return null;
        }

        public static string GetBaseName(string itemId)
            => GetItem(itemId)?.Name ?? itemId;

        public static ItemTag GetTag(string itemId)
            => GetItem(itemId)?.Tags ?? 0;

        public static long CreateUniqueId()
            => (DateTime.UtcNow - UniqueIdOffset).Ticks;

        public static IEnumerable<Item> GetItemsInGroup(string groupId)
        {
            return Assets.Items.Where(x => x.GroupId == groupId);
        }

        public static ItemGroup GetGroup(string id)
        {
            if (Assets.Groups.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one groups with the specified ID.");

            return Assets.Groups.FirstOrDefault(x => x.Id == id);
        }

        public static string GetGroupIcon(string groupId)
        {
            return GetGroup(groupId)?.Icon?.ToString() ?? "";
        }

        public static ItemData GetBestStack(ArcadeUser user, Item item)
            => user.Items.FirstOrDefault(x => CanStack(x) && x.Id == item.Id);

        public static ItemData AddOrSetToStack(ArcadeUser user, Item item, int amount)
        {
            ItemData stack = GetBestStack(user, item);

            if (stack != null)
            {
                stack.StackCount += amount;
                return stack;
            }

            stack = new ItemData(item.Id, amount);
            user.Items.Add(stack);

            return stack;
        }

        public static IEnumerable<ItemData> GetFromInventory(ArcadeUser user, string itemId)
            => user.Items.Where(x => x.Id == itemId);

        public static ItemData GetFromInventory(ArcadeUser user, string itemId, string uniqueId)
            => user.Items.First(x => x.Id == itemId && x.Data != null && x.Data.Id == uniqueId);

        public static bool TryGetGroup(string id, out ItemGroup group)
        {
            group = null;

            if (!GroupExists(id))
                return false;

            group = GetGroup(id);
            return true;
        }

        public static bool TryGetItem(string id, out Item item)
        {
            item = null;

            if (!Exists(id))
                return false;

            item = GetItem(id);
            return true;
        }

        public static ItemData SetInStack(ArcadeUser user, Item item, int amount = 1)
        {
            if (amount <= 0)
                throw new Exception("Expected item amount to be larger than 0");

            ItemData best = user.Items.FirstOrDefault(CanStack);

            if (best != null)
            {
                best.StackCount += amount;
                return best;
            }

            // If no available stacks are found, a new stack is made instead.
            best = new ItemData(item.Id, amount);
            user.Items.Add(best);

            return best;
        }

        public static ItemData GetItemData(ArcadeUser user, string dataId)
        {
            if (!Check.NotNull(dataId))
                return null;

            bool isUniqueId = user.Items.Any(x => x.Seal == null && x.Data?.Id == dataId);

            if (!isUniqueId)
            {
                if (user.Items.Any(x => x.TempId == dataId))
                    return user.Items.First(x => x.TempId == dataId);

                if (Exists(dataId))
                    return GetBestStack(user, GetItem(dataId));
            }


            if (!Exists(dataId) && !isUniqueId)
            {
                return null;
            }

            string itemId = isUniqueId ? ItemOf(user, dataId).Id : dataId;
            string uniqueId = isUniqueId ? dataId : null;

            bool isUnique = IsUnique(itemId);

            if (isUnique && !isUniqueId)
            {
                string uId = GetBestUniqueId(user, itemId);

                if (!Check.NotNull(uId))
                    return null;

                return DataOf(user, itemId, uId);
            }

            return DataOf(user, itemId, uniqueId);
        }

        public static bool RemovePalette(ArcadeUser user)
        {
            if (user.Card.Palette == PaletteType.Default)
                return false;

            GiveItem(user, IdFor(user.Card.Palette.Primary, user.Card.Palette.Secondary));
            user.Card.Palette = PaletteType.Default;
            return true;
        }

        public static bool IsUnique(string itemId)
            => IsUnique(GetItem(itemId));

        public static bool IsUnique(Item item)
        {
            if (item.TradeLimit.HasValue)
                if (item.TradeLimit > 0)
                    return true;

            // is there any form of usage specified?
            if (item.Usage == null)
                return false;

            // can it be used more than once?
            if (item.Usage.Durability > 1)
                return true;

            // is this item left behind when the durability is broken?
            if (item.Usage.Durability.HasValue && !item.Usage.DeleteMode.HasFlag(DeleteTriggers.Break))
                return true;

            // does this item have a cooldown that is applied to the instance of an item?
            if (item.Usage.Cooldown.HasValue && item.Usage.CooldownTarget == CooldownTarget.Instance)
                return true;

            return false;
        }

        public static bool IsIngredient(Item item)
            => Assets.Recipes.Any(x => x.Components.Any(c => c.ItemId == item.Id));

        public static Item GetItem(string id)
        {
            if (Assets.Items.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one items with the specified ID.");

            return Assets.Items.FirstOrDefault(x => x.Id == id);
        }

        public static string NameFor(FontType font)
            => GetItem(IdFor(font))?.Name;

        public static string NameFor(PaletteType palette, PaletteType? secondary = null)
            => GetItem(IdFor(palette, secondary))?.Name;

        public static string IdFor(FontType font)
        {
            return font switch
            {
                FontType.Delta => Ids.Items.FontDelta,
                _ => null
            };
        }

        public static string IdFor(PaletteType palette, PaletteType? secondary = null)
        {
            return palette switch
            {
                PaletteType.GammaGreen => Ids.Items.PaletteGammaGreen,

                PaletteType.Crimson => Ids.Items.PaletteCrimson,

                PaletteType.Glass when secondary == PaletteType.Wumpite => Ids.Items.PaletteGlossyWumpite,
                PaletteType.Glass => Ids.Items.PaletteGlass,

                PaletteType.Wumpite when secondary == PaletteType.Glass => Ids.Items.PaletteGlossyWumpite,
                PaletteType.Wumpite => Ids.Items.PaletteWumpite,
                _ => null
            };
        }

        public static string NameOf(string id)
        {
            if (!TryGetItem(id, out Item item))
                return id;

            if (TryGetGroup(item.GroupId, out ItemGroup group))
                return $"{group.Prefix}{item.Name}";

            return item.Name;
        }

        public static string NameOf(ItemData data)
        {
            if (Check.NotNull(data.Data?.Name))
                return data.Data?.Name;

            if (data.Seal != null)
                return NameOf(data.Seal.ReferenceId);

            string icon = IconOf(data);

            if (Check.NotNull(icon))
                return GetBaseName(data.Id);

            return NameOf(data.Id);
        }

        public static string GroupOf(string itemId)
        {
            if (!TryGetItem(itemId, out Item item))
                return null;

            return item.GroupId;
        }

        public static string IconOf(string itemId)
            => GetItem(itemId)?.GetIcon() ?? "";

        public static string IconOf(ItemData data)
        {
            if (data.Seal != null)
                return IconOf(data.Seal.ReferenceId);

            return IconOf(data.Id);
        }

        public static long SizeOf(string itemId)
            => GetItem(itemId)?.Size ?? 0;

        public static Item ItemOf(ArcadeUser user, string uniqueId)
        {
            if (user.Items.All(x => x.Data?.Id != uniqueId))
                return null;

            return GetItem(user.Items.FirstOrDefault(x => x.Data?.Id == uniqueId)?.Id);
        }

        public static ItemData DataOf(ArcadeUser user, string itemId, string uniqueId = null)
        {
            return user.Items.FirstOrDefault(x =>
                (!string.IsNullOrWhiteSpace(uniqueId) ? x.Data?.Id == uniqueId && x.Id == itemId : x.Id == itemId)
                && x.Seal == null);
        }

        public static ItemData DataOf(ArcadeUser user, Item item, string uniqueId = null)
            => DataOf(user, item.Id, uniqueId);

        public static void TakeItem(ArcadeUser user, string itemId, int amount = 1)
            => TakeItem(user, GetItem(itemId), amount);

        public static void TakeItem(ArcadeUser user, ItemData data, int amount = 1)
            => TakeItem(user, GetItem(data.Id), amount);

        public static void TakeItem(ArcadeUser user, Item item, int amount = 1)
        {
            if (!HasItem(user, item.Id))
                return;

            if (IsUnique(item))
            {
                int available = user.Items.Count(x => x.Id == item.Id);

                if (available < amount)
                    amount = available;

                for (int i = 0; i < amount; i++)
                {
                    string uniqueId = GetBestUniqueId(user, item.Id);

                    if (uniqueId == null)
                        throw new Exception("Expected to find an item ID but returned null");

                    TakeItem(user, item, uniqueId);
                }
                return;
            }

            if (GetOwnedAmount(user, item) - amount <= 0)
            {
                ItemData slot = user.Items.First(x => x.Id == item.Id);
                user.Items.Remove(slot);
            }
            else
            {
                user.Items.First(x => x.Id == item.Id).StackCount -= amount;
            }
        }

        public static void TakeItem(ArcadeUser user, Item item, string uniqueId)
        {
            if (!HasItem(user, item.Id))
                return;

            // This method only works for unique items.
            if (!IsUnique(item))
                return;

            ItemData match = user.Items.FirstOrDefault(x => x.Id == item.Id && x.Data.Id == uniqueId);

            if (match == null)
                throw new Exception("Could not find a unique item with the specified ID.");

            user.Items.Remove(match);
        }

        public static int GetOwnedAmount(ArcadeUser user, string itemId)
            => GetOwnedAmount(user, GetItem(itemId));

        public static int GetOwnedAmount(ArcadeUser user, Item item)
        {
            if (!HasItem(user, item.Id))
                return 0;

            if (IsUnique(item))
                return user.Items.Count(x => x.Id == item.Id);

            return user.Items.First(x => x.Id == item.Id).Count;
        }

        public static bool CanStack(ItemData data)
            => (data.Seal == null) & (data.Data == null);

        public static bool CanGift(string itemId, ItemData data = null)
            => CanGift(GetItem(itemId), data);

        public static bool CanGift(Item item, ItemData data = null)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (data?.Seal != null)
            {
                if (data.Seal.SenderId.HasValue || Check.NotNullOrEmpty(data.Seal.ToUnlock))
                    return false;

                return true;
            }

            return !item.TradeLimit.HasValue || (data?.Data?.TradeCount ?? 0) < item.TradeLimit.Value;
        }

        public static bool CanGift(ArcadeUser target, string itemId, ItemData data = null)
            => CanGift(target, GetItem(itemId), data);

        public static bool CanGift(ArcadeUser target, Item item, ItemData data = null)
        {
            if (!CanGift(item, data))
                return false;

            return item.BypassCriteriaOnTrade || (item.ToOwn?.Invoke(target) ?? true);
        }

        public static bool CanTrade(ItemData data)
            => CanTrade(GetItem(data.Id), data);

        public static bool CanTrade(string itemId, ItemData data = null)
            => CanTrade(GetItem(itemId), data);

        public static bool CanTrade(Item item, ItemData data = null)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (data?.Seal != null)
            {
                return false;
            }

            return !item.TradeLimit.HasValue || (data?.Data?.TradeCount ?? 0) < item.TradeLimit.Value;
        }

        public static bool CanUse(ArcadeUser user, string itemId, ItemData data = null)
            => CanUse(user, GetItem(itemId), data);

        public static bool CanUse(ArcadeUser user, Item item, ItemData data = null)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (item.Usage == null)
                return false;

            DateTime? lastUsed = GetLastUsed(user, item.Id);

            if (lastUsed.HasValue && item.Usage.Cooldown.HasValue)
            {
                // if the cooldown has expired, allow use.
                if (DateTime.UtcNow - lastUsed.Value.Add(item.Usage.Cooldown.Value) >= TimeSpan.Zero)
                    return true;

                return false;
            }

            if (data?.Data != null)
            {
                if (data.Data.ExpiresOn.HasValue)
                    if (DateTime.UtcNow >= data.Data.ExpiresOn.Value)
                        return false;

                if (data.Data.Durability <= 0)
                    return false;
            }

            return true;
        }

        public static bool HasItem(ArcadeUser user, string itemId)
            => user.Items.Any(x => x.Id == itemId);

        public static bool HasItem(ArcadeUser user, Item item)
            => HasItem(user, item.Id);

        public static bool HasItem(ArcadeUser user, Item item, string uniqueId)
            => HasItem(user, item.Id, uniqueId);

        public static bool HasItem(ArcadeUser user, string itemId, string uniqueId)
            => user.Items.Any(x => x.Id == itemId && x.Data != null && x.Data.Id == uniqueId);

        public static bool HasItemWhen(ArcadeUser user, string itemId, Func<ItemData, bool> criterion)
            => HasItem(user, itemId) && user.Items.Any(criterion);

        public static bool HasItemWhen(ArcadeUser user, Item item, Func<ItemData, bool> criterion)
            => HasItem(user, item) && user.Items.Any(criterion);

        public static UsageResult UseItem(ArcadeUser user, ItemData data, string input = null)
        {
            var isBroken = false;
            Item item = GetItem(data.Id);

            // If a usage isn't specified
            if (item.Usage == null)
                return UsageResult.FromError(Format.Warning("This item does not have functionality."));

            // If the item is sealed
            if (data.Seal != null)
            {
                // If the item has requirements to unlock
                if (Check.NotNullOrEmpty(data.Seal.ToUnlock))
                {
                    foreach ((string id, VarProgress progress) in data.Seal.ToUnlock)
                        if (progress.Current < progress.Required)
                            return UsageResult.FromError(Format.Warning("You have not met all of the criteria needed to open this seal."));
                }

                ItemData stack = GetBestStack(user, item);

                if (stack != null && !IsUnique(item))
                {
                    stack.StackCount += data.StackCount;
                    user.Items.Remove(data);
                }
                else
                {
                    data.Seal = null;
                }

                return UsageResult.FromSuccess($"> You have released the seal to discover:\n{IconOf(data.Id) ?? "•"} **{GetBaseName(data.Id)}**");
            }

            // If the item has unique properties
            if (data.Data != null)
            {
                // If the item has a cooldown
                TimeSpan? remainder = GetCooldownRemainder(user, item.Id, data.Data.Id);

                if (remainder.HasValue)
                    return UsageResult.FromError(
                        $"This item is on cooldown and can be used in {Format.Counter(remainder.Value.TotalSeconds)}.");

                // If the item has expired
                if (data.Data.ExpiresOn.HasValue)
                    if (DateTime.UtcNow >= data.Data.ExpiresOn.Value)
                        return UsageResult.FromError(Format.Warning("This item has expired and cannot be used."));

                // If the item is broken
                if (data.Data.Durability <= 0)
                {
                    if (item.Usage.DeleteMode.HasFlag(DeleteTriggers.Break))
                    {
                        user.Items.Remove(data);
                        return UsageResult.FromError(Format.Warning("This item is broken and will be removed."));
                    }

                    return UsageResult.FromError(Format.Warning("This item is broken and cannot be used."));
                }
            }

            // Attempt to use the item
            UsageResult result = item.Usage.Action(new UsageContext(user, input, data));

            // If the item failed on use, return its result
            if (!result.IsSuccess)
                return result;

            // Apply durability effect
            if (item.Usage.Durability.HasValue)
            {
                if (data.Data != null)
                {
                    if (item.Usage.DeleteMode.HasFlag(DeleteTriggers.Break))
                    {
                        // if it's going to be deleted on use, remove it
                        if (data.Data.Durability - 1 <= 0)
                            TakeItem(user, item, data.Data.Id);
                    }

                    if (data.Data.Durability > 0)
                        data.Data.Durability--; // remove 1 from the durability.

                    if (data.Data.Durability <= 0)
                        isBroken = true;
                }
                else
                {
                    if (item.Usage.Durability == 1)
                    {
                        if (item.Usage.DeleteMode.HasFlag(DeleteTriggers.Break))
                        {
                            data.StackCount -= 1;

                            if (data.StackCount <= 0)
                                user.Items.Remove(data);
                        }

                        isBroken = true;
                    }
                }
            }

            // Apply item cooldowns
            if (item.Usage.Cooldown.HasValue)
            {
                if (item.Usage.CooldownTarget == CooldownTarget.Instance)
                {
                    if (data.Data == null)
                        throw new Exception("Expected an item to be unique");

                    data.Data.LastUsed = DateTime.UtcNow;
                }
                else if (item.Usage.CooldownTarget == CooldownTarget.Group)
                {
                    user.SetVar($"{item.GroupId}:last_used", DateTime.UtcNow.Ticks);
                }
                else if (item.Usage.CooldownTarget == CooldownTarget.Item)
                {
                    // finally, if all of the checks pass, use up the item.
                    user.SetVar(GetCooldownId(item.Id), DateTime.UtcNow.Ticks);
                }
                else
                {
                    user.SetVar("cooldown:item", DateTime.UtcNow.Ticks);
                }
            }

            // Apply break mechanics
            if (isBroken && item.GroupId != Ids.Groups.Booster)
            {
                // Only invoke breaking if the group is not a booster
                // This is because boosters reference this method when they are used up
                item.Usage.OnBreak?.Invoke(user);
                user.AddToVar(Stats.Common.ItemsBroken);
            }

            return result;
        }

        public static UsageResult UseItem(ArcadeUser user, Item item, string uniqueId, string input = null)
            => UseItem(user, DataOf(user, item.Id, uniqueId), input);

        public static UsageResult UseItem(ArcadeUser user, string itemId, string uniqueId, string input)
            => UseItem(user, DataOf(user, itemId, uniqueId), input);

        public static UsageResult UseItem(ArcadeUser user, string dataId, string input = null)
            => UseItem(user, GetItemData(user, dataId), input);

        public static void GiveItem(ArcadeUser user, string itemId, int amount = 1)
            => GiveItem(user, GetItem(itemId), amount);

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
            CatalogHelper.SetCatalogStatus(user, item, CatalogStatus.Known);
            // If they somehow went over, this will fix that
            /*
            if (currentAmount > item.OwnLimit)
            {
                user.Items[item.Id].StackCount = item.OwnLimit;
                currentAmount = item.OwnLimit.Value;
            }
            */
        }

        internal static void AddItem(ArcadeUser user, ItemData data)
        {
            user.Items.Add(data);
        }

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
                    user.Items.Add(new ItemData(item.Id, CreateUniqueData(item)));
                }
            }
            else
                AddOrSetToStack(user, item, amount);
        }

        // This clamps all stacks in an inventory to their specified owning limit.
        public static void ClampItemStacks(ArcadeUser user)
        {

            foreach (ItemData data in user.Items.Where(x => x.Seal == null
                && Exists(x.Id)
                && GetItem(x.Id).OwnLimit.HasValue).ToList())
            {
                int ownLimit = GetItem(data.Id).OwnLimit ?? -1;

                if (ownLimit >= 0 && data.Count > ownLimit)
                {
                    data.StackCount = ownLimit;
                }

                if (data.Count == 0)
                    TakeItem(user, data);
            }
        }

        private static UniqueItemData CreateUniqueData(Item item)
        {
            if (!IsUnique(item))
                throw new Exception("The specified item is not unique.");

            var data = new UniqueItemData
            {
                Durability = item.Usage?.Durability,
                // ExpiresOn = (item.Usage?.ExpiryTrigger ?? 0)
                ExpiresOn = (item.Usage?.ExpireTriggers ?? 0) == ExpireTriggers.Own && (item.Usage?.Timer.HasValue ?? false)
                ? DateTime.UtcNow.Add(item.Usage.Timer.Value)
                : (DateTime?)null
            };

            // No need to initialize if the item cannot be traded
            if (item.TradeLimit.HasValue && item.TradeLimit > 0)
                data.TradeCount = 0;

            return data;
        }

        private static DateTime? GetExpiry(TimeSpan duration, ExpireTriggers trigger)
        {
            if (trigger == ExpireTriggers.Own)
                return DateTime.UtcNow.Add(duration);

            return null;
        }

        private static string GetBestUniqueId(ArcadeUser user, string itemId)
            => GetBestUniqueId(user, GetItem(itemId));

        private static string GetBestUniqueId(ArcadeUser user, Item item)
        {
            if (item == null)
                throw new ArgumentException("Could not find an item with the specified ID.");

            if (!IsUnique(item))
                throw new ArgumentException("The specified item is not unique.");

            // Slim down to all matching item entries of the same item
            IEnumerable<ItemData> matching = user.Items.Where(x => x.Id == item.Id && !x.Locked);

            if (user.Items.All(x => x.Id != item.Id))
                return null;

            if (item.Usage?.Durability != null)
                matching = matching.OrderBy(x => x.Data.Durability);

            return matching.First().Id;
        }

        private static string GetCooldownId(string itemId)
            => $"{itemId}:last_used";
    }
}
