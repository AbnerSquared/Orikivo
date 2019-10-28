using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents a user from Orikivo.
    /// </summary>
    public class OriUser : IDiscordEntity<SocketUser>, IJsonEntity
    {
        private static readonly List<CurrencyData> _walletPlaceholder = new List<CurrencyData>
        {
            new CurrencyData(CurrencyType.Generic),
            new CurrencyData(CurrencyType.Vote)
        };

        [JsonConstructor]
        internal OriUser(ulong id, string username, string discriminator, DateTime createdAt, ulong? exp, List<CurrencyData> wallets,
            Dictionary<ulong, ulong> guildExp, Dictionary<string, ItemData> items, Dictionary<string, int> stats, Dictionary<string, MeritData> merits,
            Dictionary<string, int> upgrades, Dictionary<string, BoosterInfo> boosters, Dictionary<string, CooldownInfo> cooldowns, OriUserOptions options,
            GimiData gimi)
        {
            // TODO: Create Booster/CooldownHandler classes that can auto-manage deleting expired boosters and cooldowns.
            // find a way to auto-remove inactive items
            /*
            Dictionary<string, BoosterInfo> _boosters = null;
            Dictionary<string, CooldownInfo> _cooldowns = null;
            if (boosters != null)
            {
                _boosters = boosters;
                foreach (KeyValuePair<string, BoosterInfo> booster in boosters)
                    if (booster.Value.IsExpired)
                        _boosters.Remove(booster.Key);
            }
            if (cooldowns != null)
            {
                _cooldowns = cooldowns;
                foreach (KeyValuePair<string, CooldownInfo> cooldown in cooldowns)
                    if (!cooldown.Value.IsActive)
                        _cooldowns.Remove(cooldown.Key);
                cooldowns = _cooldowns;
            }*/

            Id = id;
            Username = username;
            Discriminator = discriminator;
            CreatedAt = createdAt;
            Wallets = wallets ?? _walletPlaceholder;
            Exp = exp ?? 0;
            GuildExp = guildExp ?? new Dictionary<ulong, ulong>();
            Items = items ?? new Dictionary<string, ItemData>();
            Stats = stats ?? new Dictionary<string, int>();
            Merits = merits ?? new Dictionary<string, MeritData>();
            Upgrades = upgrades ?? new Dictionary<string, int>();
            Boosters = boosters ?? new Dictionary<string, BoosterInfo>();
            Cooldowns = cooldowns ?? new Dictionary<string, CooldownInfo>();
            Options = options ?? OriUserOptions.Default;
            Gimi = gimi ?? new GimiData();
        }

        public OriUser(SocketUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
            CreatedAt = DateTime.UtcNow;
            Wallets = _walletPlaceholder;
            Exp = 0;
            GuildExp = new Dictionary<ulong, ulong>();
            Cooldowns = new Dictionary<string, CooldownInfo>();
            Stats = new Dictionary<string, int>();
            Merits = new Dictionary<string, MeritData>();
            Upgrades = new Dictionary<string, int>();
            Items = new Dictionary<string, ItemData>();
            Boosters = new Dictionary<string, BoosterInfo>();
            Options = OriUserOptions.Default;
            Gimi = new GimiData();
        }


        /// <summary>
        /// Represents the Discord Snowflake ID.
        /// </summary>
        [JsonProperty("id")]
        public ulong Id { get; }

        /// <summary>
        /// Represents the Discord username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; private set; }

        /// <summary>
        /// Represents the Discord discriminator.
        /// </summary>
        [JsonProperty("discriminator")]
        public string Discriminator { get; private set; }

        /// <summary>
        /// The date at which this account was created at.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("bal")]
        public ulong Balance => GetWalletFor(CurrencyType.Generic).Value;

        [JsonProperty("wallets")]
        public List<CurrencyData> Wallets { get; private set; }

        public CurrencyData GetWalletFor(CurrencyType type)
            => Wallets.First(x => x.Type == type);

        [JsonProperty("guild_bal")]
        public Dictionary<ulong, GuildCurrencyData> GuildBal { get; private set; }

        [JsonProperty("exp")]
        public ulong Exp { get; private set; }

        [JsonProperty("guild_exp")]
        public Dictionary<ulong, ulong> GuildExp { get; }

        [JsonProperty("stats")]
        public Dictionary<string, int> Stats { get; }

        [JsonProperty("merits")]
        public Dictionary<string, MeritData> Merits { get; }

        [JsonProperty("upgrades")]
        public Dictionary<string, int> Upgrades { get; }

        [JsonProperty("boosters")]
        public Dictionary<string, BoosterInfo> Boosters { get; }

        [JsonProperty("items")]
        public Dictionary<string, ItemData> Items { get; }

        // TODO: Split cooldowns between temporary cooldowns (commands) and literal cooldowns (item usage, dailies, voting)
        // TODO: Store cooldowns as <ID, ExpiresOn>; the extra information isn't required.
        [JsonProperty("cooldowns")]
        public Dictionary<string, CooldownInfo> Cooldowns { get; }

        // TODO: Transfer this to ExpHandler.
        [JsonIgnore]
        public double ExpGainRate
        {
            get
            {
                double boosterRate = 0.0;
                Boosters.Where(x => x.Key == GenericBooster.Exp && !x.Value.IsExpired).ToList().ForEach(x => boosterRate += x.Value.GainRate);
                return 1.0 * boosterRate; // combine all multipliers together, and then multiply by the base rate.
            }
        }

        // TODO: Transfer this to ExpHandler.
        [JsonIgnore]
        public double MoneyGainRate
        {
            get
            {
                double boosterRate = 0.0;
                Boosters.Where(x => x.Key == GenericBooster.Money && !x.Value.IsExpired).ToList().ForEach(x => boosterRate += x.Value.GainRate);
                return 1.0 * boosterRate;
            }
        }

        // TODO: Plan out experience and their types.
        public ulong GetGuildExp(ulong guildId)
            => GuildExp.TryGetValue(guildId, out ulong value) ? value : 0;

        public void UpdateGuildExp(ulong guildId, ulong value)
        {
            if (!GuildExp.TryAdd(guildId, value))
                GuildExp[guildId] += value;
        }

        public ulong GetGuildBal(ulong guildId)
            => GuildBal.TryGetValue(guildId, out GuildCurrencyData guildBal) ? guildBal.Value : 0;

        // TODO: This could honestly be scrapped, since the wallet now supports auto-debt and auto-balance upon values being added or subtracted.
        public void UpdateGuildBal(ulong guildId, ulong amount, bool isDebt = false)
        {
            GuildBal.TryAdd(guildId, new GuildCurrencyData(guildId));
            if (isDebt)
                GuildBal[guildId] -= amount;
            else
                GuildBal[guildId] += amount;
        }

        // TODO: Seperate accumulative information as stats, and dynamic information as attributes.
        // EXAMPLE: CommandsUsed == Stat; CurrentWinStreak == Attribute; LargestWinStreak == Stat;
        public int GetStat(string key)
            => Stats.ContainsKey(key) ? Stats[key] : 0;

        public void SetStat(string key, int value)
        {
            if (value == 0)
            {
                if (Stats.ContainsKey(key))
                    Stats.Remove(key);
            }
            else if (!Stats.TryAdd(key, value))
                Stats[key] = value;
        }

        public List<KeyValuePair<string, int>> GetStats(string type)
            => Stats.Where(x => x.Key.StartsWith(type)).ToList();

        public void UpdateStat(string key, int value)
        {
            if (!Stats.TryAdd(key, value))
                Stats[key] += value;
        }

        // inventory
        public bool HasItem(string key)
            => Items.ContainsKey(key);

        public void AddItem(string key, int amount = 1)
        {
            // TODO:  check if the user has the item in the first place.
            // After that, check if the UniqueItemData are exact matches to the one being set.
            // If they are, add one to the StackCount.
            // If they aren't create a new item slot with the UniqueItemData.
        }

        public void RemoveItem(string key, int amount = 1)
        {
            // implement IsStackable
            if (Items.ContainsKey(key))
            {
                if (Items[key].Count - amount < 1)
                    Items.Remove(key);
                else
                    Items[key].StackCount -= amount;
            }
        }

        /// <summary>
        /// Checks to see if the user has a specified upgrade.
        /// </summary>
        public bool HasUpgrade(string key)
            => Upgrades.ContainsKey(key);

        /// <summary>
        /// Checks to see if the user has a specified upgrade at a certain tier.
        /// </summary>
        public bool HasUpgradeAt(string key, int value)
            => HasUpgrade(key) ? Upgrades[key] == value : false;

        // TODO: Make StatCriterion. UPNEXT: Handle this at UserHandler.
        public bool MeetsStatCriteria(params (string, int)[] criteria)
        {
            foreach ((string key, int count) in criteria)
            {
                if (!(Stats.ContainsKey(key) ? Stats[key] == count : false))
                    return false;
            }
            return true;
        }

        // TODO: ItemCriterion, handle at UserHandler.
        public bool HasItemAt(string key, int value)
            => Items.ContainsKey(key) ? Items[key].Count == value : false;

        /// <summary>
        /// Checks to see if the user has a specified merit.
        /// </summary>
        public bool HasMerit(string key)
            => Merits.ContainsKey(key);

        /// <summary>
        /// Attempts to get the level of an upgrade on a user. If one isn't found, it returns as 0.
        /// </summary>
        public int GetUpgrade(string key)
            => HasUpgrade(key) ? Upgrades[key] : 0;

        // TODO: Create CooldownHandler // Literal Cooldown
        // TODO: Create CooldownHandler // Process Cooldown
        /// <summary>
        /// Sets or updates a cooldown for a user.
        /// </summary>
        public void SetCooldown(string key, double seconds)
        {
            if (!Cooldowns.TryAdd(key, new CooldownInfo(DateTime.UtcNow, seconds)))
                Cooldowns[key] = new CooldownInfo(DateTime.UtcNow, seconds);
        }

        /// <summary>
        /// Gets all currently set cooldowns for a specified type.
        /// </summary>
        public List<KeyValuePair<string, CooldownInfo>> GetCooldownsFor(CooldownType type)
            => Cooldowns.Where(x => x.Key.StartsWith(type.ToString().ToLower())).ToList();

        /// <summary>
        /// Checks to see if the user has a currently existing cooldown in place.
        /// </summary>
        public bool IsOnCooldown(string key)
            => Cooldowns.ContainsKey(key) ? Cooldowns[key].IsActive : false;

        
        /// <summary>
        /// Defines the last time a user has saved, if they have.
        /// </summary>
        [JsonIgnore]
        public DateTime? LastSaved { get; internal set; }

        /// <summary>
        /// Returns the default Discord username format.
        /// </summary>
        [JsonIgnore]
        public string DefaultName => $"{Username}#{Discriminator}";

        // TODO: Don't see too much use of this yet.
        [JsonIgnore]
        public string Name => Checks.NotNull(Options.Nickname) ? Options.Nickname : Username;

        /// <summary>
        /// The configurations for this user.
        /// </summary>
        [JsonProperty("options")]
        public OriUserOptions Options { get; private set; }

        // TODO: This could most likely just be decompiled as stats. Even more so, as attributes.
        [JsonProperty("gimi")]
        public GimiData Gimi { get; }

        // TODO: Make this formatting separate from the user.
        public MessageBuilder GetDisplay(EntityDisplayFormat displayFormat)
        {
            MessageBuilder oriMessage = new MessageBuilder();
            switch (displayFormat)
            {
                case EntityDisplayFormat.Json:
                    StringBuilder sbj = new StringBuilder();
                    sbj.AppendLine("```json");
                    sbj.AppendLine("{");
                    sbj.AppendLine($"    \"id\": \"{Id}\",");
                    sbj.AppendLine($"    \"username\": \"{Username}\",");
                    sbj.AppendLine($"    \"discriminator\": \"{Discriminator}\",");
                    sbj.AppendLine($"    \"created_at\": \"{CreatedAt}\"");
                    sbj.AppendLine("}```");
                    oriMessage.Content = sbj.ToString();
                    return oriMessage;
                default:
                    StringBuilder sbd = new StringBuilder();
                    sbd.AppendLine($"**{Username}**#{Discriminator}"); // name display
                    sbd.AppendLine($"• {Id}");// id display
                    sbd.AppendLine($"\n**Joined**: {CreatedAt.ToString($"`MM`.`dd`.`yyyy` **@** `hh`:`mm`:`ss`")}");
                    oriMessage.Content = sbd.ToString();
                    return oriMessage;
            }
        }

        public bool TryGetSocketEntity(BaseSocketClient client, out SocketUser user)
        { 
            user = client.GetUser(Id);
            return user != null;
        }

        public override bool Equals(object obj)
            => ReferenceEquals(null, obj) || obj == null || GetType() != obj.GetType() ? false :
            ReferenceEquals(this, obj) ? true : Equals(obj as IJsonEntity);

        public bool Equals(IJsonEntity obj)
            => Id == obj.Id;

        public override int GetHashCode()
            => unchecked((int)Id);
    }
}
