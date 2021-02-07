using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Multiplayer.Games
{
    public class TriviaQuestion
    {
        public static TriviaQuestion CreateTrueFalse(string question, TriviaTopic topic, TriviaDifficulty difficulty, bool answerResult)
        {
            string correct = answerResult ? "True" : "False";
            string incorrect = answerResult ? "False" : "True";
            return new TriviaQuestion(question, topic, difficulty, correct, incorrect);
        }

        public static TriviaQuestion CreateTrueFalse(string question, string topic, TriviaDifficulty difficulty, bool answerResult)
        {
            string correct = answerResult ? "True" : "False";
            string incorrect = answerResult ? "False" : "True";
            return new TriviaQuestion(question, topic, difficulty, correct, incorrect);
        }

        internal static int GetQuestionValue(TriviaDifficulty difficulty)
        {
            return difficulty switch
            {
                TriviaDifficulty.Easy => 10,
                TriviaDifficulty.Medium => 15,
                TriviaDifficulty.Hard => 25,
                _ => 10
            };
        }

        public TriviaQuestion() { }

        public TriviaQuestion(string question, TriviaTopic topic, TriviaDifficulty difficulty, string correctAnswer, params string[] rest)
        {
            Question = question;
            Value = GetQuestionValue(difficulty);
            Topic = topic;
            Difficulty = difficulty;
            Answers = rest.Prepend(correctAnswer).Select((x, i) => new TriviaAnswer(x, i == 0)).ToList();
        }

        public TriviaQuestion(string question, string topic, TriviaDifficulty difficulty, string correctAnswer, params string[] rest)
        {
            Question = question;
            Value = GetQuestionValue(difficulty);
            TopicOverride = topic;
            Difficulty = difficulty;
            Answers = rest.Prepend(correctAnswer).Select((x, i) => new TriviaAnswer(x, i == 0)).ToList();
        }

        public TriviaQuestion(string question, int value, TriviaTopic topic, TriviaDifficulty difficulty, string correctAnswer, params string[] rest)
        {
            Question = question;
            Value = value;
            Topic = topic;
            Difficulty = difficulty;
            Answers = rest.Prepend(correctAnswer).Select((x, i) => new TriviaAnswer(x, i == 0)).ToList();
        }

        public TriviaQuestion(string question, int value, string topic, TriviaDifficulty difficulty, string correctAnswer, params string[] rest)
        {
            Question = question;
            Value = value;
            TopicOverride = topic;
            Difficulty = difficulty;
            Answers = rest.Prepend(correctAnswer).Select((x, i) => new TriviaAnswer(x, i == 0)).ToList();
        }

        // the topic of this question
        public TriviaTopic Topic { get; set; }
        public string TopicOverride { get; set; }

        // what is the question to display
        public string Question { get; set; }

        // how many points is this worth?
        public int Value { get; set; }

        public List<TriviaAnswer> Answers { get; set; } = new List<TriviaAnswer>();

        public TriviaDifficulty Difficulty { get; set; }

        public TriviaResponse Response { get; set; } = TriviaResponse.Multiple;
    }

    public enum TriviaResponse
    {
        Multiple = 1,
        Boolean = 2
    }
}
