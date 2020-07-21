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
            ChipBalance = 0;
            Debt = 0;
            Exp = 0;
            Ascent = 0;
            Stats = new Dictionary<string, long>();
            Merits = new Dictionary<string, MeritData>();
            Items = new List<ItemData>();
        }

        [JsonConstructor]
        internal ArcadeUser(ulong id, string username, string discriminator, DateTime createdAt, UserConfig config,
            ulong balance, ulong tokenBalance, ulong chipBalance, ulong debt, ulong exp, int ascent, Dictionary<string, long> stats,
            Dictionary<string, MeritData> merits, List<ItemData> items)
            : base(id, username, discriminator, createdAt, config)
        {
            Balance = balance;
            TokenBalance = tokenBalance;
            ChipBalance = chipBalance;
            Debt = debt;
            Exp = exp;
            Ascent = ascent;
            Stats = stats ?? new Dictionary<string, long>();
            Merits = merits ?? new Dictionary<string, MeritData>();
            Items = items ?? new List<ItemData>();
        }

        [JsonProperty("balance")]
        public ulong Balance { get; internal set; }

        [JsonProperty("tokens")]
        public ulong TokenBalance { get; internal set; }

        [JsonProperty("chips")]
        public ulong ChipBalance { get; internal set; }

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

        [JsonProperty("items")]
        public List<ItemData> Items { get; }

        public long GetStat(string id)
            => Stats.ContainsKey(id) ? Stats[id] : 0;

        public void SetStat(string id, long value)
        {
            if (value == 0)
            {
                if (Stats.ContainsKey(id))
                    Stats.Remove(id);

                return;
            }

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
// public ObjectiveMainData Objectives { get; } = new ObjectiveMainData();

// Arcadia-only property
// public Dictionary<ExpType, ulong> ExpData { get; } = new Dictionary<ExpType, ulong> { [ExpType.Global] = 0 };

// Boosters will be used by Orikivo Arcade, not Orikivo
// public List<BoosterData> Boosters { get; } = new List<BoosterData>();

// Arcadia-only property
// public CardConfig Card { get; set; }
