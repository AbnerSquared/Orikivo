using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Games
{
    public class SticksGame : GameBuilder
    {
        public override List<PlayerData> OnBuildPlayers(List<Player> players)
        {
            return players.Select((x, i) =>
            {
                return new PlayerData
                {
                    Player = x,
                    Properties = new List<GameProperty>
                    {
                        // position
                        // used to determine turn rotation
                        // it goes in a loop
                        GameProperty.Create<int>("position", i),
                        GameProperty.Create<int>("left_hand", 1),
                        GameProperty.Create<int>("right_hand", 1)
                    }
                };
            }).ToList();
        }

        public override List<GameProperty> OnBuildProperties()
        {
            return new List<GameProperty>
            {
                // next_position
                // Determines the next player that goes
                GameProperty.Create<int>("next_position", 0),

                // current_position
                // Determines the current player that can take their turn.
                GameProperty.Create<int>("current_position", 0),

                // selected_position
                // Determines the selected player to add onto.
                GameProperty.Create<int>("selected_position", 0)
            };
        }


        /*
            For the current player on their turn, they select a player to attack
            
            Once that player is selected, they choose either their right or left hand
            If they are missing a hand, it cannot be chosen.

            The hand that attacks the other player is removed once it can equal 5 exactly

            You cannot attack a player if the hand you attack with goes over 5 

             
             
             */
        public override List<GameCriterion> OnBuildRules(List<PlayerData> players)
        {
            throw new NotImplementedException();
        }

        public override List<GameAction> OnBuildActions(List<PlayerData> players)
        {
            throw new NotImplementedException();
        }

        public override List<DisplayChannel> OnBuildDisplays(List<PlayerData> players)
        {
            throw new NotImplementedException();
        }

        public override void OnSessionStart(GameServer server, GameSession session)
        {
            throw new NotImplementedException();
        }

        public override SessionResult OnSessionFinish(GameSession session)
        {
            return null;
        }
    }
}
