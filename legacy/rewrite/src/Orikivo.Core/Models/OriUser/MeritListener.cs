using Newtonsoft.Json;
using Orikivo.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class MeritListener
    {
        public MeritListener()
        {
            Merits = CacheIndex.Merits.Merits;
        }

        public List<Merit> Merits { get; }

        /*
        public void Listen(OriUser user)
        {
            foreach(Merit merit in Merits.Where(x => !x.EqualsAny(user.Cache.Merits.Claimed.Enumerate(y => y.Source).ToArray())))
            {
                if (merit.MetCriteria(user))
                    user.Cache.Merits.Log(merit);
            }
        }
        */
    }

    public class ClaimMerit
    {
        public ClaimMerit(Merit merit)
        {
            Source = merit;
            Claimed = merit.Rewards == null ? true : false;
            EarnedAt = DateTime.UtcNow;
        }
        public Merit Source { get; }
        public bool Claimed { get; private set; }
        public DateTime EarnedAt { get; }
        /*
        public void Claim(OriUser user)
        {
            if (Claimed)
                return;

            if (Source.Rewards == null)
                return;
            foreach (Reward reward in Source.Rewards)
            {
                switch (reward.Type)
                {
                    case RewardType.Item:
                        if (reward.Item == null)
                            throw new Exception("The Reward.Item required to award is missing.");
                        user.Inventory.Store(reward.Item, reward.Amount);
                        break;
                    case RewardType.Money:
                        user.Wallet.Give(reward.Amount);
                        break;
                }
            }

            Claimed = true;
        }
        */
    }

    // merits must have seperate goal tracking.
    public class Merit
    {
        public Merit(ushort id, ushort rank, ushort group, string name, string description, bool classified, string iconUrl = "")
        {
            Id = id;
            RankId = rank;
            GroupId = group;
            Name = name;
            Description = description;
            Classified = classified;
            _iconUrl = iconUrl;
        }

        public Merit(ushort id, ushort rank, ushort group, string name, string description, bool classified, MeritCriterion criterion, Reward reward, string iconUrl = "") : this(id, rank, group, name, description, classified, iconUrl)
        {
            Criteria = new List<MeritCriterion>();
            Criteria.Add(criterion);

            Rewards = new List<Reward>();
            Rewards.Add(reward);
        }

        public Merit(ushort id, ushort rank, ushort group, string name, string description, bool classified, MeritCriterion criterion, List<Reward> rewards, string iconUrl = "") : this(id, rank, group, name, description, classified, iconUrl)
        {
            Criteria = new List<MeritCriterion>();
            Criteria.Add(criterion);

            Rewards = rewards;
        }

        public Merit(ushort id, ushort rank, ushort group, string name, string description, bool classified, List<MeritCriterion> criteria, Reward reward, string iconUrl = "") : this(id, rank, group, name, description, classified, iconUrl)
        {
            Criteria = criteria;

            Rewards = new List<Reward>();
            Rewards.Add(reward);
        }

        [JsonConstructor]
        public Merit(ushort id, ushort rank, ushort group, string name, string description, bool classified, List<MeritCriterion> criteria, List<Reward> rewards, string iconUrl = "") : this(id, rank, group, name, description, classified, iconUrl)
        {
            Criteria = criteria;
            Rewards = rewards;
        }
        
        [JsonProperty("id")]
        public ushort Id { get; }
        [JsonProperty("rank")]
        public ushort RankId { get; }
        [JsonProperty("group")]
        public ushort GroupId { get; }

        [JsonProperty("name")]
        public string Name { get; }
        [JsonProperty("description")]
        public string Description { get; }
        [JsonProperty("classified")]
        public bool Classified { get; } // if the criteria is visible or not.
        [JsonProperty("criteria")]
        public List<MeritCriterion> Criteria { get; }
        [JsonProperty("rewards")]
        public List<Reward> Rewards { get; } // the pool of items to be granted.

        [JsonProperty("icon_url")]
        private string _iconUrl;
        public string IconUrl { get { return string.IsNullOrWhiteSpace(_iconUrl) ? "" : $"{Locator.Resources}merits//icons//{_iconUrl}"; } }

        public string IdValue { get { return $"{GroupId.ToString("00")}{Id.ToString("000")}{RankId.ToString("00")}"; } }


        /*
        public bool MetCriteria(OriUser user)
        {
            foreach(MeritCriterion criterion in Criteria)
                if (!criterion.Check(user))
                    return false;
            
            return true;
        }
        */
    }

    // the reward of a challenge or merit.
    public class Reward
    {
        public Reward(RewardType type, ulong amount, OriItem item = null)
        {
            Type = type;

            switch(type)
            {
                case RewardType.Item:
                    if (item == null)
                        throw new Exception("RewardType.Item requires that an item be specified to reward.");
                    else
                        Item = item;
                    break;
            }

            Amount = amount;
        }

        [JsonProperty("reward_type")]
        public RewardType Type { get; }
        [JsonProperty("item")] // as item_id
        public OriItem Item { get; } // the item to be rewarded.
        [JsonProperty("amount")]
        public ulong Amount { get; } // however much should be rewarded, if the item is null, it will automate to money.
    }

    public class MeritCriterion
    {
        [JsonConstructor]
        public MeritCriterion(MeritGoalType goal, ulong bound, EventType? eventType = null, MeritValueType? type = null)
        {
            if (!(bound > 0))
                throw new Exception("The upper bound of a merit cannot be equal to or less than 0.");

            Goal = goal;
            
            switch(Goal)
            {
                // MeritGoalType.Collect
                case MeritGoalType.Own:
                    if (!type.HasValue)
                        throw new Exception("MeritGoalType.Own requires that MeritValueType be used for its criterion.");
                    Type = type;
                    break;
                case MeritGoalType.Event:
                    if (!eventType.HasValue)
                        throw new Exception("MeritGoalType.Event requires that EventType be used for its criterion.");
                    Event = eventType;
                    break;
            } // add default?

            Bound = bound;
        }

        [JsonProperty("goal")]
        public MeritGoalType Goal { get; }
        [JsonProperty("value_type")]
        public MeritValueType? Type { get; }
        [JsonProperty("event_type")]
        public EventType? Event { get; }
        [JsonProperty("bound")]
        public ulong Bound { get; }
        /*
        public bool Check(OriUser user)
        {
            switch(Goal)
            {
                case MeritGoalType.Own:
                    if (Type == null)
                        throw new Exception("The MeritValueType required to check a criterion is null.");
                    switch (Type)
                    {
                        case MeritValueType.Balance:
                            if (user.Wallet.Balance > Bound)
                                return true;
                            break;
                        case MeritValueType.Debt:
                            if (user.Wallet.Debt > Bound)
                                return true;
                            break;
                    }
                    break;
                case MeritGoalType.Event:
                    if (Event == null)
                        throw new Exception("The EventType required to check a criterion is null.");
                    switch(Event)
                    {
                        case EventType.Daily:
                            if (user.Cache.Events[EventType.Daily] > Bound)
                                return true;
                            break;
                        case EventType.Vote:
                            if (user.Cache.Events[EventType.Vote] > Bound)
                                return true;
                            break;
                        case EventType.Spectral:
                            if (user.Cache.Events[EventType.Spectral] > Bound)
                                return true;
                            break;
                    }
                    break;
            }

            return false;
        }
        */
    }

    public enum RewardType
    {
        Item = 1,
        Money = 2
    }

    public enum MeritGoalType
    {
        Collect = 1, // if the user has to collect something
        Own = 2, // if the user has to own n amount of something.
        Event = 3 // if the user has to go through an event
    }

    public enum MeritValueType
    {
        Balance = 1, // the user has to collect balance
        Debt = 2 // the user has to collect debt
    }

    public enum EventType
    {
        Daily = 1, // the user has to check in daily
        Vote = 2, // the user has to vote
        Midas = 3, // the user has to experience a midas touch
        Spectral = 4 // the user has to visit the other side of the casino
    }
}
