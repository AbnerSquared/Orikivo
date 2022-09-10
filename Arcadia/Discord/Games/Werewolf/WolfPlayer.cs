using Arcadia.Multiplayer.Games;
using Arcadia.Multiplayer.Games.Werewolf;

namespace Arcadia.Multiplayer
{
    public class WolfPlayer : PlayerBase
    {
        [Property("position")]
        public int Position { get; internal set; }

        [Property("name")]
        public string Name { get; internal set; }

        [Property("initial_role_id")]
        public string InitialRoleId { get; internal set; }

        [Property("role")]
        public WerewolfRole Role { get; internal set; }

        [Property("is_winner")]
        public bool IsWinner { get; internal set; }

        [Property("vote")]
        public WerewolfVote VoteState { get; internal set; }

        [Property("status")]
        public WerewolfStatus Status { get; internal set; }

        [Property("has_requested_skip")]
        public bool HasRequestedSkip { get; internal set; }

        [Property("has_used_ability")]
        public bool HasUsedAbility { get; internal set; }

        public override PlayerBase GetDefault()
        {
            return new WolfPlayer
            {
                IsWinner = true,
                VoteState = WerewolfVote.Pending,
                Status = WerewolfStatus.None
            };
        }
    }
}
