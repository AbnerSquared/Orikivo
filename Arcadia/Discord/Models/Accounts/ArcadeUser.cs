using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Graphics;
using Discord;
using Orikivo;
using Arcadia.Services;

namespace Arcadia
{
    /// <summary>
    /// Represents a user account for Arcadia.
    /// </summary>
    public class ArcadeUser : BaseUser
    {
        public ArcadeUser(IUser user)
            : base(user)
        {
            Exp = 0;
            Ascent = 0;
            Stats = new Dictionary<string, long>();
            Merits = new Dictionary<string, MeritData>();
            Boosters = new List<BoostData>();
            Challenges = new Dictionary<string, ChallengeData>();
            Quests = new List<QuestData>();
            Items = new List<ItemData>();
            Card = new CardConfig
            {
                Layout = LayoutType.Default,
                Palette = PaletteType.Default,
                Font = FontType.Foxtrot
            };

            CatalogHistory = new Dictionary<string, CatalogHistory>();
        }

        [JsonConstructor]
        internal ArcadeUser(ulong id, string username, string discriminator, DateTime createdAt, UserConfig config,
            ulong exp, int ascent, Dictionary<string, long> stats,
            Dictionary<string, MeritData> merits, List<BoostData> boosters, Dictionary<string, ChallengeData> challenges,
            List<QuestData> quests, List<ItemData> items, CardConfig card,
            Dictionary<string, CatalogHistory> catalogHistory)
            : base(id, username, discriminator, createdAt, config)
        {
            Exp = exp;
            Ascent = ascent;
            Stats = stats ?? new Dictionary<string, long>();
            Merits = merits ?? new Dictionary<string, MeritData>();
            Boosters = boosters ?? new List<BoostData>();
            Challenges = challenges ?? new Dictionary<string, ChallengeData>();
            Quests = quests ?? new List<QuestData>();
            Items = items ?? new List<ItemData>();
            Card = card ?? new CardConfig
            {
                Layout = LayoutType.Default,
                Palette = PaletteType.Default,
                Font = FontType.Foxtrot
            };
            CatalogHistory = catalogHistory ?? new Dictionary<string, CatalogHistory>();
        }

        [JsonIgnore]
        public long Balance
        {
            get => GetVar(Vars.Balance);
            set => SetVar(Vars.Balance, value);
        }

        [JsonIgnore]
        public long TokenBalance
        {
            get => GetVar(Vars.Tokens);
            set => SetVar(Vars.Tokens, value);
        }

        [JsonIgnore]
        public long ChipBalance
        {
            get => GetVar(Vars.Chips);
            set => SetVar(Vars.Chips, value);
        }

        [JsonIgnore]
        public long Debt
        {
            get => GetVar(Vars.Debt);
            set => SetVar(Vars.Debt, value);
        }

        [JsonProperty("exp")]
        public ulong Exp { get; internal set; }

        [JsonIgnore]
        public int Level => ExpConvert.AsLevel(Exp, Ascent);

        [JsonProperty("ascent")]
        public int Ascent { get; internal set; }

        [JsonProperty("stats")]
        public Dictionary<string, long> Stats { get; }

        [JsonProperty("merits")]
        public Dictionary<string, MeritData> Merits { get; }

        [JsonProperty("boosters")]
        public List<BoostData> Boosters { get; }

        [JsonProperty("challenges")]
        public Dictionary<string, ChallengeData> Challenges { get; }

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

        [JsonIgnore] public List<TradeOffer> Offers { get; } = new List<TradeOffer>();

        [JsonIgnore]
        public DateTime? GlobalCooldown { get; set; }

        [JsonIgnore]
        public bool IsInSession { get; set; } = false;

        [JsonIgnore]
        public bool HasBeenNoticed { get; set; } = false;

        [JsonIgnore] public Wager LastFundsLost { get; set; }

        [JsonIgnore]
        public string EraseGuildKey { get; set; }

        [JsonIgnore]
        public string EraseConfirmKey { get; set; }

        private string _cardGenKey;
        [JsonIgnore]
        public string CardGenKey => _cardGenKey ??= KeyBuilder.Generate(6);

        internal void RegenCardKey()
            => _cardGenKey = KeyBuilder.Generate(6);

        public long GetVar(string id)
            => Var.GetValue(this, id);

        internal void SetQuestProgress(string id)
        {
            long value = GetVar(id);

            if (!Quests.Any(x => x.Progress.ContainsKey(id) && !x.Progress[id].Complete && !QuestHelper.MeetsCriterion(x.Id, id, x.Progress[id]?.Value ?? value)))
                return;

            QuestData data = Quests.First(x => x.Progress.ContainsKey(id) && !x.Progress[id].Complete && !QuestHelper.MeetsCriterion(x.Id, id, x.Progress[id]?.Value ?? value));
            data.Progress[id].Value = value;

            // When a criterion is met, it might be better to remove it from the Quest data
            // When viewing a quest with a missing criterion, it is automatically assumed to be completed already

            if (QuestHelper.MeetsCriterion(data.Id, id, data.Progress[id].Value ?? value))
            {
                data.Progress[id].Complete = true;
                data.Progress[id].Value = null;
            }
        }

        internal void AddToQuestProgress(string id, long amount)
        {
            if (!Quests.Any(x => x.Progress.ContainsKey(id) && !x.Progress[id].Complete && x.Progress[id].Value.HasValue && !QuestHelper.MeetsCriterion(x.Id, id, x.Progress[id].Value.Value)))
                return;

            QuestData data = Quests.First(x => x.Progress.ContainsKey(id) && !x.Progress[id].Complete && x.Progress[id].Value.HasValue && !QuestHelper.MeetsCriterion(x.Id, id, x.Progress[id].Value.Value));
            data.Progress[id].Value += amount;

            if (QuestHelper.MeetsCriterion(data.Id, id, data.Progress[id].Value.Value))
            {
                data.Progress[id].Complete = true;
                data.Progress[id].Value = null;
            }
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

            AddToQuestProgress(id, amount);
        }

        public void AddToVar(string id, long amount, out long previous)
        {
            previous = GetVar(id);
            AddToVar(id, amount);
        }

        public void GiveExp(ulong amount)
        {
            ulong previous = Exp;

            Exp += amount;

            if (Config.Notifier.HasFlag(NotifyAllow.Level))
            {
                int oldLevel = ExpConvert.AsLevel(previous, Ascent);
                int level = Level;

                if (level > oldLevel)
                {
                    Notifier.Add($"Level up! ({LevelViewer.GetLevel(oldLevel, Ascent)} to {LevelViewer.GetLevel(level, Ascent)})");
                }
            }
        }

        public void Give(long value, CurrencyType currency = CurrencyType.Money)
        {
            switch (currency)
            {
                case CurrencyType.Chips:
                    ChipBalance += value;
                    return;

                case CurrencyType.Tokens:
                    TokenBalance += value;
                    return;

                case CurrencyType.Money:

                    if (Debt >= value)
                    {
                        Debt -= value;
                    }
                    else if (Debt > 0)
                    {
                        Balance += value - Debt;
                        Debt = 0;
                    }
                    else
                    {
                        Balance += value;
                    }
                    return;

                case CurrencyType.Debt:
                    Take(value, CurrencyType.Money);
                    return;

                default:
                    throw new ArgumentException("Unknown currency type specified");
            }
        }

        public void Take(long value, CurrencyType currency = CurrencyType.Money)
        {
            switch (currency)
            {
                case CurrencyType.Chips:
                    if (value > ChipBalance)
                        throw new Exception("Unable to take more than the currently specified chip balance");

                    ChipBalance -= value;
                    return;

                case CurrencyType.Tokens:
                    if (value > TokenBalance)
                        throw new Exception("Unable to take more than the currently specified token balance");

                    TokenBalance -= value;
                    return;

                case CurrencyType.Money:
                    if (Balance >= value)
                    {
                        Balance -= value;
                    }
                    else if (Balance > 0)
                    {
                        Debt += value - Balance;
                        Balance = 0;
                    }
                    else
                    {
                        Debt += value;
                    }
                    return;

                case CurrencyType.Debt:
                    Give(value, CurrencyType.Money);
                    return;

                default:
                    throw new ArgumentException("Unknown currency type specified");
            }
        }
    }
}
