using Discord.WebSocket;
using Newtonsoft.Json;
using Orikivo;
using Orikivo.Desync;
using System;
using System.Collections.Generic;

namespace Arcadia
{

    public class ArcadeUser : BaseUser
    {
        public ArcadeUser(SocketUser user)
            : base(user)
        {
            Balance = 0;
            TokenBalance = 0;
            Debt = 0;
            Exp = 0;
            Ascent = 0;
            Stats = new Dictionary<string, long>();
            Merits = new Dictionary<string, MeritData>();
        }

        [JsonConstructor]
        internal ArcadeUser(ulong id, string username, string discriminator, DateTime createdAt, UserConfig config,
            ulong balance, ulong tokenBalance, ulong debt, ulong exp, int ascent, Dictionary<string, long> stats,
            Dictionary<string, MeritData> merits)
            : base(id, username, discriminator, createdAt, config)
        {
            Balance = balance;
            TokenBalance = tokenBalance;
            Debt = debt;
            Exp = exp;
            Ascent = ascent;
            Stats = stats ?? new Dictionary<string, long>();
            Merits = merits ?? new Dictionary<string, MeritData>();
        }

        [JsonProperty("balance")]
        public ulong Balance { get; internal set; }

        [JsonProperty("tokens")]
        public ulong TokenBalance { get; internal set; }

        [JsonProperty("debt")]
        public ulong Debt { get; internal set; }

        [JsonProperty("exp")]
        public ulong Exp { get; internal set; }

        [JsonIgnore]
        public int Level => ExpConvert.AsLevel(Exp);
        
        [JsonProperty("ascent")]
        public int Ascent { get; internal set; } = 0;

        [JsonProperty("stats")]
        public Dictionary<string, long> Stats { get; }

        [JsonProperty("merits")]
        public Dictionary<string, MeritData> Merits { get; }

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
    }
}

// Arcadia-only property
// public Dictionary<ulong, GuildData> Connections { get; } = new Dictionary<ulong, GuildData>();

// Arcadia-only property
// public ObjectiveMainData Objectives { get; } = new ObjectiveMainData();

// Arcadia-only property
// public Dictionary<ExpType, ulong> ExpData { get; } = new Dictionary<ExpType, ulong> { [ExpType.Global] = 0 };
// public ulong Exp { get; internal set; } = 0;

// Arcadia-only property
// public int Level => ExpConvert.AsLevel(Exp);

// Arcadia-only property
// Represents the total number of level resets.
// public int Ascent { get; set; } = 0;

// Boosters will be used by Orikivo Arcade, not Orikivo
// public List<BoosterData> Boosters { get; } = new List<BoosterData>();

// Arcadia-only property
// public CardConfig Card { get; set; }
