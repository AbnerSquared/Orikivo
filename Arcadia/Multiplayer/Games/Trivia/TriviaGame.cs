using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Games
{
    public class TriviaGame : GameBuilder
    {
        // Now for this:
        // Two more things need to be made:
        // Timers: This will allow an action to remain active for the time specified. When the timer runs out, a GameAction can be specified
        // Rulesets: This will allow a current action or state to change if any criteria is met (x player scores 5, or time runs out)
        
        

        public override List<GameAction> OnLoadActions()
        {
            var actions = new List<GameAction>();

            var getQuestionResult = new GameAction
            {
                Id = "get_question_result",
                OnExecute = delegate (GameServer server, GameSession session)
                {

                }
            };
            // 'get_question_result'
            // This action is executed from 'get_next_question'

            // This shows the result of the question that they answered

            // A timer is set for 3 seconds
            // When the timer runs out, the action 'try_get_next_question' is executed

            var tryGetNextQuestion = new GameAction
            {
                Id = "try_get_next_question",
                OnExecute = delegate (GameServer server, GameSession session)
                {

                }
            };
            // 'try_get_next_question'
            // This action is executed right after 'get_question_result'
            // If the rule 'has_answered_all_questions' is true, the action 'get_results' is executed
            // Otherwise, if that rule is false, the action 'get_next_question' is executed instead

            var getNextQuestion = new GameAction
            {
                Id = "get_next_question",
                OnExecute = delegate (GameServer server, GameSession session)
                {

                }
            };
            // 'get_next_question'
            // This action sets the display to frequency 10

            // This action is executed from OnSessionStart()
            // This action is executed from 'try_get_next_question'
            // This action is executed from 'restart'

            // This iterates through to get the next available question

            // A timer is set for 30 seconds
            // When the timer runs out, the action 'get_question_result' is executed

            // The rule 'has_all_players_answered' is set for this action
            // If 'has_all_players_answered' is true, the action 'get_question_result' is executed
            // Otherwise, the game is left as is

            var getResults = new GameAction
            {
                Id = "get_results",
                OnExecute = delegate (GameServer server, GameSession session)
                {
                    // set all currently playing connection to frequency 12
                    foreach(ServerConnection connection in server.Connections.Where(x => x.State == GameState.Playing))
                    {
                        connection.Frequency = 12;
                    }

                    session.GetDisplay(12).Content.GetComponent("leaderboard").Draw(session.Players
                        .OrderByDescending(x => x.GetAttribute("score"))
                        .Select(x => $"{x.Player.User.Username}: {x.GetAttribute("score")}"));
                }
            };

            // 'get_results'
            // This action sets the display to frequency 12

            // This action is executed from 'try_get_next_question'

            // This action simply shows the leaderboard of all players

            // A timer is set for 15 seconds
            // When the timer runs out, the session is ended

            // The rule 'most_players_want_rematch' is set for this action
            // If 'most_players_want_rematch' is true, the action 'restart' is executed

            var restart = new GameAction
            {
                Id = "restart",
                OnExecute = delegate (GameServer server, GameSession session)
                {
                    // reset all player attributes
                    foreach (PlayerSessionData data in session.Players)
                    {
                        foreach (GameProperty attribute in data.Attributes)
                        {
                            attribute.Reset();
                        }
                    }

                    // reset all game attributes
                    foreach (GameProperty attribute in session.Attributes)
                    {
                        attribute.Reset();
                    }

                    session.ExecuteAction("try_get_next_question");
                }
            };
            // 'restart'
            // This action resets all game and player attributes, and restarts the game as it was new
            // After everything is reset, the action 'try_get_next_question' is executed 

            throw new NotImplementedException();
        }
        
        
        public override List<GameRule> OnLoadRules()
        {
            var rules = new List<GameRule>();

            // 'has_all_players_answered'
            // This rule simply checks to see if: 'players_voted' == Players.Count
            var hasAllPlayersAnswered = new GameRule
            {
                Id = "has_all_players_answered",
                Criterion = delegate(GameSession session)
                {
                    return ((int)(session.GetAttribute("players_answered").Value)) == session.Players.Count;
                }
            };


            // 'most_players_want_rematch'
            // This rule checks to see if: 'rematch_requests' is >= (Players.Count / 2)
            var mostPlayersWantRematch = new GameRule
            {
                Id = "most_players_want_rematch",
                Criterion = delegate(GameSession session)
                {
                    return ((int)(session.GetAttribute("rematch_requests").Value)) >= (int)(session.Players.Count / 2);
                }
            };

            rules.Add(hasAllPlayersAnswered);
            rules.Add(mostPlayersWantRematch);

            return rules;
        }

        public override List<GameProperty> OnLoadAttributes()
        {
            var attributes = new List<GameProperty>();

            // 'rematch_requests'
            // This keeps track of all players wanting to rematch
            var rematchRequests = GameProperty.Create<int>("rematch_requests", 0);


            // This attribute is set from the freqency 12:rematch input
            // This attribute is reset from 'restart'

            // 'players_answered'
            // This keeps track of all players that have answered
            var playersAnswered = GameProperty.Create<int>("players_answered", 0);

            // This attribute is set from frequency 10:a,b,c, or d input
            // This attribute is reset from 'get_question_result'

            var currentQuestion = GameProperty.Create<int>("current_question", 0);

            attributes.Add(rematchRequests);
            attributes.Add(playersAnswered);
            attributes.Add(currentQuestion);

            return attributes;
        }

        // create display channels here
        // inputs are specified here
        public override List<DisplayChannel> OnLoadDisplays()
        {
            var displays = new List<DisplayChannel>();
            // frequency 10 (question)
            // This is what displays all of the questions and allows for answering
            // The action 'show_question_result' is executed when:
            // - All current players chose an answer
            // - The timer of 30 seconds since the question started runs out

            var question = new DisplayChannel
            {
                Frequency = 10,
                Content = new DisplayContent
                {
                    Components = new List<IComponent>
                    {
                        new Component
                        {
                            Active = true,
                            Id = "question_header",
                            Position = 0,
                            Formatter = new ComponentFormatter
                            {
                                BaseFormatter = "**Question {0}**\n{1}",
                                OverrideBaseIndex = true
                            }
                        },
                        new ComponentGroup
                        {
                            Active = true,
                            Id = "answer_box",
                            Position = 1,
                            Capacity = 4,
                            Formatter = new ComponentFormatter
                            {
                                BaseFormatter = "{0}",
                                ElementFormatter = "- {0}",
                                Separator = "\n",
                                OverrideBaseIndex = true
                            }
                        }
                    }
                }
            };

            // Display 2: Question Result (Frequency 11)
            // This is what shows what the actual answer was
            // There is a basic timer of 3 seconds before executing the action 'try_get_next_question'
            var questionResult = new DisplayChannel
            {
                Frequency = 11,
                Content = new DisplayContent
                {
                    Components = new List<IComponent>
                    {
                        new Component
                        {
                            Active = true,
                            Id = "question_header",
                            Position = 0,
                            Formatter = new ComponentFormatter
                            {
                                BaseFormatter = "**Question {0}**\n{1}",
                                OverrideBaseIndex = true
                            }
                        },
                        new ComponentGroup
                        {
                            Active = true,
                            Id = "answer_box",
                            Position = 1,
                            Capacity = 4,
                            Formatter = new ComponentFormatter
                            {
                                BaseFormatter = "{0}",
                                ElementFormatter = "~~- {0}~~",
                                Separator = "\n",
                                OverrideBaseIndex = true
                            }
                        }
                    }
                }
            };


            var results = new DisplayChannel
            {   Frequency = 12,
                Content = new DisplayContent
                {
                    Components = new List<IComponent>
                   {
                       new ComponentGroup
                       {
                           Active = true,
                           Formatter = new ComponentFormatter
                           {
                               Separator = "\n",
                               BaseFormatter = "**Results**\n{0}",
                               ElementFormatter = "{0}",
                               OverrideBaseIndex = true
                           },
                           Id = "leaderboard",
                           Position = 0
                       }
                   }
                },
                Inputs = new List<IInput>
                {
                    new TextInput
                    {
                        Name = "rematch",
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            server.Session.AddToAttribute("rematch_requests", 1);
                        }
                    }
                }
            };
            // frequency 12 (results)
            // This shows the scores of each player
            // An input 'rematch' is set
            // 


            // the display should simply handle the showing and replying to questions

            displays.Add(question);
            displays.Add(questionResult);
            displays.Add(results);

            return displays;
        }

        public override List<PlayerSessionData> OnLoadPlayers(List<Player> players)
        {
            // this is the list of all attributes the players need
            var attributes = new List<GameProperty>();

            var score = GameProperty.Create<int>("score", 0);

            // if the answer they selected was correct, set to true; otherwise false
            var vote = GameProperty.Create<bool>("answer", false);
           
            attributes.Add(score);
            attributes.Add(vote);

            var sessionData = players.Select(x => new PlayerSessionData { Player = x, Attributes = attributes });

            return sessionData.ToList();
        }

        public override void OnSessionStart(GameSession session)
        {
            throw new NotImplementedException();
        }

        


        /*
         **Question 1**
         Solve the following problem: 2 + 2

         A. 3
         B. 6
         C. 4
         D. 20
         */

        /*
         **Question 1**
         Solve the following problem: 2 + 2
         
         ~~A. 3~~
         ~~B. 6~~
         C. 4
         ~~D. 20~~
         */
    }
}
