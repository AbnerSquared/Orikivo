using System.Collections.Generic;

namespace Arcadia.Games
{
    public class TriviaQuestion
    {
        // the topic of this question
        public TriviaTopic Topic { get; set; }

        // what is the question to display
        public string Question { get; set; }

        public List<TriviaAnswer> Answers { get; set; } = new List<TriviaAnswer>();
    }
}
