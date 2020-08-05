using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Multiplayer.Games
{
    public class WolfGame
    {
        public bool SharePeeking { get; set; }

        // Handles channel connection if an ability is shared
        public bool IsShared(WerewolfAbility ability)
        {
            return ability switch
            {
                WerewolfAbility.Feast => true,
                WerewolfAbility.Peek => SharePeeking,
                _ => false
            };
        }

        // Get all deaths that were added that weren't handled
        public IEnumerable<WerewolfDeath> GetUnhandledDeaths()
            => Deaths.Where(x => !x.Handled);

        // Store players
        public List<WerewolfPlayer> Players;

        // Store deaths
        public List<WerewolfDeath> Deaths;

        // Keep track of lovers
        // If a user ID already exists in either the key or value, don't add
        public Dictionary<ulong, ulong> Lovers;

        // Store global properties
        public int TotalRounds;

        public WerewolfPhase CurrentPhase;
        public WerewolfPhase NextPhase;

        // Handles ability mechanics
        protected WerewolfAbility HandledAbilities;
        protected WerewolfAbility CurrentAbility;

        // HandleDeath(WerewolfDeath) // If this death doesn't exist in the list of deaths
        // AND the user ID specified doesn't already have a death, add it
        // Otherwise, throw an error

        // HandleDeaths()
        // automatically iterate and handle all unhandled deaths, and then iterate and manage all possible deaths
    }


    /// <summary>
    /// Represents a generic player base for a game.
    /// </summary>
    public interface IBasePlayer
    {
        Player Source { get; }
    }

    /// <summary>
    /// Represents a base game session for a <see cref="GameServer"/>.
    /// </summary>
    public class BaseSession
    {
        // The time at which this session started
        public DateTime StartedAt { get; }

        // The current game that this session is referencing
        public GameBase Game { get; }

        public List<BasePlayer> Players { get; }
    }

    public class BasePlayer
    {
        public Player Source { get; }


    }

    // Represents a generic base game session

    /// <summary>
    /// Represents a generic game session for a <see cref="GameServer"/>.
    /// </summary>
    public interface IBaseSession
    {
        GameBase Game { get; }

        IEnumerable<IBasePlayer> Players { get; }
    }

    // global data
    public class WerewolfSession : IBaseSession
    {
        public WerewolfGame Game { get; }

        GameBase IBaseSession.Game => Game;

        public List<WerewolfPlayer> Players { get; }

        IEnumerable<IBasePlayer> IBaseSession.Players => Players;
    }

    /// <summary>
    /// Represents the status of a player in Werewolf.
    /// </summary>
    [Flags]
    public enum WerewolfStatus
    {
        // The player has no statuses applied
        None = 0,

        // The player is currently dead
        Dead = 1,

        // The player is marked for death
        Marked = 2,

        // The player is currently hurt
        Hurt = 4,

        // The player is currently protected
        Protected = 8,

        // The player is currently convicted
        Convicted = 16,

        // The player is currently accusing someone
        Accusant = 32
    }

    // player data
    public class WerewolfPlayer : IBasePlayer
    {
        public Player Source { get; }

        public int Index { get; }

        public string Name { get; }

        // Represents the status of this player
        public WerewolfStatus Status { get; set; }

        // Represents the vote status of this player
        public WerewolfVote Vote { get; set; }

        public WerewolfRole Role { get; set; }

        // If specified, represents their lover
        public ulong LoverId { get; set; }

        // True if the player has requested a skip
        public bool RequestedSkip { get; set; }

        // True if the player has already used their ability
        public bool UsedAbility { get; set; }

        public bool IsDead()
            => HasStatus(WerewolfStatus.Dead);

        public void ClearStatus(WerewolfStatus status)
            => Status = WerewolfStatus.None;

        public void SetStatus(WerewolfStatus status)
            => Status |= status;

        public void RemoveStatus(WerewolfStatus status)
            => Status &= ~status;

        public bool HasStatus(WerewolfStatus status)
            => Status.HasFlag(status);

        public bool HasAbility()
            => Role.Ability != WerewolfAbility.None;

        public bool HasAbility(WerewolfAbility ability)
            => Role.Ability.HasFlag(ability);

        public bool HasPassive()
            => Role.Passive != WerewolfPassive.None;

        public bool HasPassive(WerewolfPassive passive)
            => Role.Passive.HasFlag(passive);

        public bool CanVote()
            => Vote == WerewolfVote.Pending;
    }

    public enum WerewolfRolePack
    {
        Custom = 0, // your own cards are defined
        Classic = 1 // Play with the default cards: Villager/Seer/Werewolf
    }

    public class WerewolfRole
    {
        public static List<WerewolfRole> GetPack(WerewolfRolePack pack)
        {
            return pack switch
            {
                _ => new List<WerewolfRole>
                {
                    Villager,
                    Seer,
                    Werewolf
                }
            };
        }

        public static readonly WerewolfRole Villager = new WerewolfRole
        {
            Moral = 1,
            Group = WerewolfGroup.Villager,

            Id = "villager",
            Name = "Villager",
            Summary = "You are a villager.",

            Initial = WerewolfInitial.None,
            Passive = WerewolfPassive.None,
            Ability = WerewolfAbility.None,
        };

        public static readonly WerewolfRole Werewolf = new WerewolfRole
        {
            Moral = -7,
            Group = WerewolfGroup.Werewolf,

            Id = "werewolf",
            Name = "Werewolf",
            Summary = "You are a werewolf.",

            Initial = WerewolfInitial.None,
            Passive = WerewolfPassive.Wolfish,
            Ability = WerewolfAbility.Feast
            
        };

        public static readonly WerewolfRole Seer = new WerewolfRole
        {
            Moral = 6,
            Group = WerewolfGroup.Villager,

            Id = "seer",
            Name = "Seer",
            Summary = "You can inspect a player each night to determine if they are a werewolf.",
            
            Initial = WerewolfInitial.None,
            Passive = WerewolfPassive.None,
            Ability = WerewolfAbility.Peek
        };

        public static readonly WerewolfRole ApprenticeSeer = new WerewolfRole
        {
            Moral = 2, // Figure out power level
            Group = WerewolfGroup.Villager,

            Id = "apprentice_seer",
            Name = "Apprentice Seer",
            Summary = "You take up the role of Seer when they die.",

            Initial = WerewolfInitial.None,
            Passive = WerewolfPassive.Apprentice,
            Ability = WerewolfAbility.None
        };

        public static readonly WerewolfRole Lycan = new WerewolfRole
        {
            Moral = -1,
            Group = WerewolfGroup.Villager,

            Id = "lycan",
            Name = "Lycan",
            Summary = "You appear as a werewolf to a seer.",

            Initial = WerewolfInitial.None,
            Passive = WerewolfPassive.Wolfish,
            Ability = WerewolfAbility.None
        };

        public string Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        // If specified, requires to have this many instances of this role when in the pack
        // An example is a mason requiring 2 instances in order to function
        public int RequiredCount { get; set; } = 0;

        // This is used to help generate roles based on player count
        // if < 0, is an enemy, if 0, neutral, if > 0, is good
        public int Moral { get; set; }

        // This is the group that the role is using
        public WerewolfGroup Group { get; set; }

        public WerewolfInitial Initial { get; set; }
        public WerewolfPassive Passive { get; set; } // this is the role's passive ability
        public WerewolfAbility Ability { get; set; } // this is the role's nightly ability

        // figure out role distribution based on players
    }
}
