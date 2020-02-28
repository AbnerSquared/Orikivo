using System.Collections.Generic;

namespace Orikivo
{
    public class WerewolfRole
    {
        private WerewolfRole()
        {
            Passives = new List<WerePassiveType>();
            Abilities = new List<WereAbilityType>();
        }
        public static WerewolfRole FromRole(WerewolfRoleType roleType)
        {
            WerewolfRole role = new WerewolfRole();
            switch (roleType)
            {
                case WerewolfRoleType.Werewolf:
                    role.Name = "Werewolf";
                    role.Morality = -6;
                    return role;
                case WerewolfRoleType.Seer:
                    role.Name = "Seer";
                    role.Morality = 3;
                    return role;
                case WerewolfRoleType.Tanner:
                    role.Name = "Tanner";
                    role.Morality = 0;
                    return role;
                default:
                    role.Name = "Villager";
                    role.Morality = 1;
                    return role;
            }
        }

        public int Morality { get; private set; } // -6 for werewolf
        public WereTeamType Team
        {
            get
            {
                if (Morality > 0)
                    return WereTeamType.Villagers;
                if (Morality < 0)
                    return WereTeamType.Werewolves;
                return WereTeamType.Neutral;
            }
        }
        public string Name { get; private set; }
        public List<WerePassiveType> Passives { get; }
        public List<WereAbilityType> Abilities { get; }
        public WereWinType WinCondition
        {
            get
            {
                if (Team == WereTeamType.Villagers)
                    return WereWinType.KillAllWerewolves;
                if (Team == WereTeamType.Werewolves)
                    return WereWinType.SameVillagerCount;
                return WereWinType.Hanged;
            }
        }
    }
}
