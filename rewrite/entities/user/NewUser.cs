using System;
using System.Collections.Generic;

namespace Unstable
{
    public class User
    {
        ulong Balance; // Money
        ulong Debt; // Negative Money
        ulong TokenCount; // Voting Tokens
        ulong Exp; // Raw Level
        int Level; // [JsonIgnore] ExpConvert.AsLevel(Exp)
        int Ascent; // Exp Prestige
        Dictionary<ulong, UserGuildData> Guilds; // GuildId, { ulong Balance, ulong Exp, ulong MessageExp }
        Dictionary<string, int> Stats; // Id, Value
        Dictionary<string, CooldownData> Cooldowns; // Id, { DateTime ExecutedAt, int Streak }
        Dictionary<string, DateTime> ProcessCooldowns; // [JsonIgnore] Id, ExpiresOn
        BoostData[] Boosters; // { string Type, float Rate, DateTime ExecutedAt }
        Dictionary<string, int> Upgrades; // Id, Tier
        MeritData[] Merits; // { string Id, DateTime AchievedAt, bool? IsClaimed }
        CardConfig Card; // idk yet lol
        UserConfig Config; // { string Prefix, Nickname, Privacy, Notify, { Risk, BetUpperBound }, { DefaultExpectedTick, WinMethod }, { DefaultDice } }
    }

    public class UserConfig
    {
        string Prefix;
        string Nickname;
        Privacy Privacy;
        NotifyDeny Notify;
        GimiConfig Gimi;
        DoubleConfig Double;
        DiceConfig Dice;
    }

    public enum Privacy
    {
        Public = 1,
        Local = 2,
        Private = 3
    }

    public class GimiConfig
    {
        int Risk;
        int BetUpperBound;
    }

    public class DoubleConfig
    {
        int DefaultExpectedTick;
        DoubleMethod WinMethod;
    }

    public enum DoubleMethod
    {
        Exact = 1,
        UnderOrExact = 2
    }

    public class DiceConfig
    {
        Dice DefaultDice;
    }

    public class Dice
    {
        int Sides;
        int SideLength;
    }

    [Flags]
    public enum NotifyDeny
    {
        None = 1,
        LevelGain = 2,

    }

    public class CardConfig
    {
        // idk lol
    }

    public class MeritData
    {
        ulong AchievedTicks;
        bool? IsClaimed;
    }

    public class BoostData
    {
        string Type;
        float Rate;
        ulong ExecutedTicks;
    }

    public class CooldownData
    {
        ulong ExecutedTicks;
        int Streak;
    }

    public class UserGuildData
    {
        ulong Balance;
        ulong Exp;
        ulong MessageExp;
    }
}
