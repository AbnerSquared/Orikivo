using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer.Games
{
    internal static class TriviaVars
    {
        internal static readonly string TotalAnswered = "total_answered";
        internal static readonly string GetQuestionResult = "get_question_result";
        internal static readonly string TryGetQuestionResult = "try_get_question_result";
        internal static readonly string TryGetNextQuestion = "try_get_next_question";
        internal static readonly string GetNextQuestion = "get_next_question";
        internal static readonly string GetResults = "get_results";
        internal static readonly string TryRestart = "try_restart";
    }

    internal static class TriviaConfig
    {
        internal static readonly string Topics = "topics";
        internal static readonly string Difficulty = "difficulty";
        internal static readonly string QuestionCount = "questioncount";
        internal static readonly string QuestionDuration = "questionduration";
    }

    public class TriviaGame : GameBase
    {
        public static List<TriviaQuestion> Questions => new List<TriviaQuestion>
        {
            new TriviaQuestion("2+2=", TriviaTopic.Math, TriviaDifficulty.Easy, "4", "3", "fish", "dude i can't math why are you doing this", "answer"),
            new TriviaQuestion("4*2=", TriviaTopic.Math, TriviaDifficulty.Easy, "8", "42", "6", "2", "2.000001", "Int32.MinValue"),
            new TriviaQuestion("sqrt(4)=", TriviaTopic.Math, TriviaDifficulty.Easy, "2", "3", "ezpz", "[4]", "4^2"),
            new TriviaQuestion("log(32)=", TriviaTopic.Math, TriviaDifficulty.Medium, "1.50514997832", "1.504", "2", "4", "128 planks"),
            new TriviaQuestion("In the game *Celeste*, how many strawberries do you have to collect in order to unlock the achievement **Impress Your Friends**?", TriviaTopic.Gaming, TriviaDifficulty.Easy, "175", "80", "181", "210", "174", "177", "176", "205")
        };

        public TriviaGame()
        {
            Id = "Trivia";
            Details = new GameDetails
            {
                Name = "Trivia",
                Summary = "Answer questions against the clock!",
                PlayerLimit = 16,
                RequiredPlayers = 1
            };

            Options = new List<GameOption>
            {
                GameOption.Create("topics", "Topics", TriviaTopic.Any),
                GameOption.Create("difficulty", "Difficulty", TriviaDifficulty.Any),
                GameOption.Create("questioncount", "Question Count", 5),
                GameOption.Create("questionduration", "Question Duration", 15d)
            };
        }


        // 'get_question_result'
        // This action is executed from 'get_next_question'

        // This shows the result of the question that they answered

        // A timer is set for 3 seconds
        // When the timer runs out, the action 'try_get_next_question' is executed
        private void GetQuestionResult(GameContext ctx)
        {
            // set all currently playing displays to freq. 10
            foreach (ServerConnection connection in ctx.Server.GetConnectionsInState(GameState.Playing))
            {
                connection.Frequency = 11;
            }

            //session.CancelQueuedAction();

            int currentQuestion = ctx.Session.ValueOf<int>("current_question");
            ctx.Session.SetValue("players_answered", 0);

            DisplayContent content = ctx.Server.GetBroadcast(11).Content;

            content
                .GetComponent("result")
                .Draw(ctx.Session.ValueOf<int>("current_question"),
                    ctx.Session.GetConfigValue<int>("questioncount"),
                      CurrentQuestion.Question,
                      CurrentAnswers.Select((x, i) => x.IsCorrect ? $"[**{GetLetter(i).ToUpper()}**] {x.Response}" : null)
                        .First(x => !string.IsNullOrWhiteSpace(x)));

            foreach (PlayerData playerData in ctx.Session.Players)
            {
                bool hasAnswered = playerData.ValueOf<bool>("has_answered");

                if (!hasAnswered)
                {
                    playerData.ResetProperty("streak");
                }
                else
                {
                    bool isCorrect = playerData.ValueOf<bool>("is_correct");


                    if (isCorrect)
                    {
                        int points = GetPoints(CurrentQuestion.Value,
                            playerData.ValueOf<int>("streak"),
                            playerData.ValueOf<int>("answer_position"),
                            CurrentQuestion.Difficulty);

                        playerData.AddToValue("score", points);
                    }
                }

                playerData.ResetProperty("has_answered");
                playerData.ResetProperty("is_correct");
            }

            ctx.Session.QueueAction(TimeSpan.FromSeconds(5), "try_get_next_question");
        }

        private static void TryGetQuestionResult(GameContext ctx)
        {
            if (ctx.Session.MeetsCriterion("has_all_players_answered"))
            {
                ctx.Session.CancelNewestInQueue();
                ctx.Session.InvokeAction(TriviaVars.GetQuestionResult, true);
            }
        }

        private static void TryGetNextQuestion(GameContext ctx)
        {
            if (ctx.Session.MeetsCriterion("has_answered_all_questions"))
                ctx.Session.InvokeAction(TriviaVars.GetResults, true);
            else
                ctx.Session.InvokeAction(TriviaVars.GetNextQuestion, true);
        }

        private void GetNextQuestion(GameContext ctx)
        {
            // set all currently playing displays to freq. 10
            foreach (ServerConnection connection in ctx.Server.GetConnectionsInState(GameState.Playing))
            {
                connection.Frequency = 10;
            }

            int currentQuestion = ctx.Session.ValueOf<int>("current_question");
            CurrentQuestion = QuestionPool[currentQuestion];
            ctx.Session.AddToValue("current_question", 1);

            DisplayContent content = ctx.Server.GetBroadcast(10).Content;

            content.GetComponent("question_header")
                .Draw(
                    Format.Counter(ctx.Session.GetConfigValue<double>("questionduration")),
                    ctx.Session.ValueOf<int>("current_question"),
                    ctx.Session.GetConfigValue<int>("questioncount"),
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
                    $"{CurrentQuestion.Value} {Format.TryPluralize("Point", CurrentQuestion.Value)}");

            ctx.Session.QueueAction(TimeSpan.FromSeconds(ctx.Session.GetConfigValue<double>("questionduration")), "get_question_result");
        }


        private void GetResults(GameContext ctx)
        {
            // set all currently playing connection to frequency 12
            foreach (ServerConnection connection in ctx.Server.Connections.Where(x => x.State == GameState.Playing))
            {
                connection.Frequency = 12;
            }

            foreach (PlayerData data in ctx.Session.Players)
            {
                Console.WriteLine($"{data.Source.User.Username}:\n{string.Join('\n', data.Properties.Select(x => $"{x.Id}: {x.Value.ToString()}"))}");
            }

            ctx.Session.Server
                .GetBroadcast(12).Content
                .GetComponent("leaderboard")
                .Draw(ctx.Session.Players
                    .OrderByDescending(x => x.ValueOf<int>("score"))
                    .Select((x, i) => $"[**{i + 1}**{GetPositionSuffix(i + 1)}] **{x.Source.User.Username}**: **{x.ValueOf<int>("score")}**p"));

            ctx.Session.QueueAction(TimeSpan.FromSeconds(15), "end");
        }

        private void TryRestart(GameContext ctx)
        {
            if (ctx.Session.MeetsCriterion("most_players_want_rematch"))
            {
                ctx.Session.CancelNewestInQueue();
                // reset all player attributes
                foreach (PlayerData data in ctx.Session.Players)
                {
                    foreach (GameProperty attribute in data.Properties)
                    {
                        attribute.Reset();
                    }
                }

                // reset all game attributes
                foreach (GameProperty attribute in ctx.Session.Properties)
                {
                    attribute.Reset();
                }

                // regenerate question pool.
                QuestionPool = GenerateQuestions(
                    ctx.Session.GetConfigValue<int>("questioncount"),
                    ctx.Session.GetConfigValue<TriviaDifficulty>("difficulty"),
                    ctx.Session.GetConfigValue<TriviaTopic>("topics")).ToList();

                ctx.Session.InvokeAction("try_get_next_question", true);
            }
        }

        public override List<GameAction> OnBuildActions(List<PlayerData> players)
        {
            return new List<GameAction>
            {
                new GameAction(TriviaVars.GetQuestionResult, GetQuestionResult),
                new GameAction("try_get_question_result", TryGetQuestionResult, false),
                new GameAction("try_get_next_question", TryGetNextQuestion, false),
                new GameAction("get_next_question", GetNextQuestion),
                new GameAction("get_results", GetResults),
                new GameAction("try_restart", TryRestart, false)
            };
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

        private static bool HasAllPlayersAnswered(GameSession session)
            => session.ValueOf<int>(TriviaVars.TotalAnswered) == session.Players.Count;

        public override List<GameCriterion> OnBuildRules(List<PlayerData> players)
        {
            var rules = new List<GameCriterion>();

            // 'has_all_players_answered'
            // This rule simply checks to see if: 'players_voted' == Players.Count
            var hasAllPlayersAnswered = new GameCriterion("has_all_players_answered", HasAllPlayersAnswered)
            {
                Id = "has_all_players_answered",
                Criterion = delegate(GameSession session)
                {
                    return session.ValueOf<int>("players_answered") == session.Players.Count;
                }
            };


            // 'most_players_want_rematch'
            // This rule checks to see if: 'rematch_requests' is >= (Players.Count / 2)
            var mostPlayersWantRematch = new GameCriterion
            {
                Id = "most_players_want_rematch",
                Criterion = delegate(GameSession session)
                {
                    return session.ValueOf<int>("rematch_requests") >= (session.Players.Count / 2);
                }
            };

            var hasAnsweredAllQuestions = new GameCriterion
            {
                Id = "has_answered_all_questions",
                Criterion = delegate (GameSession session)
                {
                    return session.ValueOf<int>("current_question") == session.GetConfigValue<int>("questioncount");
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
        public override List<DisplayBroadcast> OnBuildBroadcasts(List<PlayerData> players)
        {
            var displays = new List<DisplayBroadcast>();
            // frequency 10 (question)
            // This is what displays all of the questions and allows for answering
            // The action 'show_question_result' is executed when:
            // - All current players chose an answer
            // - The timer of 30 seconds since the question started runs out

            var question = new DisplayBroadcast(10)
            {
                Content = new DisplayContent
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
                },
                Inputs = new List<IInput>
                {
                    new TextInput
                    {
                        Name = "a",
                        UpdateOnExecute = false,
                        OnExecute = delegate(InputContext ctx)
                        {

                            var data = ctx.Session.DataOf(ctx.Invoker.Id);

                            if (data.ValueOf<bool>("has_answered"))
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(0);

                            data.SetValue("has_answered", true);
                            data.SetValue("is_correct", answerSelected.IsCorrect);

                            if (answerSelected.IsCorrect)
                            {
                                data.AddToValue("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            ctx.Session.AddToValue("players_answered", 1);
                            data.SetValue("answer_position", ctx.Session.ValueOf<int>("players_answered"));
                            ctx.Session.InvokeAction("try_get_question_result");
                        }
                    },
                    new TextInput
                    {
                        Name = "b",
                        UpdateOnExecute = false,
                        OnExecute = delegate(InputContext ctx)
                        {
                            var data = ctx.Session.DataOf(ctx.Invoker.Id);

                            if (data.ValueOf<bool>("has_answered"))
                                return;

                            int answerCount = CurrentAnswers.Count();

                            if (answerCount < 2)
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(1);

                            data.SetValue("has_answered", true);
                            data.SetValue("is_correct", answerSelected.IsCorrect);

                            if (answerSelected.IsCorrect)
                            {
                                data.AddToValue("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            ctx.Session.AddToValue("players_answered", 1);
                            data.SetValue("answer_position", ctx.Session.ValueOf<int>("players_answered"));
                            ctx.Session.InvokeAction("try_get_question_result");
                        }
                    },
                    new TextInput
                    {
                        Name = "c",
                        UpdateOnExecute = false,
                        OnExecute = delegate(InputContext ctx)
                        {
                            var data = ctx.Session.DataOf(ctx.Invoker.Id);

                            if (data.ValueOf<bool>("has_answered"))
                                return;

                            int answerCount = CurrentAnswers.Count();

                            if (answerCount < 3)
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(2);

                            data.SetValue("has_answered", true);
                            data.SetValue("is_correct", answerSelected.IsCorrect);

                            if (answerSelected.IsCorrect)
                            {
                                data.AddToValue("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            ctx.Session.AddToValue("players_answered", 1);
                            data.SetValue("answer_position", ctx.Server.Session.ValueOf<int>("players_answered"));
                            ctx.Session.InvokeAction("try_get_question_result");
                        }
                    },
                    new TextInput
                    {
                        Name = "d",
                        UpdateOnExecute = false,
                        OnExecute = delegate(InputContext ctx)
                        {
                            var data = ctx.Server.Session.DataOf(ctx.Invoker.Id);

                            if (data.ValueOf<bool>("has_answered"))
                                return;

                            int answerCount = CurrentAnswers.Count();

                            if (answerCount < 4)
                                return;

                            var answerSelected = CurrentAnswers.ElementAt(3);

                            data.SetValue("has_answered", true);
                            data.SetValue("is_correct", answerSelected.IsCorrect);
                            
                            if (answerSelected.IsCorrect)
                            {
                                data.AddToValue("streak", 1);
                            }
                            else
                            {
                                data.ResetProperty("streak");
                            }

                            ctx.Server.Session.AddToValue("players_answered", 1);
                            data.SetValue("answer_position", ctx.Server.Session.ValueOf<int>("players_answered"));
                            ctx.Server.Session.InvokeAction("try_get_question_result");
                        }
                    }
                }
            };

            // Display 2: Question Result (Frequency 11)
            // This is what shows what the actual answer was
            // There is a basic timer of 3 seconds before executing the action 'try_get_next_question'
            var questionResult = new DisplayBroadcast(11)
            {
                Content = new DisplayContent
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
                },
                Inputs = new List<IInput>
                {
                    new TextInput
                    {
                        Name = "next",
                        UpdateOnExecute = false,
                        OnExecute = delegate(InputContext ctx)
                        {
                            ctx.Session.CancelNewestInQueue();
                            ctx.Session.InvokeAction("try_get_next_question", true);
                        }
                    }
                }
            };


            var results = new DisplayBroadcast(12)
            {
                Content = new DisplayContent
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
                },
                Inputs = new List<IInput>
                {
                    new TextInput
                    {
                        Name = "rematch",
                        UpdateOnExecute = false,
                        OnExecute = delegate(InputContext ctx)
                        {
                            ctx.Session.AddToValue("rematch_requests", 1);
                            ctx.Session.InvokeAction("try_restart", true);
                        }
                    },
                    new TextInput
                    {
                        Name = "end",
                        UpdateOnExecute = false,
                        OnExecute = delegate(InputContext ctx)
                        {
                            ctx.Session.CancelNewestInQueue();
                            ctx.Session.InvokeAction("end", true);
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

        public override List<PlayerData> OnBuildPlayers(in IEnumerable<Player> players)
        {
            var sessionData = players.Select(x =>
                new PlayerData
                {
                    Source = x,
                    Properties = new List<GameProperty>
                    {
                        GameProperty.Create("score", 0),
                        GameProperty.Create("streak", 0),
                        // if the answer they selected was correct, set to true; otherwise false
                        GameProperty.Create("is_correct", false),
                        GameProperty.Create("has_answered", false),
                        GameProperty.Create("answer_position", 0) // this determines how many points they get if they were right
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

            return (int)Math.Floor(((value * (1.0 - ((answerPosition - 1) / (double) 10))) * multiplier) + streakValue);
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

        public override async Task OnSessionStartAsync(GameServer server, GameSession session)
        {
            Options = server.Config.GameOptions;

            // generate the question pool
            QuestionPool = GenerateQuestions(
                session.GetConfigValue<int>(TriviaConfig.QuestionCount),
                session.GetConfigValue<TriviaDifficulty>(TriviaConfig.Difficulty),
                session.GetConfigValue<TriviaTopic>(TriviaConfig.Topics)
                ).ToList();

            // once all of that is ready, invoke action try_get_next_questions
            session.InvokeAction(TriviaVars.TryGetNextQuestion, true);
        }

        public override SessionResult OnSessionFinish(GameSession session)
        {
            var result = new SessionResult();
            // because we don't have access to stats directly, we have to use stat update packets
            // NOTE: unless the stat allows it, you CANNOT update existing stats outside of the ones specified.
            foreach (PlayerData player in session.Players)
            {
                ulong playerId = player.Source.User.Id;
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
            return session.Players.OrderByDescending(x => x.ValueOf("score")).First().Source.User.Id;
        }

        private int GetScore(GameSession session, ulong playerId)
        {
            return session.DataOf(playerId).ValueOf<int>("score");
        }

        internal IEnumerable<TriviaQuestion> GenerateQuestions(int questionCount, TriviaDifficulty difficultyRange, TriviaTopic topic)
        {
            var questions = new List<TriviaQuestion>();
            var availableQuestions = FilterQuestions(difficultyRange, topic);

            return Randomizer.ChooseMany(availableQuestions, questionCount);
        }

        private static IEnumerable<TriviaQuestion> FilterQuestions(TriviaDifficulty difficulty, TriviaTopic topic)
        {
            return Questions.Where(x => difficulty.HasFlag(x.Difficulty) && topic.HasFlag(x.Topic));
        }

        public TriviaQuestion CurrentQuestion { get; set; }

        public List<TriviaQuestion> QuestionPool { get; set; }

        public IEnumerable<TriviaAnswer> CurrentAnswers { get; set; }
    }
}
