using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Gaming.Werewolf
{
    public class WerewolfGame
    {
        private static List<WerewolfRole> GetRoles(int playerCount)
        {
            throw new NotImplementedException();
        }

        public WerewolfGame(List<Identity> players)
        {
            List<WerewolfPlayer> gamePlayers = new List<WerewolfPlayer>();
            List<WerewolfRole> roles = GetRoles(players.Count);
            for (int i = 0; i < players.Count; i++)
                gamePlayers.Add(new WerewolfPlayer(players[i], roles[i]));

            Players = gamePlayers;
            CurrentPhase = WerewolfPhase.Opening;
            CurrentRound = 0;
        }

        public int LivingCount => Players.Where(x => !x.Dead).Count();
        public int VillagerCount => Players.Where(x => x.Role.Team == WerewolfTeam.Villager).Count();
        public int WolfCount => Players.Where(x => x.IsWerewolf).Count();

        // only resets at the end of each day.
        private List<ulong> AccusedIds;

        private WerewolfTeam WinningTeam;
        // keeps track of what the seer already checked

        // this stacks throughout the game, only for the original seer. If the original seer dies, this is to be cleared if there is an apprentice seer.
        private List<ulong> ScannedIds { get; set; }

        // Targets reset each main phase transition.
        private List<WerewolfTarget> Targets { get; set; }

        public void SendRoleInfo()
        {
            // message each player directly about information for their role.
            // foreach(WerewolfPlayer player in Players)
            // player.Message("werewolf info goes here");

            // once that is done, check to see if all of the players wish to skip the opening sequence. if they do, go to Day(), otherwise go to Opening()
        }

        // skip this if everyone chooses to skip
        public void StartOpening()
        {
            string openA = "";
            string openB = "";
            // formate the strings to random player names.
            // tell the main tutorial story, and then go to Day().
        }

        public void Morning()
        {
            // after night ends, the morning phase is where all of the targets are checked.
            // if a player was killed, go to Death(), otherwise go to Day()
        }

        public void Day()
        {
            // this is the line of time where everyone just simply talks with each other to determine stuff
            // someone can accuse someone here.
            // or the day can be skipped.
        }

        // a method in which a Player targets someone to be Accused.
        public void Accuse(ulong targetId)
        {
            if (AccusedIds.Contains(targetId))
                return;

            SecondMotion(targetId);
            // make sure to mark the player that accused someone, so the same player can't accuse them again.
        }

        public void SecondMotion(ulong targetId)
        {
            // wait for anyone OTHER than the one accused and the one who accused
            // if nobody responds, mark the player who accused them with the accused id, so they cant accuse them again.
            // if someone agrees, take the accused to Defense(), and tick AccusedCount by 1.
        }

        public void Defense()
        {
            // the accused is given a duration of time to write/say their defense statement
            // once this ends, go directly to trial
        }

        public void Trial()
        {
            // everyone EXCEPT the one accused can vote
            // this can lead to Death (Trial), a return to day, ticking +1 to AccuseCount, or maxing out AccuseCount, going to Night.
        }

        // display a death screen for the player that died, check if the game ends or not.
        public void Death(WerewolfPlayer player)
        {
            // if someone dies (from Trial or Morning target mismatch), proceed to display how they died, with their role.

            // player.Death.GetReport();
            // if (Phase == Day, Phase = Night: if Phase == Morning, Phase = Day)

            // when Tanner is implemented, create a separate check method to see if the person killed was a Tanner.
            // if the Tanner was killed due to Trial(), they win.

            // After everything is displayed, check if any game-ending win conditions were met: CheckWinState()
        }

        public void Night()
        {
            // the state in which everyone who has nightly abilities come to fruition
            // order of operation: Seer -> Werewolf
            // once the night ends, go to Morning() and check all of the targets
            HandleSeer();
            HandleWolves();
        }

        public void HandleSeer()
        {
            List<WerewolfPlayer> notScanned = Players.Where(x => !ScannedIds.Contains(x.Identity.Id)).ToList();
            // store the list of already peeked users for the seer.
            // allow the seer to pick anyone
            // notify ONLY the seer if they are a wolf or not.
        }

        public void HandleWolves()
        {
            List<WerewolfPlayer> living = Players.Where(x => !x.Dead && !x.IsWerewolf).ToList();
            // show a list of users alive
            // allow the werewolves to have a synchronized display to message each other on their choice
            // once picked, the WolfTarget will store the ID.
            // when bodyguards and other roles are implemented later on, this can be used to deny a death.
        }

        public void CheckWinState()  // WerewolfPhase incomingPhase
        {
            if (WolfCount == 0)
                WinningTeam = WerewolfTeam.Villager;
            else if (VillagerCount == WolfCount)
                WinningTeam = WerewolfTeam.Werewolf;

            // check if villagers killed all werewolves
            // check if werewolves are parity to villagers

            // uf villagers win, close, or if werewolves win, close.

            // otherwise, if the death resulted from the last night, and Morning(), go to Day().
            // if the death resulted from Trial() (Day()), go to Night().
        }

        public void Close()
        {
            // The winning team is to be displayed accordingly.
            // display random fun facts, etc.
            // Create a pop-up to see if anyone wishes to play again
            // if enough agree, refresh the game following the same initial ruleset.
            // otherwise, close the game, and return to the main game lobby layout.
        }

        public Display Display { get; } // this is what the main game sees
        public Display SeerDisplay { get; } // this is the display, only shown for the Seer.
        public Display WerewolfDisplay { get; } // this is the display, only shown for the Werewolf.
        // for the message collector, make it to where it only focuses on the werewolf DM channels, and listen for correct command input.
        // MessageCollector(List<Command> commands);
        // for the MessageCollector system, the MessageReceived is always active, and checks if each message matches a filter. If it doesn't, it returns, otherwise:
        // it invokes the delegate that the player called.
        public WerewolfPhase CurrentPhase { get; private set; } = WerewolfPhase.Opening;
        public int CurrentRound { get; private set; } = 0;
        public List<WerewolfPlayer> Players { get; }
    }
}
