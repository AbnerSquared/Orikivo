using System;
using System.Text;

namespace Orikivo.Gaming.Werewolf
{

    public class WerewolfTarget
    {
        public WerewolfTarget(ulong attackerId, ulong targetId, WerewolfAbility abilityUsed)
        {
            AttackerId = attackerId;
            TargetId = targetId;
            AbilityUsed = abilityUsed;
        }

        public ulong AttackerId;
        public ulong TargetId;
        public WerewolfAbility AbilityUsed;
    }

    public enum WerewolfPhase
    {
        Opening = 1, // Simply the introduction. within it: Roles (everyone is given their role), Introductory (the 'tutorial' of the game)
        Day = 2, // can start with: Death (someone died) // can end with: Death (someone voted to die) // within it: Discuss (default day), Accusation (call someone out), Defense (accused writes a defense), Second Motion (if anyone seconds the accusation), Trial (final votes)
        Night = 3, // within it: Seer (ability to see if someone is a werewolf), Werewolf (they all discuss on who to kill)
        Closing = 4 // Simply the results page. can end with: Opening (if everyone wishes to play again)
    }

    public enum WerewolfTeam
    {
        Villager = 1,

        Werewolf = 2
    }

    public class WerewolfRole
    {
        public WerewolfTeam Team;
        public int Strength;
        public WerewolfPassive? Passive;
        public WerewolfAbility? Ability;
    }

    public enum WerewolfPassive
    {

    }
    
    public enum WerewolfAbility
    {
        Seer = 1, // ability to see right through people
        Werewolf = 2 // ability to kill 1 person a night
    }

    // represents an Attribute that is used to mark a property as a GameAttribute
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GamePropertyAttribute : Attribute
    {
        public string Name { get; }
        public GamePropertyAttribute(string name) : base()
        {
            Name = name;
        }
    }

    public class WerewolfPlayer : IPlayer
    {
        public WerewolfPlayer(Identity identity, WerewolfRole role)
        {
            Identity = identity;
            Role = role;
        }

        public Identity Identity { get; }

        [GameProperty("role")]
        public WerewolfRole Role { get; }

        [GameProperty("dead")]
        public bool Dead { get; } = false;

        public bool IsWerewolf => Role.Team == WerewolfTeam.Werewolf;

    }
}
