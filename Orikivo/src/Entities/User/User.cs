using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Discord;
using MongoDB.Bson.Serialization.Attributes;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a user account for <see cref="Orikivo"/>.
    /// </summary>
    public class User : BaseUser
    {
        [JsonConstructor, BsonConstructor]
        internal User(
            ulong id,
            string username,
            string discriminator,
            DateTime createdAt,
            Notifier notifier,
            ulong balance,
            ulong tokenBalance,
            ulong debt,
            Dictionary<string, ItemData> items,
            Dictionary<string, DateTime> cooldowns,
            Dictionary<string, long> stats,
            Dictionary<string, MeritData> merits,
            HuskBrain brain, Husk husk,
            UserConfig config) : base(id, username, discriminator, createdAt, config)
        {
            Username = username;
            Discriminator = discriminator;
            Balance = balance;
            TokenBalance = tokenBalance;
            Debt = debt;
            Items = items ?? new Dictionary<string, ItemData>();
            Cooldowns = cooldowns ?? new Dictionary<string, DateTime>();
            Stats = stats ?? new Dictionary<string, long>();
            Merits = merits ?? new Dictionary<string, MeritData>();
            Brain = brain ?? new HuskBrain();
            Husk = husk;
        }

        public User(IUser user) : base(user)
        {
            Balance = 0;
            TokenBalance = 0;
            Debt = 0;
            Items = new Dictionary<string, ItemData>();
            Cooldowns = new Dictionary<string, DateTime>();
            InternalCooldowns = new Dictionary<string, DateTime>();
            Stats = new Dictionary<string, long>();
            Merits = new Dictionary<string, MeritData>();
            Brain = new HuskBrain();
            Husk = null;
        }

        /// <summary>
        /// The <see cref="User"/>'s global balance, in use for both the world and client.
        /// </summary>
        [JsonProperty("balance"), BsonElement("balance")]
        public ulong Balance { get; internal set; }

        /// <summary>
        /// The <see cref="User"/>'s balance, primarily from voting.
        /// </summary>
        [JsonProperty("tokens"), BsonElement("tokens")]
        public ulong TokenBalance { get; private set; }

        /// <summary>
        /// Represents the <see cref="User"/>'s negative funds.
        /// </summary>
        [JsonProperty("debt"), BsonElement("debt")]
        public ulong Debt { get; internal set; }

        // Arcadia-only property
        /// <summary>
        /// Represents the <see cref="User"/>'s complete collection of digital items.
        /// </summary>
        [JsonProperty("items"), BsonElement("items")]
        public Dictionary<string, ItemData> Items { get; }

        /// <summary>
        /// Represents collection of cooldowns, typically utilized by an <see cref="Item"/>.
        /// </summary>
        [JsonProperty("cooldowns"), BsonElement("cooldowns")]
        public Dictionary<string, DateTime> Cooldowns { get; }

        /// <summary>
        /// A collection of internal cooldowns for the current process.
        /// </summary>
        [JsonIgnore, BsonIgnore]
        public Dictionary<string, DateTime> InternalCooldowns { get; } = new Dictionary<string, DateTime>();

        [JsonProperty("stats"), BsonElement("stats")]
        public Dictionary<string, long> Stats { get; }
        
        [JsonProperty("merits"), BsonElement("merits")]
        public Dictionary<string, MeritData> Merits { get; }

        /// <summary>
        /// Represents the brain of their <see cref="Desync.Husk"/>, which keeps track of everything they have accomplished.
        /// </summary>
        [JsonProperty("brain"), BsonElement("brain")]
        public HuskBrain Brain { get; }
        
        [JsonProperty("husk"), BsonElement("husk")]
        public Husk Husk { get; internal set; }

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

        

        /// <summary>
        /// Sets or updates a cooldown for a user.
        /// </summary>
        public void SetCooldown(CooldownType type, string name, TimeSpan duration)
        {
            DateTime expiresOn = DateTime.UtcNow.Add(duration);
            string id = $"{type.ToString().ToLower()}:{name}";

            if (type.EqualsAny(CooldownType.Command, CooldownType.Global, CooldownType.Notify))
            {
                if (!InternalCooldowns.TryAdd(id, expiresOn))
                    InternalCooldowns[id] = expiresOn;
            }

            if (type == CooldownType.Claimable)
            {
                Claimable info = Engine.GetClaimable(name);

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

        

        public bool IsOnCooldown(string id)
        {
            if (Cooldowns.ContainsKey(id))
                return DateTime.UtcNow.Subtract(Cooldowns[id]).TotalSeconds > 0;

            if (InternalCooldowns.ContainsKey(id))
                return DateTime.UtcNow.Subtract(InternalCooldowns[id]).TotalSeconds > 0;

            return false;
        }
       
        public long GetStat(string id)
            => Stats.ContainsKey(id) ? Stats[id] : 0;

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

        public bool HasMerit(string id)
            => Merits.ContainsKey(id);

        public override bool Equals(object obj)
            => obj != null && GetType() == obj.GetType() && (ReferenceEquals(this, obj) || Equals(obj as IJsonEntity));

        public override int GetHashCode()
            => unchecked((int)Id);

        public override string ToString()
            => $"{Username}#{Discriminator}";
    }
}
