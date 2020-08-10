using Newtonsoft.Json;
using Orikivo;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord;

namespace Arcadia
{
    public class ArcadeUser : BaseUser
    {
        public ArcadeUser(IUser user)
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
            Quests = new List<QuestData>();
            Items = new List<ItemData>();
            Card = new CardConfig(Graphics.PaletteType.Default);
        }

        [JsonConstructor]
        internal ArcadeUser(ulong id, string username, string discriminator, DateTime createdAt, UserConfig config,
            ulong balance, ulong tokenBalance, ulong chipBalance, ulong debt, ulong exp, int ascent, Dictionary<string, long> stats,
            Dictionary<string, MeritData> merits, List<QuestData> quests, List<ItemData> items, CardConfig card)
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
            Quests = quests ?? new List<QuestData>();
            Items = items ?? new List<ItemData>();
            Card = card ?? new CardConfig(Graphics.PaletteType.Default);
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
        public int Ascent { get; internal set; }

        [JsonProperty("stats")]
        public Dictionary<string, long> Stats { get; }

        [JsonProperty("merits")]
        public Dictionary<string, MeritData> Merits { get; }

        // [JsonProperty("boosters")]
        // public List<BoosterData> Boosters { get; }

        [JsonProperty("quests")]
        public List<QuestData> Quests { get; }

        [JsonProperty("items")]
        public List<ItemData> Items { get; }

        [JsonProperty("card")]
        public CardConfig Card { get; }

        /// <summary>
        /// Represents a collection of internal cooldowns for the current process.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, DateTime> InternalCooldowns { get; } = new Dictionary<string, DateTime>();

        [JsonIgnore] public bool CanAutoGimi { get; set; } = true;

        public long GetStat(string id)
            => Stats.ContainsKey(id) ? Stats[id] : 0;

        public void SetStat(string id, long value)
        {
            // This updates the quest progress without altering main stats
            if (Quests.Any())
            {
                foreach (QuestData data in Quests.Where(x => x.Progress.ContainsKey(id)))
                    data.Progress[id] = value;
            }

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

            // This updates the quest progress without altering main stats
            if (Quests.Any())
            {
                foreach (QuestData data in Quests.Where(x => x.Progress.ContainsKey(id)))
                    data.Progress[id] += amount;
            }
        }

        // TODO: make the type of integer consistent with balances
        public void Give(long value)
        {
            if ((long) Debt >= value)
            {
                Debt -= (ulong) value;
            }
            else if ((long) Debt > 0)
            {
                value -= (long)Debt;
                Debt = 0;
                Balance += (ulong) value;
            }
            else
            {
                Balance += (ulong) value;
            }
        }

        public void Take(long value)
        {
            if ((long) Balance >= value)
            {
                Balance -= (ulong) value;
            }
            else if ((long) Balance > 0)
            {
                value -= (long)Balance;
                Balance = 0;
                Debt += (ulong) value;
            }
            else
            {
                Debt += (ulong) value;
            }
        }
    }
}


// Arcadia-only property
// public Dictionary<ExpType, ulong> ExpData { get; } = new Dictionary<ExpType, ulong> { [ExpType.Global] = 0 };
