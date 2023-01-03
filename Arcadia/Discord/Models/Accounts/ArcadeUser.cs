﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Arcadia.Graphics;
using Discord;
using Orikivo;

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
            Stats = new Dictionary<string, long>();
            Merits = new Dictionary<string, BadgeData>();
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
            Dictionary<string, long> stats,
            Dictionary<string, BadgeData> merits, List<BoostData> boosters, Dictionary<string, ChallengeData> challenges,
            List<QuestData> quests, List<ItemData> items, CardConfig card,
            Dictionary<string, CatalogHistory> catalogHistory)
            : base(id, username, discriminator, createdAt, config)
        {
            Stats = stats ?? new Dictionary<string, long>();
            Merits = merits ?? new Dictionary<string, BadgeData>();
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

        [JsonIgnore]
        public long Exp
        {
            get => GetVar(Vars.Experience);
            set => SetVar(Vars.Experience, value);
        }

        [JsonIgnore]
        public int Level => ExpConvert.AsLevel(Exp, Ascent);

        [JsonIgnore]
        public long Ascent
        {
            get => GetVar(Vars.Ascent);
            set => SetVar(Vars.Ascent, value);
        }

        [JsonProperty("stats")]
        public Dictionary<string, long> Stats { get; }

        [JsonProperty("merits")]
        public Dictionary<string, BadgeData> Merits { get; }

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

        [JsonIgnore]
        public List<TradeOffer> Offers { get; } = new List<TradeOffer>();

        [JsonIgnore]
        public DateTime? GlobalCooldown { get; set; }

        [JsonIgnore]
        public bool IsInSession { get; set; } = false;

        [JsonIgnore]
        public bool HasBeenNoticed { get; set; } = false;

        [JsonIgnore]
        public Wager LastFundsLost { get; set; }

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
            {
                Stats[id] += amount;
                AddToQuestProgress(id, amount);
            }

            
        }

        public void AddToVar(string id, long amount, out long previous)
        {
            previous = GetVar(id);
            AddToVar(id, amount);
        }

        public void GiveExp(long amount)
        {
            long previous = Exp;
            Exp += amount;

            if (Config.Notifier.HasFlag(NotificationType.LevelUpdated))
            {
                int oldLevel = ExpConvert.AsLevel(previous, Ascent);

                if (Level > oldLevel)
                    Notifier.Notifications.Add(NotificationFactory.CreateLevelUpdated(oldLevel, Level, Ascent));
            }
        }

        public void Give(long value, CurrencyType currency = CurrencyType.Cash)
        {
            switch (currency)
            {
                case CurrencyType.Token:
                    ChipBalance += value;
                    return;

                case CurrencyType.Favor:
                    TokenBalance += value;
                    return;

                case CurrencyType.Cash:

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
                    if (Config.AutoPayDebt)
                        Take(value, CurrencyType.Cash);
                    else
                        Debt += value;
                    return;

                default:
                    throw new ArgumentException("Unknown currency type specified");
            }
        }

        public void Take(long value, CurrencyType currency = CurrencyType.Cash)
        {
            switch (currency)
            {
                case CurrencyType.Token:
                    if (value > ChipBalance)
                        throw new Exception("Unable to take more than the currently specified chip balance");

                    ChipBalance -= value;
                    return;

                case CurrencyType.Favor:
                    if (value > TokenBalance)
                        throw new Exception("Unable to take more than the currently specified token balance");

                    TokenBalance -= value;
                    return;

                case CurrencyType.Cash:
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
                    Give(value, CurrencyType.Cash);
                    return;

                default:
                    throw new ArgumentException("Unknown currency type specified");
            }
        }
    }
}
