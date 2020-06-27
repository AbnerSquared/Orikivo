using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Games
{
    public class TriviaQuestion
    {
        public TriviaQuestion() { }

        public TriviaQuestion(string question, int value, TriviaTopic topic, TriviaDifficulty difficulty, string correctAnswer, params string[] rest)
        {
            Question = question;
            Value = value;
            Topic = topic;
            Difficulty = difficulty;
            Answers = rest.Prepend(correctAnswer).Select((x, i) => new TriviaAnswer(x, i == 0)).ToList();
        }
        // the topic of this question
        public TriviaTopic Topic { get; set; }

        // what is the question to display
        public string Question { get; set; }

        // how many points is this worth?
        public int Value { get; set; }

        public List<TriviaAnswer> Answers { get; set; } = new List<TriviaAnswer>();

        public TriviaDifficulty Difficulty { get; set; }
    }
}
