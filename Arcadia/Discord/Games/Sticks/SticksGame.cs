using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer.Games
{
    public static class StickVars
    {
        public static readonly string Position = "position";
        public static readonly string RightHand = "right_hand";
        public static readonly string LeftHand = "left_hand";
    }
    public class StickGame : GameBase
    {
        public override string Id => "sticks";

        public override GameDetails Details => new GameDetails
        {

        };

        public override List<PlayerData> OnBuildPlayers(in IEnumerable<Player> players)
        {
            return players.Select((x, i) =>
            {
                return new PlayerData
                {
                    Source = x,
                    Properties = new List<GameProperty>
                    {
                        GameProperty.Create("position", i),
                        GameProperty.Create("left_hand", 1),
                        GameProperty.Create("right_hand", 1)
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

        public override List<GameAction> OnBuildActions()
        {
            throw new NotImplementedException();
        }

        public override List<GameBroadcast> OnBuildBroadcasts(List<PlayerData> players)
        {
            throw new NotImplementedException();
        }

        public override async Task StartAsync(GameServer server, GameSession session)
        {
            throw new NotImplementedException();
        }

        public override GameResult OnGameFinish(GameSession session)
        {
            return null;
        }
    }
}
