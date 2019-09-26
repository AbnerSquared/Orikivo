using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo
{
    public class OriUser
    {
        public OriUser()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public OriUser(SocketUser user) : this() // new account from default build.
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
        }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("id")]
        public ulong Id { get; }

        [JsonProperty("name")]
        public string Username { get; private set; }

        [JsonProperty("tag_id")]
        public string Discriminator { get; private set; }
    }

    public class OriUserInventory
    {
        public List<OriItem> Items { get; }
        public void Store(OriItem item, ulong amount = 1)
        {
            for (ulong i = 0; i < amount; i++)
                Items.Add(item);
        }
    }

    /*
    public class BankCollection
    {
        public List<BankWallet>? Accounts {get; set;}
        public Wallet Wallet {get; set;}
    }
    */
    
    public enum CurrencyType
    {
        Guild = 0, // guild coins
        User = 1, // user coins
        Voter = 2, // voter tokens
    }

    /*
    public class Wallet
    {
        public CurrencyType Currency {get; set;} // the type of currency.
        public ulong Balance {get; set;} // the balance
        public List<Fine> Fines {get; set;} // the collection of fines appended on a wallet.
        //public ulong Debt {get; set;} // the debt pool
    }*/

    public class Fine
    {
        public CurrencyType Currency {get; set;} // the type of currency they have to pay back.
        public DateTime Date {get; set;} // the date this fine was ensued.
        public TimeSpan Duration {get; set;} // the amount of time they have until the fine is a violation.
        public ulong Amount {get; set;} // the funds required to pay off the fine.
    }

    public class AccountCache
    {
        public PlazaCache Plaza {get; set;} // cache of all stored events/interactions/outcomes/etc.
        public MeritCache Merits {get; set;} // cache of all earned merits
        public UpgradeCache Upgrades {get; set;} // cache of all stored upgrades
        public CooldownCache Cooldowns {get; set;} // cache of all cooldowns
        public ArcadeCache Arcade {get; set;} // data cache of all long-term/active games.
        public CasinoCache Casino {get; set;} // data cache of all long-term/active gambles.
        public LicenseCache License {get; set;} // data cache of the last license built; used to determine if the card is up to date/if it needs to render again.
    }

    public class CooldownCache { }
    public class PlazaCache { }
    public class UpgradeCache { }
    public class ArcadeCache { }
    public class CasinoCache { }

    // an interface for a class that contains items, from which is required to be retrieved once starting the bot.
    // as these objects define everything on the bot.
    public interface IDefinerCache
    {
        void Retrieve();
    }

    public class LicenseCache
    {   
        public string AvatarId {get; set;}
        public string RenderingOptions {get; set;} // a string that defines itself as a PixelRenderingOptions, compressed.
    }

    public class ActivityLog
    {
        // logging components
        public WordLog Words {get; set;} // a list of words the account has used.
        public ulong MessagesSent {get; set;}
        public ulong AttachmentsSent {get; set;}
        public CachedMessage LastMessage {get; set;} // date of message sent, location of message sent


        // subclass logging components
        public ArcadeLog Arcade {get; set;} // log of arcade history
        public CasinoLog Casino {get; set;} // log of casino events
        public UsageLog Usage {get; set;} // log of bot usage.
        public InventoryLog Inventory {get; set;} // log of item collections.
        public ActionLog Actions {get; set;} // log of all actions (trading, buying/selling, etc.)
    }
    public class WordLog { }
    public class CachedMessage { }

    public class ArcadeLog { }
    public class InventoryLog { }
    public class ActionLog { }

    public class UsageLog
    {
        public ulong UsageCount {get; set;}
        public ulong ErrorCount {get; set;}
        public List<CommandLog> Commands {get; set;}
        public double SuccessRate {get; set;} // the success rate of commands executed.
    }

    public class CasinoLog
    {
        // append a limiter.
        public List<CasinoGameResult> History {get; set;} // a list of all recent games
        public GiveOrTakeLog GiveOrTake {get; set;}
    }

    public class CasinoGameResult
    {
        public bool Won {get; set;}
        public ulong Wager {get; set;}
        public ulong Winnings {get; set;}
        public double Risk {get; set;}
    }

    public class GiveOrTakeLog
    {
        public ulong TimesPlayed {get; set;}

        public ulong Wins {get; set;} // a general count on wins
        public ulong Losses {get; set;} // a general count on losses.

        public ulong Midas {get; set;} // rare win
        public ulong Curses {get; set;} // rare loss
        public ulong Earns {get; set;} // normal win
        public ulong Steals {get; set;} // normal loss

        public ulong HighestMidasChain {get; set;} // the highest midas chain earned
        public ulong HighestCurseChain {get; set;} // the highest curse chain earned
        public ulong HighestEarnChain {get; set;} // highest earn chain
        public ulong HighestStealChain {get; set;} // highest steal chain

        public ulong HighestWinChain {get; set;} // generic winning chain
        public ulong HighestLossChain {get; set;} // generic losses chain

        public ulong LargestValueChainPool {get; set;}
        public ulong LargestDebtChainPool {get; set;}

        public ulong TotalValuePool {get; set;}
        public ulong TotalDebtPool {get; set;}

        public ulong MidasChain {get; set;}
        public ulong CurseChain {get; set;}
        public ulong EarnChain {get; set;}
        public ulong StealChain {get; set;}
        
        public ulong WinChain {get; set;}
        public ulong LossChain {get; set;}

        public ulong ValueChainPool {get; set;}
        public ulong DebtChainPool {get; set;}
    }

    public class CommandLog { }
}