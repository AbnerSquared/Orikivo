using Discord;
using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Games
{

    public class TriviaGame : GameBuilder
    {
        public static List<TriviaQuestion> Questions => new List<TriviaQuestion>
        {
            new TriviaQuestion("2+2=", TriviaTopic.Math, TriviaDifficulty.Easy, "4", "3", "fish", "dude i can't math why are you doing this", "answer"),
            new TriviaQuestion("4*2=", TriviaTopic.Math, TriviaDifficulty.Easy, "8", "42", "6", "2", "2.000001", "Int32.MinValue"),
            new TriviaQuestion("sqrt(4)=", TriviaTopic.Math, TriviaDifficulty.Easy, "2", "3", "ezpz", "[4]", "4^2"),
            new TriviaQuestion("log(32)=", TriviaTopic.Math, TriviaDifficulty.Medium, "1.50514997832", "1.504", "2", "4", "128 planks"),
            new TriviaQuestion("In the game *Celeste*, how many strawberries do you have to collect in order to unlock the achievement **Impress Your Friends**?", TriviaTopic.Gaming, TriviaDifficulty.Easy, "175", "80", "181", "210", "174", "177", "176", "205")
        };

        public TriviaGame() : base()
        {
            Id = "Trivia";
            Details = new GameDetails
            {
                Name = "Trivia",
                Summary = "Answer questions against the clock!",
                PlayerLimit = 16,
                RequiredPlayers = 1
            };

            Config = new List<ConfigProperty>
            {
                ConfigProperty.Create<TriviaTopic>("topics", "Topics", TriviaTopic.Any),
                ConfigProperty.Create<TriviaDifficulty>("difficulty", "Difficulty", TriviaDifficulty.Any),
                ConfigProperty.Create<int>("questioncount", "Question Count", 5),
                ConfigProperty.Create<double>("questionduration", "Question Duration", 15)
            };
        }

        public override List<GameAction> OnBuildActions(List<PlayerData> players)
        {
            var actions = new List<GameAction>();

            var getQuestionResult = new GameAction
            {
                Id = "get_question_result",
                UpdateOnExecute = true,
                OnExecute = delegate (PlayerData player, GameSession session, GameServer server)
                {
                    // set all currently playing displays to freq. 10
                    foreach (ServerConnection connection in server.GetConnectionsInState(GameState.Playing))
                    {
                        connection.Frequency = 11;
                    }

                    //session.CancelQueuedAction();

                    int currentQuestion = session.GetPropertyValue<int>("current_question");
                    session.SetPropertyValue("players_answered", 0);

                    DisplayContent content = server.GetDisplayChannel(11).Content;

                    content
                        .GetComponent("result")
                        .Draw(session.GetPropertyValue<int>("current_question"),
                              GetConfigValue<int>("questioncount"),
                              CurrentQuestion.Question,
                              CurrentAnswers.Select((x, i) => x.IsCorrect ? $"[**{GetLetter(i).ToUpper()}**] {x.Response}" : null)
                                .First(x => !string.IsNullOrWhiteSpace(x)));

                    foreach (PlayerData playerData in session.Players)
                    {
                        bool hasAnswered = playerData.GetPropertyValue<bool>("has_answered");

                        if (!hasAnswered)
                        {
                            playerData.ResetProperty("streak");
                        }
                        else
                        {
                            bool isCorrect = playerData.GetPropertyValue<bool>("is_correct");


                            if (isCorrect)
                            {
                                int points = GetPoints(CurrentQuestion.Value,
                                    playerData.GetPropertyValue<int>("streak"),
                                    playerData.GetPropertyValue<int>("answer_position"),
                                    CurrentQuestion.Difficulty);

                                playerData.AddToProperty("score", points);
                            }
                        }

                        playerData.ResetProperty("has_answered");
                        playerData.ResetProperty("is_correct");
                    }

                    session.QueueAction(TimeSpan.FromSeconds(5), "try_get_next_question");
                }
            };
            // 'get_question_result'
            // This action is executed from 'get_next_question'

            // This shows the result of the question that they answered

            // A timer is set for 3 seconds
            // When the timer runs out, the action 'try_get_next_question' is executed

            var tryGetQuestionResult = new GameAction
            {
                Id = "try_get_question_result",
                UpdateOnExecute = false,
                OnExecute = delegate (PlayerData player, GameSession session, GameServer server)
                {
                    if (session.MeetsCriterion("has_all_players_answered"))
                    {
                        session.CancelQueuedAction();
                        session.InvokeAction("get_question_result", true);
                        
                    }
                }
            };

            var tryGetNextQuestion = new GameAction
            {
                Id = "try_get_next_question",
                UpdateOnExecute = false,
                OnExecute = delegate (PlayerData player, GameSession session, GameServer server)
                {
                    if (session.MeetsCriterion("has_answered_all_questions"))
                        session.InvokeAction("get_results", true);
                    else
                        session.InvokeAction("get_next_question", true);
                }
            };

            // 'try_get_next_question'
            // This action is executed right after 'get_question_result'
            // If the rule 'has_answered_all_questions' is true, the action 'get_results' is executed
            // Otherwise, if that rule is false, the action 'get_next_question' is executed instead

            var getNextQuestion = new GameAction
            {
                Id = "get_next_question",
                UpdateOnExecute = true,
                OnExecute = delegate (PlayerData player, GameSession session, GameServer server)
                {
                    // set all currently playing displays to freq. 10
                    foreach (ServerConnection connection in server.GetConnectionsInState(GameState.Playing))
                    {
                        connection.Frequency = 10;
                    }

                    int currentQuestion = session.GetPropertyValue<int>("current_question");
                    CurrentQuestion = QuestionPool[currentQuestion];
                    session.AddToProperty("current_question", 1);

                    DisplayContent content = server.GetDisplayChannel(10).Content;

                    content.GetComponent("question_header")
                        .Draw(
                            Orikivo.Format.Counter(GetConfigValue<double>("questionduration")),
                            session.GetPropertyValue<int>("current_question"),
                            GetConfigValue<int>("questioncount"),
                            CurrentQuestion.Question);

                    // select only 3 random answers and shuffle with the correct answer in there
                    CurrentAnswers = Randomizer.Shuffle(Randomizer
                            .ChooseMany(CurrentQuestion.Answers.Where(x => !x.IsCorrect), 3)
                            .Append(CurrentQuestion.Answers.First(x => x.IsCorrect)));

                    content
                        .GetComponent("answers")
                        .Draw(CurrentAnswers.Select((x, i) => $"[**{GetLetter(i).ToUpper()}**] {x.Response}"));

                    content
                        .GetComponent("footer")
                        .Draw(CurrentQuestion.Difficulty.ToString(),
                              CurrentQuestion.Topic.ToString(),
                              $"{CurrentQuestion.Value} {Orikivo.Format.TryPluralize("Point", CurrentQuestion.Value)}");

                    session.QueueAction(TimeSpan.FromSeconds(GetConfigValue<double>("questionduration")), "get_question_result");
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
                UpdateOnExecute = true,
                OnExecute = delegate (PlayerData player, GameSession session, GameServer server)
                {
                    // set all currently playing connection to frequency 12
                    foreach(ServerConnection connection in server.Connections.Where(x => x.State == GameState.Playing))
                    {
                        connection.Frequency = 12;
                    }

                    foreach(PlayerData data in session.Players)
                    {
                        Console.WriteLine($"{data.Player.User.Username}:\n{string.Join('\n', data.Properties.Select(x => $"{x.Id}: {x.Value.ToString()}"))}");
                    }

                    session._server
                    .GetDisplayChannel(12).Content
                    .GetComponent("leaderboard")
                     .Draw(session.Players
                        .OrderByDescending(x => x.GetPropertyValue<int>("score"))
                        .Select((x, i) => $"[**{i + 1}**{GetPositionSuffix(i + 1)}] **{x.Player.User.Username}**: **{x.GetPropertyValue<int>("score")}**p"));

                    session.QueueAction(TimeSpan.FromSeconds(15), "end");
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

            var tryRestart = new GameAction
            {
                Id = "try_restart",
                UpdateOnExecute = false,
                OnExecute = delegate (PlayerData player, GameSession session, GameServer server)
                {
                    if (session.MeetsCriterion("most_players_want_rematch"))
                    {
                        session.CancelQueuedAction();
                        // reset all player attributes
                        foreach (PlayerData data in session.Players)
                        {
                            foreach (GameProperty attribute in data.Properties)
                            {
                                attribute.Reset();
                            }
                        }

                        // reset all game attributes
                        foreach (GameProperty attribute in session.Properties)
                        {
                            attribute.Reset();
                        }

                        // regenerate question pool.
                        QuestionPool = GenerateQuestions(
                            GetConfigValue<int>("questioncount"),
                            GetConfigValue<TriviaDifficulty>("difficulty"),
                            GetConfigValue<TriviaTopic>("topics")).ToList();

                        session.InvokeAction("try_get_next_question", true);
                    }
                }
            };
            // 'restart'
            // This action resets all game and player attributes, and restarts the game as it was new
            // After everything is reset, the action 'try_get_next_question' is executed 

            actions.Add(getQuestionResult);
            actions.Add(tryGetQuestionResult);
            actions.Add(tryGetNextQuestion);
            actions.Add(getNextQuestion);
            actions.Add(getResults);
            actions.Add(tryRestart);


            return actions;
        }

        private string GetPositionSuffix(int position)
        {
            return position switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }

        private string GetLetter(int index)
        {
            return index switch
            {
                0 => "a",
                1 => "b",
                2 => "c",
                3 => "d",
                _ => $"{index}"
            };
        }
        
        
        public override List<GameCriterion> OnBuildRules(List<PlayerData> players)
        {
            var rules = new List<GameCriterion>();

            // 'has_all_players_answered'
            // This rule simply checks to see if: 'players_voted' == Players.Count
            var hasAllPlayersAnswered = new GameCriterion
            {
                Id = "has_all_players_answered",
                Criterion = delegate(GameSession session)
                {
                    return session.GetPropertyValue<int>("players_answered") == session.Players.Count;
                }
            };


            // 'most_players_want_rematch'
            // This rule checks to see if: 'rematch_requests' is >= (Players.Count / 2)
            var mostPlayersWantRematch = new GameCriterion
            {
                Id = "most_players_want_rematch",
                Criterion = delegate(GameSession session)
                {
                    return session.GetPropertyValue<int>("rematch_requests") >= (session.Players.Count / 2);
                }
            };

            var hasAnsweredAllQuestions = new GameCriterion
            {
                Id = "has_answered_all_questions",
                Criterion = delegate (GameSession session)
                {
                    return session.GetPropertyValue<int>("current_question") == GetConfigValue<int>("questioncount");
                }
            };

            rules.Add(hasAllPlayersAnswered);
            rules.Add(mostPlayersWantRematch);
            rules.Add(hasAnsweredAllQuestions);

            return rules;
        }

        public override List<GameProperty> OnBuildProperties()
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
        public override List<DisplayChannel> OnBuildDisplays(List<PlayerData> players)
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
                                BaseFormatter = "⏲️ {0}\n**Question {1}** (of {2})\n> {3}",
                                OverrideBaseValue = true
                            }
                        },
                        new ComponentGroup
                        {
                            Active = true,
                            Id = "answers",
                            Position = 1,
                            Formatter = new ComponentFormatter
                            {
                                BaseFormatter = "{0}\n",
                                ElementFormatter = "> {0}",
                                Separator = "\n",
                                OverrideBaseValue = true
                            }
                        },
                        new Component
                        {
                            Active = true,
                            Id = "footer",
                            Position = 2,
                            Formatter = new ComponentFormatter
                            {
                                BaseFormatter = "**Difficulty**: `{0}`\n**Topic**: `{1}`\n**Value**: `{2}`",
                                OverrideBaseValue = true
                            }
                        }
                    }
                },
                Inputs = new List<IInput>
                {
                    new TextInput
                    {
                        Name = "a",
                        UpdateOnExecute = false,
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            var session = server.Session;

                            var data = session.GetPlayerData(user.Id);

                            if (data.GetPropertyValue<bool>("has_answered"))
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(0);

                            data.SetPropertyValue("has_answered", true);
                            data.SetPropertyValue("is_correct", answerSelected.IsCorrect);

                            if (answerSelected.IsCorrect)
                            {
                                data.AddToProperty("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            session.AddToProperty("players_answered", 1);
                            data.SetPropertyValue("answer_position", server.Session.GetPropertyValue<int>("players_answered"));
                            session.InvokeAction("try_get_question_result");
                        }
                    },
                    new TextInput
                    {
                        Name = "b",
                        UpdateOnExecute = false,
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            var data = server.Session.GetPlayerData(user.Id);

                            if (data.GetPropertyValue<bool>("has_answered"))
                                return;

                            int answerCount = CurrentAnswers.Count();

                            if (answerCount < 2)
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(1);

                            data.SetPropertyValue("has_answered", true);
                            data.SetPropertyValue("is_correct", answerSelected.IsCorrect);

                            if (answerSelected.IsCorrect)
                            {
                                data.AddToProperty("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            server.Session.AddToProperty("players_answered", 1);
                            data.SetPropertyValue("answer_position", server.Session.GetPropertyValue<int>("players_answered"));
                            server.Session.InvokeAction("try_get_question_result");
                        }
                    },
                    new TextInput
                    {
                        Name = "c",
                        UpdateOnExecute = false,
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            var data = server.Session.GetPlayerData(user.Id);

                            if (data.GetPropertyValue<bool>("has_answered"))
                                return;

                            int answerCount = CurrentAnswers.Count();

                            if (answerCount < 3)
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(2);

                            data.SetPropertyValue("has_answered", true);
                            data.SetPropertyValue("is_correct", answerSelected.IsCorrect);

                            if (answerSelected.IsCorrect)
                            {
                                data.AddToProperty("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            server.Session.AddToProperty("players_answered", 1);
                            data.SetPropertyValue("answer_position", server.Session.GetPropertyValue<int>("players_answered"));
                            server.Session.InvokeAction("try_get_question_result");
                        }
                    },
                    new TextInput
                    {
                        Name = "d",
                        UpdateOnExecute = false,
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            var data = server.Session.GetPlayerData(user.Id);

                            if (data.GetPropertyValue<bool>("has_answered"))
                                return;

                            int answerCount = CurrentAnswers.Count();

                            if (answerCount < 4)
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(3);

                            data.SetPropertyValue("has_answered", true);
                            data.SetPropertyValue("is_correct", answerSelected.IsCorrect);
                            
                            if (answerSelected.IsCorrect)
                            {
                                data.AddToProperty("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            server.Session.AddToProperty("players_answered", 1);
                            data.SetPropertyValue("answer_position", server.Session.GetPropertyValue<int>("players_answered"));
                            server.Session.InvokeAction("try_get_question_result");
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
                            Id = "result",
                            Position = 0,
                            Formatter = new ComponentFormatter
                            {
                                // 0: currentQuestion
                                // 1: questionCount
                                // 2: questionValue
                                // 3: correctResponse
                                BaseFormatter = "**Question {0}** (of {1})\n> {2}\n\n> **Correct Response**:\n> {3}",
                                OverrideBaseValue = true
                            }
                        }
                    }
                },
                Inputs = new List<IInput>
                {
                    new TextInput
                    {
                        Name = "next",
                        UpdateOnExecute = false,
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            server.Session.CancelQueuedAction();
                            server.Session.InvokeAction("try_get_next_question", true);
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
                               OverrideBaseValue = true
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
                        UpdateOnExecute = false,
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            server.Session.AddToProperty("rematch_requests", 1);
                            server.Session.InvokeAction("try_restart", true);
                        }
                    },
                    new TextInput
                    {
                        Name = "end",
                        UpdateOnExecute = false,
                        OnExecute = delegate(IUser user, ServerConnection connection, GameServer server)
                        {
                            server.Session.CancelQueuedAction();
                            server.Session.InvokeAction("end", true);
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

        public override List<PlayerData> OnBuildPlayers(List<Player> players)
        {
            var sessionData = players.Select(x =>
                new PlayerData
                {
                    Player = x,
                    Properties = new List<GameProperty>
                    {
                        GameProperty.Create<int>("score", 0),
                        GameProperty.Create<int>("streak", 0),
                        // if the answer they selected was correct, set to true; otherwise false
                        GameProperty.Create<bool>("is_correct", false),
                        GameProperty.Create<bool>("has_answered", false),
                        GameProperty.Create<int>("answer_position", 0) // this determines how many points they get if they were right
                        // bonuses are given to the fastest people
                    }
                });

            return sessionData.ToList();
        }

        // value being the value of the question specified
        internal int GetPoints(int value, int streak, int answerPosition, TriviaDifficulty difficulty) // the further away your position is, the less points you receive
        {
            // streaks can be set up by 5, 10, 15, 20, 25, 30, 35, etc.
            int streakValue = streak <= 0 ? 0 : (streak - 1) * 5; // a one question delay is added to prevent streak bonuses on the first question.
            float multiplier = GetDifficultyMultiplier(difficulty);
            
            // this means they never answered
            if (answerPosition == 0)
                return 0;

            return (int)Math.Floor(((value * (1.0 - ((answerPosition - 1) / 10))) * multiplier) + streakValue);
        }

        private float GetDifficultyMultiplier(TriviaDifficulty difficulty)
        {
            return difficulty switch
            {
                TriviaDifficulty.Easy => 1.0F,
                TriviaDifficulty.Medium => 1.5F,
                TriviaDifficulty.Hard => 2.0F,
                _ => throw new Exception("Cannot determine point multiplier for the specified difficulty")
            };
        }

        public override void OnSessionStart(GameServer server, GameSession session)
        {
            Config = server.Config.GameConfig;

            // generate the question pool
            QuestionPool = GenerateQuestions(
                GetConfigValue<int>("questioncount"),
                GetConfigValue<TriviaDifficulty>("difficulty"),
                GetConfigValue<TriviaTopic>("topics")
                ).ToList();

            // once all of that is ready, invoke action try_get_next_questions
            session.InvokeAction("try_get_next_question", true);
        }

        public override SessionResult OnSessionFinish(GameSession session)
        {
            var result = new SessionResult();
            // because we don't have access to stats directly, we have to use stat update packets
            // NOTE: unless the stat allows it, you CANNOT update existing stats outside of the ones specified.
            foreach (PlayerData player in session.Players)
            {
                Console.WriteLine("IS_THIS_BEING_USED");
                ulong playerId = player.Player.User.Id;
                var stats = new List<StatUpdatePacket>();
                
                stats.Add(new StatUpdatePacket(TriviaStats.TimesPlayed, 1));
                stats.Add(new StatUpdatePacket(TriviaStats.HighestScore, GetScore(session, playerId), StatUpdateType.SetIfGreater));

                if (GetWinningPlayerId(session) == playerId)
                {
                    stats.Add(new StatUpdatePacket(TriviaStats.TimesWon, 1));
                    stats.Add(new StatUpdatePacket(TriviaStats.CurrentWinStreak, 1));
                    stats.Add(new StatUpdatePacket(TriviaStats.LongestWin, TriviaStats.CurrentWinStreak, StatUpdateType.SetIfGreater));
                }
                else
                {
                    stats.Add(new StatUpdatePacket(TriviaStats.CurrentWinStreak, 0, StatUpdateType.Set));
                }

                result.UserIds.Add(playerId);
                result.Stats.Add(playerId, stats);
            }

            return result;
        }

        private ulong GetWinningPlayerId(GameSession session)
        {
            return session.Players.OrderByDescending(x => x.GetPropertyValue("score")).First().Player.User.Id;
        }

        private int GetScore(GameSession session, ulong playerId)
        {
            return session.GetPlayerData(playerId).GetPropertyValue<int>("score");
        }

        internal IEnumerable<TriviaQuestion> GenerateQuestions(int questionCount, TriviaDifficulty difficultyRange, TriviaTopic topic)
        {
            var questions = new List<TriviaQuestion>();
            var availableQuestions = FilterQuestions(difficultyRange, topic);

            return Randomizer.ChooseMany(availableQuestions, questionCount);
        }

        private IEnumerable<TriviaQuestion> FilterQuestions(TriviaDifficulty difficulty, TriviaTopic topic)
        {
            return Questions.Where(x => difficulty.HasFlag(x.Difficulty) && topic.HasFlag(x.Topic));
        }

        public TriviaQuestion CurrentQuestion { get; set; }

        public List<TriviaQuestion> QuestionPool { get; set; }

        public IEnumerable<TriviaAnswer> CurrentAnswers { get; set; }
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
