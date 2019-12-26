using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Guild
    {
        ulong Id;
        ulong OwnerId;
        ulong Balance;
        ulong Exp;
        DateTime CreatedAt;
        List<GuildObjective> Objectives;
        List<GuildEvent> Events;
        List<GuildCommand> Commands;
        GuildConfig Config;
    }

    public class Embedder
    {
        string Author;
        string AuthorIconUrl;
        string HeaderText;
        string HeaderUrl;
        string FooterText;
        string FooterIconUrl;
        string ThumbnailUrl;
        OriColor Color;
        DateTime? Timestamp;
        List<Discord.EmbedFieldBuilder> Fields;
    }

    public class MessageBuilder
    {
        Embedder Embedder;
        string Content;
        string Url;
    }

    public class Message
    {
        Discord.Embed Embed;
        bool IsTTS;
        string Text;
    }

    public class GuildObjective : Objective
    {
        int RequiredUsers;
    }

    public class GuildConfig
    {
        string Prefix;
    }

    public class User
    {
        [JsonConstructor]
        internal User(ulong id, string username, string discriminator, DateTime createdAt,
            ulong balance, ulong tokenBalance, ulong debt, Dictionary<ulong, GuildData> connections,
            ObjectiveData objectives, List<ClaimableInfo> cooldowns, Dictionary<ExpType, ulong> exp,
            Dictionary<string, object> stats, List<MeritData> merits, List<BoosterData> boosters,
            HuskBrain brain, Husk husk)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
            CreatedAt = createdAt;
            Balance = balance;
            TokenBalance = tokenBalance;
            Debt = debt;
            Connections = connections;
            Objectives = objectives;
            Cooldowns = cooldowns;
            Exp = exp;
            Stats = stats;
            Merits = merits;
            Boosters = boosters;
            Brain = brain;
            Husk = husk;
        }

        public User(Discord.WebSocket.SocketUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
            CreatedAt = DateTime.UtcNow;
        }

        // Discord
        [JsonProperty("id")]
        public ulong Id { get; }

        [JsonProperty("username")]
        public string Username { get; private set; }
        
        [JsonProperty("discriminator")]
        public string Discriminator { get; private set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        // Balances

        [JsonProperty("balance")]
        public ulong Balance { get; private set; } = 0; // from earnings/winnings
        
        [JsonProperty("tokens")]
        public ulong TokenBalance { get; private set; } = 0; // from voting
        
        [JsonProperty("debt")]
        public ulong Debt { get; private set; } = 0; // from negative funds

        // guild data

        [JsonProperty("connections")]
        public Dictionary<ulong, GuildData> Connections { get; } = new Dictionary<ulong, GuildData>();

        [JsonProperty("objectives")]
        public ObjectiveData Objectives { get; } = new ObjectiveData();
        
        [JsonProperty("cooldowns")]
        public List<ClaimableInfo> Cooldowns { get; } = new List<ClaimableInfo>();
        
        [JsonIgnore]
        public Dictionary<string, DateTime> InternalCooldowns { get; } = new Dictionary<string, DateTime>();

        // experience

        [JsonProperty("exp")]
        public Dictionary<ExpType, ulong> Exp { get; } = new Dictionary<ExpType, ulong>();
        // stats (the stored object type of a stat must be defined elsewhere)
        
        [JsonProperty("stats")]
        public Dictionary<string, object> Stats { get; } = new Dictionary<string, object>(); // Type:{JSON} (example: Int32:0)
        
        [JsonProperty("merits")]
        public List<MeritData> Merits { get; } = new List<MeritData>();

        [JsonProperty("boosters")]
        public List<BoosterData> Boosters { get; } = new List<BoosterData>();

        [JsonProperty("brain")]
        public HuskBrain Brain { get; } = new HuskBrain();
        
        [JsonProperty("husk")]
        public Husk Husk { get; private set; } = null;

        // [JsonProperty("config")]
        // public UserConfig Config { get; } = UserConfig.Default;

        // [JsonProperty("card")]
        // public CardConfig Card { get; } = CardConfig.Default;

        // updates the username and discriminator of a user if anything was changed.
        public void SetIdentity(Discord.WebSocket.SocketUser user)
        {
            if (Id != user.Id)
                throw new Exception("The user specified must have the same matching ID as the account.");

            Username = user.Username;
            Discriminator = user.Discriminator;
        }
    }

    public class Item
    {
        string Id;
        string Name;
        string Summary;
        List<string> Quotes;
        ItemRarity Rarity;
        ulong Value;
        bool CanSell;
        bool CanBuy;
        int? TradeLimit;
        int? GiftLimit;
        bool BypassCriteriaOnGift;
        ItemAction Action;
        List<VarCriterion> ToUnlock;
        List<VarCriterion> ToOwn;
    }

    public class HuskBrain
    {
        public List<WorldRelation> Relations { get; }
        public List<string> DiscoveredAreaIds { get; }
    }

    public class WorldArea
    {
        string Id;
        string Name;
        List<Construct> Constructs;
        List<Npc> Npcs;
    }

    public class Sector
    {
        string Id;
        string Name;
        string ImagePath;
        List<WorldArea> Areas;
    }

    public class Construct // a simple building or such in an area.
    {
        string Id;
        string Name;
        List<Npc> Npcs;
    }

    public class Npc
    {
        string Id;
        string Name;
        NpcPersonality Personality;
        List<WorldRelation> InitialRelations;
        List<NpcDialogue> Dialogue;
    }

    public class NpcDialogue
    {
        NpcDialogueTrigger Trigger;
        List<string> Responses;
    }

    public enum NpcDialogueTrigger
    {
        Question
    }

    public class Vendor
    {
        string Id;
        string Name;
        List<VendorDialogue> Dialogue;
        float SellRate;
        float BuyRate;
        List<WorldItemTag> LikedItemTags;
        List<WorldItemTag> DislikedItemTags;
        bool OnlyBuyLikedItems;
        List<TimeBlock> Schedule;
    }

    public struct TimeBlock
    {
        TimeSpan From;
        TimeSpan To;
    }

    public enum WorldItemTag
    {
        Socket
    }

    public class VendorDialogue
    {
        VendorDialogueTrigger Trigger;
        List<string> Responses;
    }

    public enum VendorDialogueTrigger
    {
        Buy,
        BuyLiked,
        Sell,
        SellLiked,
        MissingFunds
    }

    public class Market
    {
        string Id;
        string Name;
        string ExteriorImagePath;
        string InteriorImagePath;
        
        List<WorldItemTag> TagTable; // the tags of the groups of items that it can sell.
        List<Vendor> Vendors;

        // Get schedule from Vendor shifts.
    }

    [Flags]
    public enum NotifyDeny
    {
        Level = 1,
        Mail = 2,
        Error = 4,
        All = Level | Mail | Error
    }

    public enum NpcPersonality
    {
        Default
    }

    public class WorldRelation
    {
        int NpcId;
        float Value; // 0.0 to 1.0
    }

    public class Husk
    {
        public DateTime ClaimedAt { get; }
        public Backpack Backpack { get; private set; }
        public HuskState State { get; private set; }
        
    }

    public enum HuskState
    {
        Active,
        Injured
    }

    public class Backpack
    {
        int Capacity;
        List<WorldItem> Items;

    }

    public class WorldItem
    {
        string Id;
        string Name;
        List<WorldItemTag> Tags;
        ulong Value;
        ItemAction OnUse;
    }

    public class ItemAction // this applies to both world and digital items.
    {
        int? UseLimit;
        TimeSpan? CooldownLength;
        List<VarPacket> OnUse; // or Action<User> OnUse
        bool BreakOnLastUse;
        TimeSpan? DecayLength;
    }

    public enum ItemTag
    {
        Entity,
        Renderable,
        Colorable
    }

    public class World

    {
        string MapImageUrl;
        string Name;
        List<Sector> Sectors;
        GlobalMarket Market;
        // sectors are safe places, anything else could be called a Field
    }

    public class GlobalMarket
    {

    }

    // cooldowns that exist from votes, checking in, etc.
    public class ClaimableInfo
    {
        // the Claimable class defines the cooldowns and reawards for streaks
        DateTime ClaimedAt;
        int CurrentStreak;
    }

    public class BoosterData
    {
        public double Rate { get; }
        public DateTime ConsumedAt { get; }
        public int? UseCount { get; }
        public string Id { get; }
    }

    public class ObjectiveData
    {
        public int CompletedObjectives { get; private set; }
        public DateTime LastAssigned { get; private set; }
        public List<Objective> CurrentObjectives { get; private set; }

        public DateTime LastSkipped { get; private set; }
    }

    public class Objective // GuildObjective : IObjective
    {
        // how hard an objective is.
        public int Rank { get; }
        public IReadOnlyList<ObjectiveCriterion> Criteria { get; }
        public Dictionary<string, int> Trackers { get; }
    }

    public class ObjectiveCriterion
    {
        public string Name { get; }
        public int ExpectedValue { get; }
    }
    // used to set up objectives.
    public class ObjectiveInfo
    {
        public int Rank { get; set; }
        public List<ObjectiveCriterion> Criteria { get; set; }
    }

    public class GuildData
    {
        public static GuildData Empty = new GuildData { Exp = 0, ActiveExp = 0, LastMessage = null };

        [JsonProperty("exp")]
        public ulong Exp { get; private set; } // total exp earned from participation within in a guild

        [JsonProperty("last_sent")]
        public DateTime? LastMessage { get; private set; } // the last time a message was sent in this guild.
        
        [JsonProperty("active")]
        public ulong ActiveExp { get; private set; }
        // exp - (decay total from last message)
        // each time a set number of days pass w/o participation, attribute an exp decay
    }
}
