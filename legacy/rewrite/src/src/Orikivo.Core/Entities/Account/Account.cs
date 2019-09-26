using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Orikivo.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class ExceptionInfo
    {
        public ExceptionInfo(Exception ex)
        {
            Data = ex.Data;
            Message = ex.Message ?? "";
            StackTrace = ex.StackTrace;
        }

        public ICollection Data { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    /// <summary>
    /// Represents a SocketUser on Orikivo.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Constructs a new Account based off of a SocketUser.
        /// </summary>
        public Account(SocketUser u)
        {
            Id = u.Id;
            Username = u.Username;
            Discriminator = u.DiscriminatorValue;
            CreationDate = DateTime.Now;
        }

        /// <summary>
        /// Constructs a new Account with its creation date set to when it was constructed.
        /// </summary>
        public Account()
        {
            CreationDate = DateTime.Now;
        }



        //Components
        ///<summary>
        /// A check to see if the profile card is up to date on information.
        ///</summary>
        [JsonIgnore]
        public bool Updated { get; set; }

        ///<summary>
        /// A check to see if the profile card is currently refreshing.
        ///</summary>
        [JsonIgnore]
        public bool IsBuilding { get; set; }

        ///<summary>
        /// A value used to identify an account. Inherits from Discord.WebSocket.SocketUser.
        ///</summary>
        [JsonProperty("id")]
        public ulong Id { get; set; }

        ///<summary>
        /// The written username of an account. Inherits from Discord.WebSocket.SocketUser.
        ///</summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        ///<summary>
        /// A sub-value used to identify an account, in the occurance of having the exact same name.
        ///</summary>
        [JsonProperty("discriminator")]
        public ushort Discriminator { get; set; }

        ///<summary>
        /// The System.DateTime component of when this account was created.
        ///</summary>
        [JsonProperty("createdat")]
        public DateTime CreationDate { get; set; }

        ///<summary>
        /// An optional component displaying basic information.
        ///</summary>
        [JsonProperty("status")]
        public PersonalityChart Personality { get; set; }

        ///<summary>
        /// The available amount of money that can be spent.
        ///</summary>
        [JsonProperty("balance")]
        public ulong Balance { get; set; } = 0;

        ///<summary>
        /// A bank account keeping track of wallets.
        ///</summary>
        [JsonProperty("offshore")]
        public OffshoreBalance Offshore { get; set; }

        ///<summary>
        /// A pool of fines that prevent income until it is empty.
        ///</summary>
        [JsonProperty("debt")]
        public ulong Debt { get; set; } = 0;

        ///<summary>
        /// The reset counter of an account.
        ///</summary>
        [JsonProperty("prestige")]
        public ulong Prestige { get; set; }

        ///<summary>
        /// Experience group derived from experience.
        ///</summary>
        [JsonProperty("level")]
        public ulong Level { get; set; }

        ///<summary>
        /// The raw percentile level value.
        ///</summary>
        [JsonProperty("rawlevel")]
        public double RawLevel { get; set; }

        ///<summary>
        /// The current value of experience earned.
        ///</summary>
        [JsonProperty("xp")]
        public ulong Experience { get; set; }

        ///<summary>
        /// Keeps track of all earned rewards.
        ///</summary>
        [JsonProperty("merits")]
        public MeritCollection Merits { get; set; }

        ///<summary>
        /// Keeps track of upgrades.
        ///</summary>
        //[JsonProperty("upgrades")]
        //public ComponentCollection Upgrades { get; set; }

        ///<summary>
        /// Manages all cooldowns.
        ///</summary>
        //[JsonProperty("cooldowns")]
        //public CooldownManager Cooldowns { get; set; }

        ///<summary>
        /// The basis of how an account functions.
        ///</summary>
        [JsonProperty("options")]
        public AccountOptions Options { get; set; }

        ///<summary>
        /// Controls when an account is notified.
        ///</summary>
        [JsonProperty("notifiers")]
        public Notificator Notifiers { get; set; }

        ///<summary>
        /// Contains the current personal status of a user.
        ///</summary>
        [JsonProperty("status")]
        public UserDisplayStatus Status { get; set; }

        ///<summary>
        /// Keeps track of items.
        ///</summary>
        [JsonProperty("inventory")]
        public Account2Inventory Inventory { get; set; }

        ///<summary>
        /// Holds all saved playlists.
        ///</summary>
        //[JsonProperty("playlists")]
        //public List<Playlist2> Playlists { get; set; }

        ///<summary>
        /// Contains a collection of saved events.
        ///</summary>
        [JsonProperty("events")]
        public EventCollection Events { get; set; }

        /// <summary>
        /// Contains a collection of favorite Clipboards.
        /// </summary>
        [JsonProperty("starred")]
        public ClipboardCollection Starred { get; set; }

        ///<summary>
        /// A mailbox that follows all inbound letters.
        ///</summary>
        [JsonProperty("mail")]
        public Mailbox Mail { get; set; }

        ///<summary>
        /// A core component that keeps a tab on every action performed.
        ///</summary>
        [JsonProperty("stats")]
        public Analyzer Analytics { get; set; }

        /// <summary>
        /// Defines how much money the user can hold.
        /// </summary>
        [JsonProperty("walletsize")]
        public ulong WalletCapacity { get; set; }
        //#endregion

        // Static methods used for certain scenarios.
        #region ReadOnly

        /// <summary>
        /// The remaining wallet space a user can store.
        /// </summary>
        [JsonIgnore]
        public ulong WalletSpace { get { return WalletCapacity - Balance; } }

        /// <summary>
        /// Checks if the user currently has expendable funds.
        /// </summary>
        [JsonIgnore]
        public bool HasMoney { get { return !(Balance == 0); } }

        /// <summary>
        /// Checks if the user currently has fines in the debt pool.
        /// </summary>
        [JsonIgnore]
        public bool InDebt { get { return !(Debt == 0); } }

        /// <summary>
        /// Returns the name of this account.
        /// </summary>
        [JsonIgnore]
        public string Name { get { return Options.Nickname ?? Username ?? $"U{Id}"; } }

        /// <summary>
        /// Returns the base name of this account.
        /// </summary>
        [JsonIgnore]
        public string DefaultName { get { return $"{Username}#{Discriminator}"; } }

        /// <summary>
        /// Returns the Discord.WebSocket.SocketUser counterpart.
        /// </summary>
        [JsonIgnore]
        public SocketUser User { get { return Global.Client.GetUser(Id); } }
        #endregion

        // Methods that return a summary of a component.
        #region ReadMethods
        #endregion

        /// <summary>
        /// Checks if the user can afford to spend a specified value.
        /// </summary>
        public bool CanExpend(double v)
            => !(Balance - v < 0);

        /// <summary>
        /// Checks if the user has room to store a specified value.
        /// </summary>
        public bool CanStore(double v)
            => !(Balance + v > WalletCapacity);

        // long is allowed to go negative.
        public long TrueBalance { get { return (long)Balance - (long)Debt; } }

        // Actions that alter the components of an account.
        #region Methods
        /// <summary>
        /// Replaces the current balance of the user to a specified value.
        /// </summary>
        public void SetBalance(ulong balance)
        {
            balance = balance > WalletCapacity ? WalletCapacity : balance;
            Balance = balance;
            //Analytics.TryUpdateMaxHeld(Balance);
        }

        /// <summary>
        /// Completely wipes all money for the user.
        /// </summary>
        public void ClearBalance()
        {
            Balance = 0;
        }

        /// <summary>
        /// Gives the user a specified value.
        /// </summary>
        public void Give(ulong v)
        {
            Balance += CanStore(v) ? v : WalletSpace;
            //Analytics.TryUpdateMaxHeld(Balance);
        }

        /// <summary>
        /// Takes money or what's left of it, if it goes over.
        /// </summary>
        public void TakeRemainder(ulong v)
        {
            Balance -= CanExpend(v) ? v : Balance;
            //Analytics.UpdateExpended(v);
        }

        /// <summary>
        /// Takes money from the user.
        /// </summary>
        private void Take(ulong v)
        {
            Balance -= v;
            //Analytics.UpdateExpended(v);
        }

        /// <summary>
        /// Attempts to buy an item.
        /// </summary>
        public bool TryBuy(Item i)
            => TryTake(i.Cost);

        /// <summary>
        /// Attempts to take money from the user.
        /// </summary>
        public bool TryTake(ulong v)
        {
            if (!HasMoney)
                return false;

            if (!CanExpend(v))
                return false;

            Take(v);
            return true;
        }

        /// <summary>
        /// Attempts to take money from the user or what's left of it, if it goes over.
        /// </summary>
        public bool TryTakeRemainder(ulong v)
        {
            if (!CanExpend(v))
            {
                if (Options.Overflow)
                    v = Balance;
                else
                    return false;
            }

            TakeRemainder(v);
            return true;
        }

        /// <summary>
        /// Sends money to another user.
        /// </summary>
        public void Pay(OldAccount a, ulong v)
        {
            if (!HasMoney)
                return;

            if (!CanExpend(v))
            {
                if (Options.Overflow)
                    v = Balance;
                else
                    return;
            }
            Take(v);
        }

        /// <summary>
        /// Take money from another user.
        /// </summary>
        public void Steal(OldAccount a, ulong v)
        {

        }

        /// <summary>
        /// Adds money to the debt pool.
        /// </summary>
        public void Fine(ulong v)
        {
            Debt += v;
            //Analytics.UpdateExpended(v);
        }

        /// <summary>
        /// Automatically subtracts given money from the debt pool.
        /// </summary>
        private void PayDebt()
        {
            if (!InDebt)
                return;

            if (!CanExpend(Debt))
            {
                ulong v = (ulong)(Debt - Math.Abs(Balance - (double)Debt));
                Take(v);
                Debt -= v;
                return;
            }

            Take(Debt);
            ClearDebt();
        }

        /// <summary>
        /// Completely wipes all debt.
        /// </summary>
        public void ClearDebt()
            => Debt = 0;
        
        #endregion

        // Methods that overwrite a base function.
        #region Overloads
        public bool Equals(Account a)
            => Id == a.Id;

        public override string ToString()
            => $"a::{DefaultName}";
        #endregion
    }

    public class Account2Inventory
    {
        public ulong Capacity { get; set; }
        public List<TradeOffer> Trades { get; set; }
        public List<Item> Personals { get; set; }
        public List<Item> Consumables { get; set; }
        // public List<Item> Consumables { get { return Items.Where(x => x.IsSingleUse).ToList();}}

    }

    public class Account2TradingBox
    {
        [JsonProperty("tradecount")]
        public ulong TradesMade { get; set; }

        //[JsonProperty("tradehistory")]
        //public List<TradeHistory> History { get; set; }
    }

    public class Account2InteractionBox
    {
        [JsonProperty("spokenwords")]
        public List<SpokenContext> Spoken { get; set; }

        [JsonProperty("sentfiles")]
        public ulong FileCount { get; set; }

        [JsonProperty("sentmessages")]
        public ulong MessageCount { get; set; }

        [JsonProperty("mentions")]
        public ulong Mentions { get; set; }

        [JsonProperty("mdlang")]
        public MarkdownLanguage FavoriteMarkdown { get; set; }

        //[JsonProperty("recent")]
        //public CompactMessage Recent { get; set; }

        [JsonProperty("lasttyped")]
        public DateTime LastTyped { get; set; }

        [JsonProperty("lastsent")]
        public DateTime LastSent { get; set; }

        [JsonProperty("swears")]
        public ulong Swears { get; set; }
    }

    public class Account2UsageBox
    {
        [JsonProperty("usedcommands")]
        public ulong Executed { get; set; }

        [JsonProperty("errors")]
        public ulong Exceptions { get; set; }

        //[JsonProperty("commands")]
        //public List<CommandData> Commands { get; set; }
    }

    public class Account2BalanceBox
    {
        //[JsonProperty("transactions")]
        //public List<Transaction2> Transactions { get; set; }

        [JsonProperty("totallost")]
        public BankStatement Expended { get; set; }

        [JsonProperty("totalwon")]
        public BankStatement Earned { get; set; }

        [JsonProperty("largestbalance")]
        public BankStatement MostHeld { get; set; }

        [JsonProperty("largestdebt")]
        public BankStatement MostOwed { get; set; }

        [JsonProperty("mostlost")]
        public Receipt MostSpent { get; set; }

        [JsonProperty("mostwon")]
        public Receipt MostEarned { get; set; }
        
    }

    public class Account2GamingBox
    {
        //[JsonProperty("games")]
        //public List<GameData> Games { get; set; }
    }

    public class GiveOrTakeAnalyzer
    {
        public int GoldenCount { get; set; }
        public int PlayCount { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public int WinStreak { get; set; }
        public int LossStreak { get; set; }
        public long WinStreakAmount { get; set; }
        public long LossStreakAmount { get; set; }
        public int MaxWinStreak { get; set; }
        public int MaxLossStreak { get; set; }
        public long MaxWinStreakAmount { get; set; }
        public long MaxLossStreakAmount { get; set; }

        public int MaxLossAmountStreak { get; set; }
        public long MaxLossAmount { get; set; }
        public int MaxWinAmountStreak { get; set; }
        public long MaxWinAmount { get; set; }
        public long WinAmount { get; set; }
        public long LossAmount { get; set; }
        public long TotalOutcome { get { return WinAmount - LossAmount; } }

        public void Track(bool b, bool g, long u)
        {
            PlayCount += 1;
            if (b)
            {
                if (g)
                {
                    GoldenCount += 1;
                }
                else
                {
                    WinCount += 1;
                }
                WinAmount += u;
            }
            else
            {
                LossCount += 1;
                LossAmount += u;
            }

            if (!b)
            {
                WinStreak = 0;
                LossStreak += 1;
                WinStreakAmount = 0;
                LossStreakAmount += u;
            }
            else
            {
                WinStreak += 1;
                LossStreak = 0;
                LossStreakAmount = 0;
                WinStreakAmount += u;
            }

            if (WinStreak > MaxWinStreak)
            {
                MaxWinStreakAmount = WinStreakAmount;
                MaxWinStreak = WinStreak;
            }
            if (LossStreak > MaxLossStreak)
            {
                MaxLossStreakAmount = LossStreakAmount;
                MaxLossStreak = LossStreak;
            }
            if (WinStreakAmount > MaxWinAmount)
            {
                MaxWinAmount = WinStreakAmount;
                MaxWinAmountStreak = WinStreak;
            }
            if (LossStreakAmount > MaxLossAmount)
            {
                MaxLossAmount = LossStreakAmount;
                MaxLossAmountStreak = LossStreak;
            }
        }
    }

    public class Account2GamblingBox
    {
        //[JsonProperty("bestwin")]
        //public WagerResult LowestWin { get; set; }

        //[JsonProperty("worstloss")]
        //public WagerResult HighestLoss { get; set; }

        [JsonProperty("mostwagered")]
        public Receipt MostWagered { get; set; }

        [JsonProperty("wagered")]
        public BankStatement Wagered { get; set; }

        [JsonProperty("topmode")]
        public WagerMode Favorite { get; set; }

        //[JsonProperty("bets")]
        //public List<WagerResult> BetHistory { get; set; }
    }

    public class DoubleOrNothingAnalyzer
    {

    }

    /// <summary>
    /// Defines the basic properties of a betting analyzer.
    /// </summary>
    public interface IBetAnalyzer
    {
        int PlayCount { get; set; }
        int WinCount { get; set; }
        int LossCount { get; set; }

    }
}
