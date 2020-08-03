using System.Collections.Generic;

namespace Arcadia.Multiplayer.Games
{
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
