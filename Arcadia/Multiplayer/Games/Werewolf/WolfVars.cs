namespace Arcadia.Multiplayer.Games
{
    internal static class WolfVars
    {
        // PLAYER
        internal static readonly string IsWinner = "is_winner";
        internal static readonly string Index = "index";
        internal static readonly string Name = "name";
        internal static readonly string InitialRoleId = "initial_role_id";
        internal static readonly string Role = "role";
        internal static readonly string Vote = "vote";
        internal static readonly string Status = "status";
        internal static readonly string HasRequestedSkip = "has_requested_skip";
        internal static readonly string HasUsedAbility = "has_used_ability";

        // GLOBAL
        internal static readonly string CurrentRound = "current_round";
        internal static readonly string CurrentDeath = "current_death";
        internal static readonly string Deaths = "deaths";
        internal static readonly string Peeks = "peeks";
        internal static readonly string Lovers = "lovers";
        internal static readonly string Suspect = "suspect";
        internal static readonly string Accuser = "accuser";
        internal static readonly string IsOnTrial = "is_on_trial";
        internal static readonly string CurrentPhase = "current_phase";
        internal static readonly string NextPhase = "next_phase";
        internal static readonly string CurrentAbility = "current_ability";
        internal static readonly string HandledAbilities = "handled_abilities";
        internal static readonly string AbilityVotes = "ability_votes";
        internal static readonly string WinningGroup = "winning_group";

        // ACTIONS
        internal static readonly string Start = "start";
        internal static readonly string StartAbility = "start_ability";
        internal static readonly string StartDay = "start_day";
        internal static readonly string StartNight = "start_night";
        internal static readonly string StartVote = "start_vote";
        internal static readonly string EndConviction = "end_conviction";
        internal static readonly string EndDay = "end_day";
        internal static readonly string EndNight = "end_night";
        internal static readonly string EndVote = "end_vote";
        internal static readonly string EndAbility = "end_ability";
        internal static readonly string HandleAbilities = "handle_abilities";
        internal static readonly string HandleDeath = "handle_death";
        internal static readonly string HandleDeaths = "handle_deaths";
        internal static readonly string HandleTrial = "handle_defense";
        internal static readonly string TryEndAbility = "try_end_ability";
        internal static readonly string TryEndPhase = "try_end_phase";
        internal static readonly string TryEndVote = "try_end_vote";
        internal static readonly string GetResults = "get_results";
    }
}