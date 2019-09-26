using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{

    // socketuser orikivo counterpart
    public class OriUser : ISocketEntity<SocketUser>, IJsonEntity
    {
        [JsonConstructor]
        internal OriUser(ulong id, string username, string discriminator, DateTime createdAt, ulong? balance, ulong? debt, ulong? exp,
            Dictionary<ulong, ulong> guildExp, Dictionary<string, ItemData> items, Dictionary<string, int> stats, Dictionary<string, MeritInfo> merits,
            Dictionary<string, int> upgrades, Dictionary<string, BoosterInfo> boosters, Dictionary<string, CooldownInfo> cooldowns, OriUserOptions options,
            GimiInfo gimi)
        {

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
            Balance = balance ?? 0;
            Debt = debt ?? 0;
            Exp = exp ?? 0;
            GuildExp = guildExp ?? new Dictionary<ulong, ulong>();
            Items = items ?? new Dictionary<string, ItemData>();
            Stats = stats ?? new Dictionary<string, int>();
            Merits = merits ?? new Dictionary<string, MeritInfo>();
            Upgrades = upgrades ?? new Dictionary<string, int>();
            Boosters = boosters ?? new Dictionary<string, BoosterInfo>();
            Cooldowns = cooldowns ?? new Dictionary<string, CooldownInfo>();
            Options = options ?? OriUserOptions.Default;
            Gimi = gimi ?? new GimiInfo();
            HasChanged = false;
        }

        public OriUser(SocketUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
            CreatedAt = DateTime.UtcNow;
            Balance = 0;
            Debt = 0;
            Exp = 0;
            GuildExp = new Dictionary<ulong, ulong>();
            Cooldowns = new Dictionary<string, CooldownInfo>();
            Stats = new Dictionary<string, int>();
            Merits = new Dictionary<string, MeritInfo>();
            Upgrades = new Dictionary<string, int>();
            Items = new Dictionary<string, ItemData>();
            Boosters = new Dictionary<string, BoosterInfo>();
            Options = OriUserOptions.Default;
            Gimi = new GimiInfo();
            HasChanged = false;
        }

        // single-layer properties
        [JsonProperty("id")]
        public ulong Id { get; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("discriminator")]
        public string Discriminator { get; private set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("balance")]
        public ulong Balance { get; private set; }
        [JsonProperty("guild_bal")] // used to help the guild out
        public Dictionary<ulong, GuildBalanceInfo> GuildBal { get; private set; }

        [JsonProperty("debt")]
        public ulong Debt { get; private set; }

        [JsonProperty("exp")]
        public ulong Exp { get; private set; }
        [JsonProperty("guild_exp")]
        public Dictionary<ulong, ulong> GuildExp { get; }

        [JsonProperty("stats")]
        public Dictionary<string, int> Stats { get; }
        [JsonProperty("merits")]
        public Dictionary<string, MeritInfo> Merits { get; }
        [JsonProperty("upgrades")]
        public Dictionary<string, int> Upgrades { get; }
        [JsonProperty("boosters")]
        public Dictionary<string, BoosterInfo> Boosters { get; }
        [JsonProperty("items")]
        public Dictionary<string, ItemData> Items { get; }
        [JsonProperty("cooldowns")]
        public Dictionary<string, CooldownInfo> Cooldowns { get; }

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

        // guild exp 
        public ulong GetGuildExp(ulong guildId)
            => GuildExp.TryGetValue(guildId, out ulong value) ? value : 0;

        public void UpdateGuildExp(ulong guildId, ulong value)
        {
            if (!GuildExp.TryAdd(guildId, value))
                GuildExp[guildId] += value;
            HasChanged = true;
        }

        public ulong GetGuildBal(ulong guildId)
            => GuildBal.TryGetValue(guildId, out GuildBalanceInfo balanceInfo) ? balanceInfo.Balance : 0;

        public void UpdateGuildBal(ulong guildId, ulong bal = 0, ulong debt = 0)
        {
            if (!GuildBal.TryAdd(guildId, new GuildBalanceInfo(bal, debt)))
                GuildBal[guildId].Update(bal, debt);
            HasChanged = true;
        }

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

            HasChanged = true;
        }

        public List<KeyValuePair<string, int>> GetStats(string type)
            => Stats.Where(x => x.Key.StartsWith(type)).ToList();

        public void UpdateStat(string key, int value)
        {
            if (!Stats.TryAdd(key, value))
                Stats[key] += value;

            HasChanged = true;
        }

        // inventory
        public bool HasItem(string key)
            => Items.ContainsKey(key);

        public void AddItem(string key, int amount = 1)
        { 
            // implement IsStackable & build-in features
            if (!Items.TryAdd(key, new ItemData { StackCount = amount }))
                Items[key].StackCount += amount;

            HasChanged = true;
        }

        public void RemoveItem(string key, int amount = 1)
        {
            if (HasItem(key))
            {
                if (Items[key].Count - amount < 1)
                    Items.Remove(key);
                else
                    Items[key].StackCount -= amount;

                HasChanged = true;
            }
        }

        // upgrades
        public bool HasUpgrade(string key)
            => Upgrades.ContainsKey(key);

        public bool HasUpgradeAt(string key, int value)
            => Upgrades.ContainsKey(key) ? Upgrades[key] == value : false;
        public bool MeetsStatCriteria(params (string, int)[] criteria)
        {
            foreach ((string key, int count) in criteria)
            {
                if (!(Stats.ContainsKey(key) ? Stats[key] == count : false))
                    return false;
            }
            return true;
        }
        public bool HasItemAt(string key, int value)
            => Items.ContainsKey(key) ? Items[key].Count == value : false;

        public bool HasMerit(string key)
            => Merits.ContainsKey(key);

        public int GetUpgrade(string key)
            => Upgrades.ContainsKey(key) ? Upgrades[key] : 0;

        // cooldown
        public void SetCooldown(string key, double seconds)
        {
            if (!Cooldowns.TryAdd(key, new CooldownInfo(DateTime.UtcNow, seconds)))
                Cooldowns[key] = new CooldownInfo(DateTime.UtcNow, seconds);
            HasChanged = true;
        }

        public List<KeyValuePair<string, CooldownInfo>> GetCommandCooldowns()
            => Cooldowns.Where(x => x.Key.StartsWith("command")).ToList();

        public bool IsOnCooldown(string key)
        {
            if (Cooldowns.ContainsKey(key))
                return Cooldowns[key].IsActive;
            return false;
        }

        // update upon any properties being changed
        [JsonIgnore]
        public DateTime? LastSaved { get; internal set; }

        // if the user object has been changed in any way.
        [JsonIgnore]
        public bool HasChanged { get; internal set; }

        // get-only properties
        [JsonIgnore]
        public string DefaultName { get { return $"{Username}#{Discriminator}"; } }

        [JsonIgnore]
        public string Name { get { return !string.IsNullOrWhiteSpace(Options.Nickname) ? Options.Nickname : Username; } }

        // inner-layer properties
        [JsonProperty("options")]
        public OriUserOptions Options { get; private set; }

        [JsonProperty("gimi")]
        public GimiInfo Gimi { get; }

        // rework this
        public OriMessage GetDisplay(EntityDisplayFormat displayFormat)
        {
            OriMessage oriMessage = new OriMessage();
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
                    oriMessage.Text = sbj.ToString();
                    return oriMessage;
                default:
                    StringBuilder sbd = new StringBuilder();
                    sbd.AppendLine($"**{Username}**#{Discriminator}"); // name display
                    sbd.AppendLine($"• {Id}");// id display
                    sbd.AppendLine($"\n**Joined**: {CreatedAt.ToString($"`MM`.`dd`.`yyyy` **@** `hh`:`mm`:`ss`")}");
                    oriMessage.Text = sbd.ToString();
                    return oriMessage;
            }
        }

        public bool TryGetSocketEntity(BaseSocketClient client, out SocketUser user)
        { 
            user = client.GetUser(Id);
            return user != null;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as IJsonEntity);
        }

        public bool Equals(IJsonEntity obj)
            => Id == obj.Id;

        public override int GetHashCode()
            => unchecked((int)Id);
    }
}
