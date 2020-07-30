namespace Arcadia.Multiplayer.Games
{
    public class WerewolfRole
    {
        public static readonly WerewolfRole Villager = new WerewolfRole();
        public static readonly WerewolfRole Wolf = new WerewolfRole();
        public static readonly WerewolfRole Seer = new WerewolfRole();

        public string Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        // if < 0, is an enemy, if 0, neutral, if > 0, is good
        public int Moral { get; set; }

        // If true, this role is a wolf-related entry.
        public bool IsWolfLike { get; set; }

        // If a seer is killed, this role will become a seer
        public bool InheritSeer { get; set; }

        // This is the group that the role is using
        public WerewolfGroup Group { get; set; }

        public WerewolfInitial Initial { get; set; }
        public WerewolfPassive Passive { get; set; } // this is the role's passive ability
        public WerewolfAbility Ability { get; set; } // this is the role's nightly ability

        // figure out role distribution based on players
    }
}