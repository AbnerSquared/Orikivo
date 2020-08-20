using Newtonsoft.Json;
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
            Boosters = new List<BoosterData>();
            Quests = new List<QuestData>();
            Items = new List<ItemData>();
            Card = new CardConfig(Graphics.PaletteType.Default);
            CatalogHistory = new Dictionary<string, CatalogHistory>();
        }

        [JsonConstructor]
        internal ArcadeUser(ulong id, string username, string discriminator, DateTime createdAt, UserConfig config,
            long balance, long tokenBalance, long chipBalance, long debt, ulong exp, int ascent, Dictionary<string, long> stats,
            Dictionary<string, MeritData> merits, List<BoosterData> boosters, List<QuestData> quests, List<ItemData> items, CardConfig card,
            Dictionary<string, CatalogHistory> catalogHistory)
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
            Boosters = boosters ?? new List<BoosterData>();
            Quests = quests ?? new List<QuestData>();
            Items = items ?? new List<ItemData>();
            Card = card ?? new CardConfig(Graphics.PaletteType.Default);
            CatalogHistory = catalogHistory ?? new Dictionary<string, CatalogHistory>();
        }

        [JsonProperty("balance")]
        public long Balance { get; internal set; }

        [JsonProperty("tokens")]
        public long TokenBalance { get; internal set; }

        [JsonProperty("chips")]
        public long ChipBalance { get; internal set; }

        [JsonProperty("debt")]
        public long Debt { get; internal set; }

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

        [JsonProperty("boosters")]
        public List<BoosterData> Boosters { get; }

        [JsonProperty("quests")]
        public List<QuestData> Quests { get; }

        [JsonProperty("items")]
        public List<ItemData> Items { get; }

        [JsonProperty("card")]
        public CardConfig Card { get; }

        [JsonProperty("catalog_history")]
        public Dictionary<string, CatalogHistory> CatalogHistory { get; }

        [JsonIgnore]
        public Dictionary<string, DateTime> InternalCooldowns { get; } = new Dictionary<string, DateTime>();

        [JsonIgnore]
        public DateTime? GlobalCooldown { get; set; }

        public bool HasBeenNoticed { get; set; } = false;

        [JsonIgnore]
        public bool CanAutoGimi { get; set; } = true;

        [JsonIgnore]
        public bool CanShop { get; set; } = true;

        [JsonIgnore]
        public bool CanTrade { get; set; } = true;

        public long GetVar(string id)
            => Stats.ContainsKey(id) ? Stats[id] : 0;

        internal void SetQuestProgress(string id)
        {
            if (!Quests.Any(x => x.Progress.ContainsKey(id) && !QuestHelper.MeetsCriterion(x.Id, id, x.Progress[id])))
                return;

            long value = GetVar(id);

            QuestData data = Quests.First(x => x.Progress.ContainsKey(id));
            data.Progress[id] = value;

            if (QuestHelper.MeetsCriterion(data.Id, id, data.Progress[id]))
                data.Progress[id] = QuestHelper.GetCriterionGoal(data.Id, id);
        }

        public void SetVar(string id, long value)
        {
            if (value == 0)
            {
                if (Stats.ContainsKey(id))
                    Stats.Remove(id);

                SetQuestProgress(id);
                return;
            }

            if (!Stats.TryAdd(id, value))
                Stats[id] = value;

            SetQuestProgress(id);
        }

        public void SetVar(string id, long value, out long previous)
        {
            previous = GetVar(id);
            SetVar(id, value);
        }

        public void AddToVar(string id, long amount = 1)
        {
            if (!Stats.ContainsKey(id))
                SetVar(id, amount);
            else
                Stats[id] += amount;

            SetQuestProgress(id);
        }

        public void AddToVar(string id, long amount, out long previous)
        {
            previous = GetVar(id);
            AddToVar(id, amount);
        }

        public void Give(long value, bool canBoost = true)
        {
            if (canBoost)
                value = ItemHelper.BoostValue(this, value, BoosterType.Money);

            if (Debt >= value)
            {
                Debt -= value;
            }
            else if (Debt > 0)
            {
                value -= Debt;
                Debt = 0;
                Balance += value;
            }
            else
            {
                Balance += value;
            }
        }

        public void Give(long value, out long actual, bool canBoost = true)
        {
            actual = canBoost ? ItemHelper.BoostValue(this, value, BoosterType.Money) : value;

            if (Debt >= actual)
            {
                Debt -= actual;
            }
            else if (Debt > 0)
            {
                actual -= Debt;
                Debt = 0;
                Balance += actual;
            }
            else
            {
                Balance += actual;
            }
        }

        public void Take(long value, bool canBoost = true)
        {
            if (canBoost)
                value = ItemHelper.BoostValue(this, value, BoosterType.Debt);

            if (Balance >= value)
            {
                Balance -= value;
            }
            else if (Balance > 0)
            {
                value -= Balance;
                Balance = 0;
                Debt += value;
            }
            else
            {
                Debt += value;
            }
        }

        public void Take(long value, out long actual, bool canBoost = true)
        {
            actual = canBoost ? ItemHelper.BoostValue(this, value, BoosterType.Debt) : value;

            if (Balance >= actual)
            {
                Balance -= actual;
            }
            else if (Balance > 0)
            {
                actual -= Balance;
                Balance = 0;
                Debt += actual;
            }
            else
            {
                Debt += actual;
            }
        }
    }
}


// Arcadia-only property
// public Dictionary<ExpType, ulong> ExpData { get; } = new Dictionary<ExpType, ulong> { [ExpType.Global] = 0 };
