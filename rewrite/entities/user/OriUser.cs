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
        private static readonly Dictionary<CurrencyType, WalletData> _emptyWallet = new Dictionary<CurrencyType, WalletData>
        {
            [CurrencyType.Generic] = new WalletData(CurrencyType.Generic),
            [CurrencyType.Vote] = new WalletData(CurrencyType.Vote)
        };

        [JsonConstructor]
        internal OriUser(
            ulong id,
            string username,
            string discriminator,
            DateTime createdAt,
            ulong? exp,
            Dictionary<CurrencyType, WalletData> wallets,
            Dictionary<ulong, ulong> guildExp,
            Dictionary<string, ItemData> items,
            Dictionary<string, int> stats,
            Dictionary<string, MeritData> merits,
            Dictionary<string, int> upgrades,
            Dictionary<string, BoostData> boosters,
            Dictionary<string, DateTime> cooldowns,
            UserOptions options)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
            CreatedAt = createdAt;
            Wallets = wallets ?? _emptyWallet;
            Exp = exp ?? 0;
            GuildExp = guildExp ?? new Dictionary<ulong, ulong>();
            Items = items ?? new Dictionary<string, ItemData>();
            Stats = stats ?? new Dictionary<string, int>();
            Merits = merits ?? new Dictionary<string, MeritData>();
            Upgrades = upgrades ?? new Dictionary<string, int>();
            Boosters = boosters ?? new Dictionary<string, BoostData>();
            Cooldowns = cooldowns ?? new Dictionary<string, DateTime>();
            ProcessCooldowns = new Dictionary<string, DateTime>();
            Options = options ?? UserOptions.Default;
        }

        public OriUser(SocketUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
            CreatedAt = DateTime.UtcNow;
            Wallets = _emptyWallet;
            Exp = 0;
            GuildExp = new Dictionary<ulong, ulong>();
            Cooldowns = new Dictionary<string, DateTime>();
            ProcessCooldowns = new Dictionary<string, DateTime>();
            Stats = new Dictionary<string, int>();
            Merits = new Dictionary<string, MeritData>();
            Upgrades = new Dictionary<string, int>();
            Items = new Dictionary<string, ItemData>();
            Boosters = new Dictionary<string, BoostData>();
            Options = UserOptions.Default;
        }

        /// <summary>
        /// The unique identifier for the current user.
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

        [JsonProperty("wallets")]
        public Dictionary<CurrencyType, WalletData> Wallets { get; private set; }

        [JsonProperty("guild_wallets")]
        public Dictionary<ulong, GuildWalletData> GuildWallets { get; private set; }

        /// <summary>
        /// Experience earned globally.
        /// </summary>
        [JsonProperty("exp")]
        public ulong Exp { get; private set; }

        /// <summary>
        /// Local experience earned across all guilds.
        /// </summary>
        [JsonProperty("guild_exp")]
        public Dictionary<ulong, ulong> GuildExp { get; }

        /// <summary>
        /// Incremental variables that track user progression.
        /// </summary>
        [JsonProperty("stats")]
        public Dictionary<string, int> Stats { get; }

        /// <summary>
        /// Dynamic variables that define service data.
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<string, int> Attributes { get; }

        /// <summary>
        /// All information about merits that the user has earned.
        /// </summary>
        [JsonProperty("merits")]
        public Dictionary<string, MeritData> Merits { get; }

        /// <summary>
        /// All information about upgrades the the user currently has.
        /// </summary>
        [JsonProperty("upgrades")]
        public Dictionary<string, int> Upgrades { get; }

        /// <summary>
        /// All information about boosters that the user currently has.
        /// </summary>
        [JsonProperty("boosters")]
        public Dictionary<string, BoostData> Boosters { get; }

        [JsonProperty("items")]
        public Dictionary<string, ItemData> Items { get; }

        // TODO: Split cooldowns between temporary cooldowns (commands) and literal cooldowns (item usage, dailies, voting)
        // TODO: Store cooldowns as <ID, ExpiresOn>; the extra information isn't required.
        [JsonProperty("cooldowns")]
        public Dictionary<string, DateTime> Cooldowns { get; }

        /// <summary>
        /// The configurations for this user.
        /// </summary>
        [JsonProperty("options")]
        public UserOptions Options { get; private set; }

        /// <summary>
        /// Defines the last time a user has saved, if they have.
        /// </summary>
        [JsonIgnore]
        public DateTime? LastSaved { get; internal set; }

        // TODO: Don't see too much use of this yet.
        [JsonIgnore]
        public string Name => Checks.NotNull(Options.Nickname) ? Options.Nickname : Username;
        // Key, ExpiresOn

        [JsonIgnore]
        public ulong Balance => GetWallet(CurrencyType.Generic).Value;

        [JsonIgnore]
        public ulong Debt => GetWallet(CurrencyType.Generic).Value;

        /// <summary>
        /// A collections of cooldowns specific to the currently running process.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, DateTime> ProcessCooldowns { get; } = new Dictionary<string, DateTime>();

        public WalletData GetWallet(CurrencyType type)
        {
            Wallets.TryAdd(type, new WalletData(type));
            return Wallets[type];
        }

        // TODO: Plan out experience and their types.
        public ulong GetGuildExp(ulong guildId)
            => GuildExp.TryGetValue(guildId, out ulong value) ? value : 0;

        public void UpdateGuildExp(ulong guildId, ulong value)
        {
            if (!GuildExp.TryAdd(guildId, value))
                GuildExp[guildId] += value;
        }

        public GuildWalletData GetWalletForGuild(ulong guildId)
        {
            GuildWallets.TryAdd(guildId, new GuildWalletData(guildId));
            return GuildWallets[guildId];
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

        public int GetAttribute(string key)
            => Attributes.ContainsKey(key) ? Attributes[key] : 0;

        // TODO: Create CooldownHandler // Literal Cooldown
        // TODO: Create CooldownHandler // Process Cooldown
        /// <summary>
        /// Sets or updates a cooldown for a user.
        /// </summary>
        public void SetCooldown(CooldownType type, string id, TimeSpan duration)
        {
            DateTime expiresOn = DateTimeUtils.GetTimeIn(duration);
            string key = $"{type.ToString().ToLower()}:{id}";

            if (type.EqualsAny(CooldownType.Command, CooldownType.Global))
            {
                if (!ProcessCooldowns.TryAdd(key, expiresOn))
                    ProcessCooldowns[key] = expiresOn;
            }

            if (!Cooldowns.TryAdd(key, expiresOn))
                Cooldowns[key] = expiresOn;
        }

        /// <summary>
        /// Gets all currently set cooldowns for a specified type.
        /// </summary>
        public List<KeyValuePair<string, DateTime>> GetCooldownsFor(CooldownType type)
            => Cooldowns.Where(x => x.Key.StartsWith($"{type.ToString().ToLower()}:")).ToList();

        /// <summary>
        /// Checks to see if the user has a currently existing cooldown in place.
        /// </summary>
        public bool IsOnCooldown(string key)
            => Cooldowns.ContainsKey(key) ?
            DateTime.UtcNow.Subtract(Cooldowns[key]).TotalSeconds > 0 :
            ProcessCooldowns.ContainsKey(key) ?
            DateTime.UtcNow.Subtract(ProcessCooldowns[key]).TotalSeconds > 0 :
            false;

        public bool TryGetSocketEntity(BaseSocketClient client, out SocketUser user)
        { 
            user = client.GetUser(Id);
            return user != null;
        }

        public override bool Equals(object obj)
            => obj is null || obj == null || GetType() != obj.GetType() ?
            false : ReferenceEquals(this, obj) ?
            true : Equals(obj as IJsonEntity);

        public bool Equals(IJsonEntity obj)
            => Id == obj.Id;

        public override int GetHashCode()
            => unchecked((int)Id);

        public override string ToString()
            => $"{Username}#{Discriminator}";
    }
}
