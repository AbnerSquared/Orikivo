using Arcadia.Multiplayer.Games.Werewolf;

namespace Arcadia.Multiplayer.Games
{
    public class WolfPlayer : PlayerBase
    {
        public int Position { get; set; }

        public string Name { get; set; }

        public string InitialRoleId { get; set; }

        public WolfRole Role { get; set; }

        public bool IsWinner { get; set; }

        public WolfVote Vote { get; set; } = 0;

        public WolfStatus Status { get; set; } = 0;

        public bool RequestedSkip { get; set; }

        public bool HasUsedAbility { get; set; }

        public override PlayerBase GetDefault()
        {
            return new WolfPlayer();
        }
    }
}