namespace Arcadia.Multiplayer.Games
{
    internal static class WolfVars
    {
        internal static readonly string Role = "role";

        internal static readonly string IsWinner = "is_winner";

        // if true, the player is dead and CANNOT interact with the game at all
        internal static readonly string IsDead = "is_dead";

        internal static readonly string DeathFrom = "death_from";

        // if true, the player will die at the end of the night
        internal static readonly string IsHurt = "is_hurt";

        // if a player was protected at the start of a day phase, set to false and prevent death
        internal static readonly string IsProtected = "is_protected";

        // If true, this player has already been showcased as dead
        internal static readonly string HasClosure = "has_closure";

        internal static readonly string Vote = "vote";

        // if they can inherit peek, can_peek is set to TRUE if a seer dies
        internal static readonly string InheritPeek = "inherit_peek";

        // if they are a peeker, initialize a list of user ids to track
        internal static readonly string CanPeek = "can_peek";

        // if they can feast, they vote for a player to kill alongside everyone else that can feast
        internal static readonly string CanFeast = "can_feast";

        // if they can protect, they choose a player to keep safe
        internal static readonly string CanProtect = "can_protect";

        // if not dead and injured, they will day upon the end of the next day phase.
        
        
        // if a player is marked for death at the start of a day phase AND they are not protected, kill the player
        internal static readonly string MarkedForDeath = "marked_for_death";

        // if a player has this value set AND is killed, the lover must also die
        internal static readonly string LoverId = "lover_id";

        // Dictionary<ulong, bool>
        internal static readonly string PeekedPlayerIds = "peeked_player_ids";

        // this keeps a track of all players killed, stored as WerewolfKill.
        internal static readonly string Kills = "kills";

        // GLOBAL PROPERTIES


        internal static readonly string LastPlayerKilled = "last_player_killed";
        internal static readonly string CurrentPhase = "current_phase";
        internal static readonly string NextPhase = "next_phase";
        internal static readonly string Suspect = "suspect";
        internal static readonly string RequestedSkips = "requested_skips";
        internal static readonly string ReadInputs = "read_inputs";

        // if true, this will not invoke the GetDeaths call on each round.
        internal static readonly string HasCheckedDeaths = "has_checked_deaths";

        // this keeps track of all of the nights:
        // if NightCount == 0:
        // 
        internal static readonly string TotalNights = "total_nights";

        // ACTIONs

        // This starts the game as a whole
        internal static readonly string Start = "start";

        // This is called once a criteria has been met 
        internal static readonly string GetResults = "get_results";

        internal static readonly string StartDay = "start_day";
        internal static readonly string EndDay = "end_day";
        internal static readonly string StartTrial = "start_trial";

        internal static readonly string StartNight = "start_night";

        // This is invoked for when the night ends
        internal static readonly string EndNight = "end_night";

        internal static readonly string StartVoteInput = "start_vote_input";
        internal static readonly string EndVoteInput = "end_vote_input";

        // This is called from handle_deaths OR end_vote_input

        // This reads the LastPlayerKilled property to get that specific player to:
        //    - Reveal their role
        //    - What they were killed by
        internal static readonly string OnDeath = "on_death";

        // This is called at the start of a day
        internal static readonly string HandleDeaths = "handle_deaths";


        // This is called once the night starts AND there is a seer
        internal static readonly string StartPeekInput = "start_peek_input";
        internal static readonly string EndPeekInput = "end_peek_input";

        // This is called once the night starts AND there are werewolves
        internal static readonly string StartFeastInput = "start_feast_input";
        internal static readonly string EndFeastInput = "end_feast_input";

        internal static readonly string StartProtectInput = "start_protect_input";
        internal static readonly string EndProtectInput = "end_protect_input";

        internal static readonly string StartHuntInput = "start_hunt_input";
        internal static readonly string EndHuntInput = "end_hunt_input";

        internal static readonly string TrySkipPhase = "try_skip_phase";

        internal static readonly string WolfGreaterEqualsVillager = "wolf_greater_equals_villager";
        internal static readonly string AllDeadWolf = "all_dead_wolf";
    }
}