using System.Collections.Generic;

namespace Arcadia.Multiplayer.Games.Werewolf
{
    public class WolfRole
    {
        public static List<WolfRole> GetPack(WolfRolePack pack)
        {
            return pack switch
            {
                _ => new List<WolfRole>
                {
                    Villager,
                    Seer,
                    Werewolf
                }
            };
        }

        public static readonly WolfRole Villager = new WolfRole
        {
            Moral = 1,
            Group = WolfGroup.Villager,
            Id = "villager",
            Name = "Villager",
            Summary = "You are a villager.",
            Initial = 0,
            Passive = 0,
            Ability = 0,
        };

        public static readonly WolfRole Werewolf = new WolfRole
        {
            Moral = -7,
            Group = WolfGroup.Werewolf,
            Id = "werewolf",
            Name = "Werewolf",
            Summary = "You are a werewolf.",
            Initial = 0,
            Passive = WolfPassive.Wolfish,
            Ability = WolfAbility.Feast
        };

        public static readonly WolfRole Seer = new WolfRole
        {
            Moral = 6,
            Group = WolfGroup.Villager,
            Id = "seer",
            Name = "Seer",
            Summary = "You can inspect a player each night to determine if they are a werewolf.",
            Initial = 0,
            Passive = 0,
            Ability = WolfAbility.Peek
        };

        public static readonly WolfRole ApprenticeSeer = new WolfRole
        {
            Moral = 2, // Figure out power level
            Group = WolfGroup.Villager,

            Id = "apprentice_seer",
            Name = "Apprentice Seer",
            Summary = "You inherit the role of Seer when they die.",

            Initial = 0,
            Passive = WolfPassive.Apprentice,
            Ability = 0
        };

        public static readonly WolfRole Lycan = new WolfRole
        {
            Moral = -1,
            Group = WolfGroup.Villager,

            Id = "lycan",
            Name = "Lycan",
            Summary = "You appear as a werewolf to a seer.",

            Initial = 0,
            Passive = WolfPassive.Wolfish,
            Ability = 0
        };

        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        // By default, this is 0, but this specifies the amount of instances required if this role is selected
        // An example is a mason requiring 2 instances in order to function
        public int RequiredCopyCount { get; set; } = 0;

        public int Moral { get; set; }

        public WolfGroup Group { get; set; }

        public WolfInitial Initial { get; set; }

        public WolfPassive Passive { get; set; }

        public WolfAbility Ability { get; set; }

        public WolfAbilityUsage AbilityUsage { get; set; }
    }

    // If initial, the ability is called at the start of the first night
    // If nightly, the ability is called per night
    // If once, the ability can be called only once per night
}
