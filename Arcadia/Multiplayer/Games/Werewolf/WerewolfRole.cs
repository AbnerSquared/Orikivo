using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Multiplayer.Games
{
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

        // By default, this is 0, but this specifies the amount of instances required if this role is selected
        // An example is a mason requiring 2 instances in order to function
        public int GroupSize { get; set; } = 0;

        public int Moral { get; set; }

        public WerewolfGroup Group { get; set; }

        public WerewolfInitial Initial { get; set; }

        public WerewolfPassive Passive { get; set; }

        public WerewolfAbility Ability { get; set; }
    }
}
