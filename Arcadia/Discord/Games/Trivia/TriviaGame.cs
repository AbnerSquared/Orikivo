﻿using Orikivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTDB;
using Orikivo.Framework;
using Format = Orikivo.Format;

namespace Arcadia.Multiplayer.Games
{
    public class TriviaGame : GameBase
    {
        public static readonly List<TriviaQuestion> Questions = new List<TriviaQuestion>
        {
            /*
             new TriviaQuestion("",
                "", TriviaDifficulty.Medium, "",
                "", "", "", ""),
             */

            new TriviaQuestion("`2 + 2` = ?", 4,
                "Mathematics: Addition", TriviaDifficulty.Easy, "4",
                "3", "fish", "22", "dude i can't math why are you doing this", "answer"),

            new TriviaQuestion("`4 * 2` = ?", 8,
                "Mathematics: Multiplication", TriviaDifficulty.Easy, "`8`",
                "`42`", "`6`", "`2`", "`2.000001`", "`Int32.MinValue`"),

            new TriviaQuestion("`sqrt(4)` = ?",
                "Mathematics: Roots", TriviaDifficulty.Easy, "2",
                "3", "ezpz", "[4]", "4^2"),

            new TriviaQuestion("`log(32)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "1.5051",
                "1.5043", "2.0243", "4.0001", "128 planks"),

            new TriviaQuestion("What is the inverse function to `log_b(x)`?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "`x = b^y`",
                "`ln(b)`", "`ln(e)`", "`Undefined`", "`log_x(b)`"),

            new TriviaQuestion("`log_b(x^y)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "`y * log_b(x)`",
                "`1/log_x(y)`", "`e^(x^y)`", "`b^(x+y)`", "`log_x(y)`"),

            new TriviaQuestion("`log_b(0)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Easy, "`Undefined`",
                "`ln(1)`", "`e`", "`b`", "`Does Not Exist`"),

            new TriviaQuestion("`log_b(x * y)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "`log_b(x) + log_b(y)`",
                "`log(ln(b))`", "`log(x) + log(y)`", "`log_{x / y}(b)`"),

            new TriviaQuestion("`log_b(x / y)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "`log_b(x) - log_b(y)`",
                "`log_{x - y}(b)`", "`log(x) + log(y)`", "`log(x) - log(y)`"),

            new TriviaQuestion("`log_b(b)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "`1`",
                "`b * log(1)`", "`0`", "`Undefined`", "`?`"),

            new TriviaQuestion("`log_b(1)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "`0`",
                "`Number`", "`log(b)`", "`ln(e)`", "`e`"),

            new TriviaQuestion("`log_e(x)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Easy, "`ln(x)`",
                "`0`", "`1`", "`Undefined`", "`ln(1)`", "`log_b(x * e)`"),

            new TriviaQuestion("What is the derivative of `f(x) = log_b(x)`?",
                "Mathematics: Derivatives", TriviaDifficulty.Medium, "`f'(x) = 1 / (x * ln(b))`",
                "`f'(x) = x * log(b)`", "`f'(x) = Undefined`", "`f'(x) = x * (log_b(x)`", "`f'(x) = 1`"),

            new TriviaQuestion("What is the symbolic representation of Euler's number?",
                "Mathematics: Constants", TriviaDifficulty.Easy, "e",
                "π", "K", "E", "ln"),

            new TriviaQuestion("What is the approximate value of Euler's number?",
                "Mathematics: Constants", TriviaDifficulty.Medium, "`2.7182`",
                "`2.7192`", "`3.1415`", "`2.7281`", "`2.8172`"),

            new TriviaQuestion("What is the literal value of Euler's number?",
                "Mathematics: Constants", TriviaDifficulty.Hard, "`lim_{x => ∞}(1 + (1 / x))^x`",
                "`2.7182`", "`3.1415`", "`lim_{x => -∞}(1 - x)^x`", "`lim_{x => ∞}(1 - (1 / x))^x`"),

            new TriviaQuestion("`log(x) / log(b)` = ?",
                "Mathematics: Logarithms", TriviaDifficulty.Medium, "`log_b(x)`",
                "`log(x/b)`", "`log_x(b)`", "`log(b) - log(x)`", "`log(log(log(b) + x))`"),

            new TriviaQuestion("In the game *Celeste*, how many strawberries do you have to collect in order to unlock the achievement **Impress Your Friends**?",
                TriviaTopic.Gaming, TriviaDifficulty.Easy, "175",
                "80", "181", "210", "174", "177", "176", "205")
        };

        /// <inheritdoc />
        public override string Id => "trivia";

        /// <inheritdoc />
        public override GameDetails Details => new GameDetails
        {
            Name = "Trivia",
            Icon = "📰",
            Summary = "Answer questions against the clock!",
            PlayerLimit = 16,
            RequiredPlayers = 1,
            CanSpectate = true
        };

        // TODO: Merge GameOption collection into GameDetails
        public override List<GameOption> Options => new List<GameOption>
        {
            GameOption.Create("topics", "Topics", TriviaTopic.Any, "Determines the topics to filter for (only when **Use OpenTDB** is disabled)."),
            GameOption.Create("difficulty", "Difficulty", TriviaDifficulty.Any, "Determines the difficulty range to filter each question for."),
            GameOption.Create("questioncount", "Question Count", 5, "Represents the total amount of questions to play in this session."),
            GameOption.Create("questionduration", "Question Duration", 15d, "Determines how long a player has to answer each question."),
            GameOption.Create(TriviaConfig.UseOpenTDB, "Use OpenTDB", true, "This toggles the usage of the OpenTDB API, which allows for a wider range of unique questions. Disable if you wish to only answer custom-made questions.")
        };

        [Action("get_question_result")]
        public void GetQuestionResult(GameContext ctx)
        {
            // set all currently playing displays to freq. 10
            foreach (ServerConnection connection in ctx.Server.GetConnectionsInState(GameState.Playing))
            {
                connection.Frequency = 11;
            }
            ctx.Session.SpectateFrequency = 11;

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

        [Action("try_get_question_result", false)]
        public void TryGetQuestionResult(GameContext ctx)
        {
            if (ctx.Session.MeetsCriterion("has_all_players_answered"))
            {
                ctx.Session.CancelNewestInQueue();
                ctx.Session.InvokeAction(TriviaVars.GetQuestionResult, true);
            }
        }

        [Action("try_get_next_question", false)]
        public void TryGetNextQuestion(GameContext ctx)
        {
            if (ctx.Session.MeetsCriterion("has_answered_all_questions"))
                ctx.Session.InvokeAction(TriviaVars.GetResults, true);
            else
                ctx.Session.InvokeAction(TriviaVars.GetNextQuestion, true);
        }

        [Action("get_next_question")]
        public void GetNextQuestion(GameContext ctx)
        {
            // set all currently playing displays to freq. 10
            foreach (ServerConnection connection in ctx.Server.GetConnectionsInState(GameState.Playing))
            {
                connection.Frequency = 10;
            }
            ctx.Session.SpectateFrequency = 10;

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
                .ChooseMany(CurrentQuestion.Answers.Where(x => !x.IsCorrect), Math.Min(3, CurrentQuestion.Answers.Count(x => !x.IsCorrect)))
                .Append(CurrentQuestion.Answers.First(x => x.IsCorrect)));

            content
                .GetComponent("answers")
                .Draw(CurrentAnswers.Select((x, i) => $"[**{GetLetter(i).ToUpper()}**] {x.Response}"));

            content
                .GetComponent("footer")
                .Draw(CurrentQuestion.Difficulty.ToString(),
                    string.IsNullOrWhiteSpace(CurrentQuestion.TopicOverride) ? CurrentQuestion.Topic.ToString() : CurrentQuestion.TopicOverride,
                    $"{CurrentQuestion.Value} {Format.TryPluralize("Point", CurrentQuestion.Value)}");

            ctx.Session.QueueAction(TimeSpan.FromSeconds(ctx.Session.GetConfigValue<double>("questionduration")), "get_question_result");
        }

        [Action("get_results")]
        private void GetResults(GameContext ctx)
        {
            // set all currently playing connection to frequency 12
            foreach (ServerConnection connection in ctx.Server.Connections.Where(x => x.State == GameState.Playing))
            {
                connection.Frequency = 12;
            }
            ctx.Session.SpectateFrequency = 12;

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

        [Action("try_restart", false)]
        public void TryRestart(GameContext ctx)
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

        public override List<GameAction> OnBuildActions()
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

        private static bool MostPlayersWantRematch(GameSession session)
            => session.ValueOf<int>("rematch_requests") >= (session.Players.Count / 2);

        private static bool HasAnsweredAllQuestions(GameSession session)
            => session.ValueOf<int>("current_question") == session.GetConfigValue<int>("questioncount");

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

        [Property("rematch_vote_count")]
        public int RematchVoteCount { get; set; } = 0;

        [Property("answer_count")]
        public int AnswerCount { get; set; } = 0;

        [Property("question_index")]
        public int QuestionIndex { get; set; } = 0;

        public override List<GameProperty> OnBuildProperties()
        {
            var attributes = new List<GameProperty>();

            // 'rematch_requests'
            // This keeps track of all players wanting to rematch
            var rematchRequests = GameProperty.Create<int>("rematch_requests", 0);

            // This attribute is set from the freqency 12:rematch input
            // This attribute is reset from 'restart'
            var playersAnswered = GameProperty.Create<int>("players_answered", 0);

            // This attribute is set from frequency 10:a,b,c, or d input
            // This attribute is reset from 'get_question_result'
            var currentQuestion = GameProperty.Create<int>("current_question", 0);

            attributes.Add(rematchRequests);
            attributes.Add(playersAnswered);
            attributes.Add(currentQuestion);

            return attributes;
        }

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
                    /*
                    new ReactionInput
                    {
                        Emote = new Emoji("🇦"),
                        Handling = ReactionHandling.Any,
                        UpdateOnExecute = true,
                        RequireOnMessage = true
                    },
                    new ReactionInput
                    {
                        Emote = new Emoji("🇧"),
                        Handling = ReactionHandling.Any,
                        UpdateOnExecute = true,
                        RequireOnMessage = true
                    },
                    new ReactionInput
                    {
                        Emote = new Emoji("🇨"),
                        Handling = ReactionHandling.Any,
                        UpdateOnExecute = true,
                        RequireOnMessage = true
                    },
                    new ReactionInput
                    {
                        Emote = new Emoji("🇩"),
                        Handling = ReactionHandling.Any,
                        UpdateOnExecute = true,
                        RequireOnMessage = true
                    }, 

                     */
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
                                data.AddToValue(TriviaVars.TotalCorrect, 1);
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
                                data.AddToValue(TriviaVars.TotalCorrect, 1);
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
                                data.AddToValue(TriviaVars.TotalCorrect, 1);
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
                                data.AddToValue(TriviaVars.TotalCorrect, 1);
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
                        // This is the sum of correct responses they've given
                        GameProperty.Create("total_correct", 0),
                        // if the answer they selected was correct, set to true; otherwise false
                        GameProperty.Create("is_correct", false),
                        GameProperty.Create("has_answered", false),
                        GameProperty.Create("answer_position", 0) // this determines how many points they get if they were right
                        // bonuses are given to the fastest people
                    }
                });

            return sessionData.ToList();
        }

        private const int STREAK_DELAY = 1;
        private const int STREAK_BONUS = 2;
        private const float BASE_POINT_SCALE = 1.0f;
        private const float EASY_POINT_SCALE = 1.0f;
        private const float MED_POINT_SCALE = 1.5f;
        private const float HARD_POINT_SCALE = 2.0f;
        private const int POSITION_CUTOFF = 12;

        // TODO: Make this calculation easier to modify by leaving constants outside
        /*
            Streak Delay: 1 = The amount of questions that are required before a streak bonus is given
            Streak Bonus Multiplier: 2 = The amount of points that are summed onto the previous streak value on each consecutive correct question
            Base Point Scale: 1.0 = The base point worth before being calculated
            Easy Multiplier Scale: 1.0 = The multiplier applied on easy questions
            Medium Multiplier Scale : 1.5 = The multiplier applied on medium questions
            Hard Multiplier Scale: 2.0 = The multiplier applied on hard questions
            Answer Position Cut-off: 10 = The position to answer a question before no points are rewarded
             
             */
        // value being the value of the question specified
        internal int GetPoints(int value, int streak, int answerPosition, TriviaDifficulty difficulty) // the further away your position is, the less points you receive
        {
            // streaks can be set up by 5, 10, 15, 20, 25, 30, 35, etc.
            int streakValue = streak <= 0 ? 0 : (streak - STREAK_DELAY) * STREAK_BONUS; // a one question delay is added to prevent streak bonuses on the first question.
            float multiplier = GetDifficultyMultiplier(difficulty);
            // this means they never answered
            if (answerPosition == 0)
                return 0;

            return (int)Math.Floor(((value * (BASE_POINT_SCALE - ((answerPosition - 1) / (double) POSITION_CUTOFF))) * multiplier) + streakValue);
        }

        private float GetDifficultyMultiplier(TriviaDifficulty difficulty)
        {
            return difficulty switch
            {
                TriviaDifficulty.Easy => EASY_POINT_SCALE,
                TriviaDifficulty.Medium => MED_POINT_SCALE,
                TriviaDifficulty.Hard => HARD_POINT_SCALE,
                _ => throw new Exception("Cannot determine point multiplier for the specified difficulty")
            };
        }

        public override async Task OnSessionStartAsync(GameServer server, GameSession session)
        {
            Options = server.Options;

            int questionCount = session.GetConfigValue<int>(TriviaConfig.QuestionCount);

            if (session.GetConfigValue<bool>(TriviaConfig.UseOpenTDB))
            {
                using (var tdb = new TdbClient())
                {
                    TriviaDifficulty diff = session.GetConfigValue<TriviaDifficulty>(TriviaConfig.Difficulty);
                    List<OpenTDB.TriviaQuestion> questions = await tdb.GetQuestionsAsync(questionCount, null, diff == TriviaDifficulty.Any ? null : (Difficulty?)diff);

                    if (questions == null)
                        throw new Exception("An error has occurred while retrieving questions.");

                    QuestionPool = questions.Select(ConvertTdbQuestion).ToList();
                }
            }
            else
            {
                // generate the question pool
                QuestionPool = GenerateQuestions(
                    session.GetConfigValue<int>(TriviaConfig.QuestionCount),
                    session.GetConfigValue<TriviaDifficulty>(TriviaConfig.Difficulty),
                    session.GetConfigValue<TriviaTopic>(TriviaConfig.Topics)
                    ).ToList();
            }

            session.SpectateFrequency = 10;
            session.CanSpectate = true;
            // once all of that is ready, invoke action try_get_next_questions
            session.InvokeAction(TriviaVars.TryGetNextQuestion, true);
        }

        private static TriviaQuestion ConvertTdbQuestion(OpenTDB.TriviaQuestion question)
        {
            var result = new TriviaQuestion();

            result.Question = question.Question;

            if (Enum.TryParse(question.Difficulty.ToString(), true, out TriviaDifficulty difficulty))
                result.Difficulty = difficulty;
            else
                result.Difficulty = TriviaDifficulty.Easy;

            result.Value = TriviaQuestion.GetQuestionValue(result.Difficulty);
            result.Answers = new List<TriviaAnswer>
            {
                new TriviaAnswer(question.Answer, true)
            };

            result.Answers.AddRange(question.IncorrectAnswers.Select(x => new TriviaAnswer(x)));

            result.TopicOverride = question.Category;

            return result;
        }

        // TODO: Move method elsewhere
        /// <inheritdoc />
        protected override ulong CalculateExp(GameSession session, PlayerData player)
        {
            TriviaDifficulty difficulty = session.GetConfigValue<TriviaDifficulty>(TriviaConfig.Difficulty);
            int questionCount = session.GetConfigValue<int>(TriviaConfig.QuestionCount);
            int playerCount = session.Players.Count;
            int totalCorrect = player.ValueOf<int>(TriviaVars.TotalCorrect);
            int streak = player.ValueOf<int>(TriviaVars.Streak);

            int baseQuestionExp = 2;

            Logger.Debug($"Q COUNT: {questionCount}\nP COUNT: {playerCount}\nTOTAL CORRECT: {totalCorrect}\nSTREAK: {streak}");

            // If they didn't get any correct, return a consolation 2 exp as long as there were at least 3 questions
            if (totalCorrect == 0)
            {
                if (questionCount >= 3)
                    return 2;

                return 0; // otherwise, grant no experience
            }

            if (questionCount < 10) // below 10 is considered a short game, which is reduced exp
            {
                if (questionCount <= 5)
                    baseQuestionExp = 1;

                ulong score = (ulong) (baseQuestionExp * totalCorrect); // return the amount of questions below 5

                // Don't grant bonus exp for a game with less than 5 questions
                if (questionCount < 5)
                    return score;

                if (streak == questionCount)
                    score += 5; // If all of the answers were correct, grant 5 bonus exp for a short game

                if (difficulty == TriviaDifficulty.Hard && ((totalCorrect / (double)questionCount) >= 0.5f))
                    score += 5; // If all of the answers were hard, grant 5 bonus exp if they got at least 50% of the questions right

                return score;
            }

            // If at least one other person was playing, set the base exp to 3 (150% per question)
            if (playerCount > 1)
                baseQuestionExp = 3;

            bool isOpenTdb = session.GetConfigValue<bool>(TriviaConfig.UseOpenTDB);

            // multiply score by total correct
            ulong baseScore = (ulong)(baseQuestionExp * totalCorrect);

            if (!isOpenTdb)
                return baseScore;

            float baseMultiplier = 1f;

            // add 0.25x to the base multiplier if they scored a flawless game
            if (streak == totalCorrect)
                baseMultiplier += 0.25f;

            // If all of the questions were hard AND they were using OpenTDB, add 0.10x to the base multiplier
            if (difficulty == TriviaDifficulty.Hard)
                baseMultiplier += 0.1f;

            // Multiply the score with the multiplier
            baseScore = (ulong)Math.Floor(baseScore * baseMultiplier);

            return baseScore;
        }

        public override GameResult OnGameFinish(GameSession session)
        {
            var result = new GameResult();

            var playerResults = new Dictionary<ulong, PlayerResult>();
            // because we don't have access to stats directly, we have to use stat update packets
            // NOTE: unless the stat allows it, you CANNOT update existing stats outside of the ones specified.
            foreach (PlayerData player in session.Players)
            {
                // when applying result packets, you need to specify the ID of the game.
                // By default, session results can only update specifics
                ulong playerId = player.Source.User.Id;
                var stats = new List<StatUpdatePacket>();
                stats.Add(new StatUpdatePacket(Stats.Trivia.TimesPlayed, 1));
                stats.Add(new StatUpdatePacket(Stats.Trivia.HighestScore, GetScore(session, playerId), StatUpdateType.SetIfGreater));

                if (GetWinningPlayerId(session) == playerId)
                {
                    stats.Add(new StatUpdatePacket(Stats.Trivia.TimesWon, 1));
                    stats.Add(new StatUpdatePacket(Stats.Trivia.CurrentWinStreak, 1));
                    stats.Add(new StatUpdatePacket(Stats.Trivia.LongestWin, Stats.Trivia.CurrentWinStreak, StatUpdateType.SetIfGreater));
                }
                else
                {
                    stats.Add(new StatUpdatePacket(Stats.Trivia.CurrentWinStreak, 0, StatUpdateType.Set));
                }

                var toUpdate = new PlayerResult
                {
                    Stats = stats,
                    Exp = CalculateExp(session, player)
                };

                toUpdate.PlayerProperties = player.Properties;
                playerResults.Add(playerId, toUpdate);
            }

            result.PlayerResults = playerResults;
            FinalizeResult(result, session);
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
