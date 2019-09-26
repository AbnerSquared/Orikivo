using Newtonsoft.Json;

namespace Orikivo
{
    /*
        Cool stats to take info of
        Oldest User >
        Oldest User in Guild >
        LargeGuild Check
        Splash URL >
        Content Filter Check > (Specify mode)
        AFK Timeout >
        Guild Creation Date >
        Guild Join Date >

        Get Game Statistics, show most common game.
        Get Discriminators, organizable.
        Game Tracker >>> Keep track of all UserActivity names.
         
         Channel Lockdown
         Read Server bans
         make a pins channel, that keeps track of top messages.
         allow a user to tab a channel, or to auto-create.
         allow a suggestion inclusion, seperate from reports.
         allow global user blacklist.
         
         
         */
    /// <summary>
    /// Represents as the stats manager of an Account.
    /// </summary>
    public class Analyzer
    {
        [JsonProperty("trading")]
        public Account2TradingBox Trading { get; set; }

        [JsonProperty("messaging")]
        public Account2InteractionBox Messaging { get; set; }

        [JsonProperty("interaction")]
        public Account2UsageBox Interaction { get; set; }

        [JsonProperty("balance")]
        public Account2BalanceBox Balance { get; set; }

        [JsonProperty("gaming")]
        public Account2GamingBox Gaming { get; set; }

        [JsonProperty("gambling")]
        public Account2GamblingBox Gambling { get; set; }
    }
}
