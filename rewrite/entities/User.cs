using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Orikivo.Unstable
{
    // Balance => Orite
    // TokenBalance => Voken
    // GuildBalance => Gini
    // Debt => Etiro

    public class User : IDiscordEntity<SocketUser>, IJsonEntity
    {
        [JsonConstructor, BsonConstructor]
        internal User(
            ulong id, string username, string discriminator, DateTime createdAt,
            ulong balance, ulong tokenBalance, ulong debt,
            Dictionary<string, ItemData> items, Dictionary<ulong, GuildData> connections,
            ObjectiveData objectives, Dictionary<string, DateTime> cooldowns, Dictionary<ExpType, ulong> expData,
            int ascent, Dictionary<string, long> stats, Dictionary<string, MeritData> merits, List<BoosterData> boosters,
            Dictionary<string, int> upgrades, HuskBrain brain, Husk husk, UserConfig config, CardConfig card)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
            CreatedAt = createdAt;

            Balance = balance;
            TokenBalance = tokenBalance;
            Debt = debt;

            Items = items;
            Connections = connections;
            Objectives = objectives;

            Cooldowns = cooldowns;

            ExpData = expData;
            Ascent = ascent;

            Stats = stats;
            Merits = merits;
            Boosters = boosters;
            Upgrades = upgrades;

            Brain = brain;
            Husk = husk;
            Config = config;
            Card = card;
        }

        public User(SocketUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
            CreatedAt = DateTime.UtcNow;

            Balance = 0;
            TokenBalance = 0;
            Debt = 0;

            Items = new Dictionary<string, ItemData>();
            Connections = new Dictionary<ulong, GuildData>();
            Objectives = new ObjectiveData();

            Cooldowns = new Dictionary<string, DateTime>();
            InternalCooldowns = new Dictionary<string, DateTime>();

            ExpData = new Dictionary<ExpType, ulong> { [ExpType.Global] = 0 };
            Ascent = 0;

            Stats = new Dictionary<string, long>();
            Merits = new Dictionary<string, MeritData>();
            Boosters = new List<BoosterData>();
            Upgrades = new Dictionary<string, int>();

            Brain = new HuskBrain();
            Husk = null;
            Config = new UserConfig();
            Card = new CardConfig();
        }

        // Discord
        [JsonProperty("id"), BsonId]
        public ulong Id { get; }

        [JsonProperty("username"), BsonElement("username")]
        public string Username { get; private set; }

        [JsonProperty("discriminator"), BsonElement("discriminator")]
        public string Discriminator { get; private set; }

        [JsonProperty("created_at"), BsonElement("created_at")]
        public DateTime CreatedAt { get; }

        /// <summary>
        /// The <see cref="User"/>'s global balance, in use for both the world and client.
        /// </summary>
        [JsonProperty("balance"), BsonElement("balance")]
        public ulong Balance { get; internal set; } = 0; // from earnings/winnings

        /// <summary>
        /// The <see cref="User"/>'s balance, primarily from voting.
        /// </summary>
        [JsonProperty("tokens"), BsonElement("tokens")]
        public ulong TokenBalance { get; private set; } = 0; // from voting

        /// <summary>
        /// Represents the <see cref="User"/>'s negative funds.
        /// </summary>
        [JsonProperty("debt"), BsonElement("debt")]
        public ulong Debt { get; internal set; } = 0; // from negative funds

        /// <summary>
        /// Represents the <see cref="User"/>'s complete collection of digital items.
        /// </summary>
        [JsonProperty("items"), BsonElement("items")]
        public Dictionary<string, ItemData> Items { get; } = new Dictionary<string, ItemData>();

        [JsonProperty("connections"), BsonElement("connections")]
        public Dictionary<ulong, GuildData> Connections { get; } = new Dictionary<ulong, GuildData>();

        [JsonProperty("objectives"), BsonElement("objectives")]
        public ObjectiveData Objectives { get; } = new ObjectiveData();

        /// <summary>
        /// A collection of cooldowns, typically utilized by an <see cref="Item"/>.
        /// </summary>
        [JsonProperty("cooldowns"), BsonElement("cooldowns")]
        public Dictionary<string, DateTime> Cooldowns { get; } = new Dictionary<string, DateTime>();

        /// <summary>
        /// A collection of internal cooldowns for the current process.
        /// </summary>
        [JsonIgnore, BsonIgnore]
        public Dictionary<string, DateTime> InternalCooldowns { get; } = new Dictionary<string, DateTime>();




        // experience
        [JsonProperty("exp"), BsonElement("exp")]
        public Dictionary<ExpType, ulong> ExpData { get; } = new Dictionary<ExpType, ulong> { [ExpType.Global] = 0 };

        
        /// <summary>
        /// Represents the <see cref="User"/>'s global experience.
        /// </summary>
        [JsonIgnore, BsonIgnore]
        public ulong Exp => ExpData[ExpType.Global];
        // stats (the stored object type of a stat must be defined elsewhere)

         
        [JsonIgnore, BsonIgnore]
        public int Level => ExpConvert.AsLevel(Exp);

        /// <summary>
        /// Represents the total number of level resets.
        /// </summary>
        [JsonProperty("ascent"), BsonElement("ascent")]
        public int Ascent { get; set; } = 0;
        
        [JsonProperty("stats"), BsonElement("stats")]
        public Dictionary<string, long> Stats { get; } = new Dictionary<string, long>(); // Type:{JSON} (example: Int32:0)
        
        [JsonProperty("merits"), BsonElement("merits")]
        public Dictionary<string, MeritData> Merits { get; } = new Dictionary<string, MeritData>();

        [JsonProperty("boosters"), BsonElement("boosters")]
        public List<BoosterData> Boosters { get; } = new List<BoosterData>();

        /// <summary>
        /// A collection of upgrades specified by their unique identifier.
        /// </summary>
        [JsonProperty("upgrades"), BsonElement("upgrades")]
        public Dictionary<string, int> Upgrades { get; } = new Dictionary<string, int>();

        [JsonProperty("brain"), BsonElement("brain")]
        public HuskBrain Brain { get; } = new HuskBrain();
        
        [JsonProperty("husk"), BsonElement("husk")]
        public Husk Husk { get; internal set; } = null;

        [JsonProperty("config"), BsonElement("config")]
        public UserConfig Config { get; } //= UserConfig.Default;

        [JsonProperty("card"), BsonElement("card")]
        public CardConfig Card { get; set; }

        // TODO: make the type of integer consistent with balances
        public void Give(long value)
        {
            if (((long)Debt - value) < 0)
            {
                Debt = 0;
                Balance += (ulong)(value - (long)Debt);
            }
            else
                Debt -= (ulong)value;
        }

        public void Take(long value)
        {
            if (((long)Balance - value) < 0)
            {
                Balance = 0;
                Debt += (ulong)(value - (long)Balance);
            }
            else
                Balance -= (ulong)value;
        }
        public void SetCooldown(Claimable claimable, out bool updated)
        {
            string id = $"{CooldownType.Claimable.ToString().ToLower()}:{claimable.Id}";
            if (Cooldowns.ContainsKey(id))
            {
                // if you can set a claimable cooldown.
                updated = (Cooldowns[id] - DateTime.UtcNow) <= TimeSpan.Zero;

                if (updated)
                {
                    // Set up/update the streak stats
                    if ((DateTime.UtcNow - Cooldowns[id]) >= claimable.Preservation) // if the streak will reset.
                        SetStat(claimable.StreakId, 1);
                    else
                        UpdateStat(claimable.StreakId, 1);

                    Cooldowns[id] = DateTime.UtcNow.Add(claimable.Cooldown); // set new expiration.
                }
            }
            else
            {
                SetStat(claimable.StreakId, 1);
                Cooldowns[id] = DateTime.UtcNow.Add(claimable.Cooldown);
                updated = true;
            }
        }

        /// <summary>
        /// Sets or updates a cooldown for a user.
        /// </summary>
        public void SetCooldown(CooldownType type, string name, TimeSpan duration)
        {
            DateTime expiresOn = DateTimeUtils.GetTimeIn(duration);
            string id = $"{type.ToString().ToLower()}:{name}";

            if (type.EqualsAny(CooldownType.Command, CooldownType.Global, CooldownType.Notify))
            {
                if (!InternalCooldowns.TryAdd(id, expiresOn))
                    InternalCooldowns[id] = expiresOn;
            }

            if (type == CooldownType.Claimable)
            {
                Claimable info = WorldEngine.GetClaimable(name);

                if (Cooldowns.ContainsKey(id))
                {
                    // if you can set a claimable cooldown.
                    bool canUpdate = (Cooldowns[id] - DateTime.UtcNow) <= TimeSpan.Zero;

                    if (canUpdate)
                    {
                        // Set up/update the streak stats
                        if ((DateTime.UtcNow - Cooldowns[id]) >= info.Preservation) // if the streak will reset.
                            SetStat(info.StreakId, 1);
                        else
                            UpdateStat(info.StreakId, 1);

                        Cooldowns[id] = DateTime.UtcNow.Add(info.Cooldown); // set new expiration.
                    }
                }
                else
                {
                    SetStat(info.StreakId, 1);
                    Cooldowns[id] = DateTime.UtcNow.Add(info.Cooldown);
                }
            }

            if (type == CooldownType.Item)
                if (!Cooldowns.TryAdd(id, expiresOn))
                    Cooldowns[id] = expiresOn;
        }

        public IEnumerable<CooldownData> GetCooldownsOfType(CooldownType type)
        {
            if (type == CooldownType.Internal)
                return InternalCooldowns.Select(x => new CooldownData(x.Key, x.Value));

            if (type.EqualsAny(CooldownType.Command, CooldownType.Global, CooldownType.Notify))
                return InternalCooldowns.Where(c => c.Key.StartsWith(type.ToString().ToLower())).Select(x => new CooldownData(x.Key, x.Value));


            if (type == CooldownType.Storeable)
                return Cooldowns.Select(x => new CooldownData(x.Key, x.Value));

            if (type.EqualsAny(CooldownType.Claimable, CooldownType.Item))
                return Cooldowns.Where(c => c.Key.StartsWith(type.ToString().ToLower())).Select(x => new CooldownData(x.Key, x.Value));

            throw new InvalidOperationException("what the heck, what kind of cooldown is this even");
        }

        public bool IsOnCooldown(string id)
        {
            if (Cooldowns.ContainsKey(id))
                return DateTime.UtcNow.Subtract(Cooldowns[id]).TotalSeconds > 0;

            if (InternalCooldowns.ContainsKey(id))
                return DateTime.UtcNow.Subtract(InternalCooldowns[id]).TotalSeconds > 0;

            return false;
        }

        public void UpdateExp(ulong amount, ExpType type = ExpType.Global)
        {
            if (!ExpData.TryAdd(type, amount))
                ExpData[type] += amount;
        }

        public bool HasUpgradeAt(string id, int tier)
            => Upgrades.ContainsKey(id) ? Upgrades[id] == tier : false;

        // tick an upgrade by 1, if possible.
        public void Upgrade(string id)
        {
            if (!Upgrades.TryAdd(id, 1))
                Upgrades[id] += 1;
        }

        public int GetUpgradeTier(string id)
            => Upgrades.ContainsKey(id) ? Upgrades[id] : 0;

        public long GetStat(string id)
            => Stats.ContainsKey(id) ? Stats[id] : 0;

        public Dictionary<string, long> GetStatsOfType(string type)
        {
            throw new NotImplementedException();
        }

        public void SetStat(string id, long value)
        {
            if (!Stats.TryAdd(id, value))
                Stats[id] = value;
        }

        public void UpdateStat(string id, long amount = 1)
        {
            if (!Stats.ContainsKey(id))
                SetStat(id, amount);
            else
                Stats[id] += amount;
        }

        public bool HasItem(string id, int amount = 1)
            => Items.ContainsKey(id) ? Items[id].Count == amount : false;

        // Correlate with GameDatabase.Items.
        public void AddItem(string id, int amount = 1) { }
        public void RemoveItem(string id, int amount = 1) { }

        public bool HasMerit(string id)
            => Merits.ContainsKey(id);

        public bool TryGetDiscordEntity(BaseSocketClient client, out SocketUser user)
        {
            user = client.GetUser(Id);
            return user != null;
        }

        // updates the username and discriminator of a user if anything was changed.
        public void SetIdentity(SocketUser user)
        {
            if (Id != user.Id)
                throw new Exception("The user specified must have the same matching ID as the account.");

            Username = user.Username;
            Discriminator = user.Discriminator;
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
