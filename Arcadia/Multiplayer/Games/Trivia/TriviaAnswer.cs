namespace Arcadia.Multiplayer.Games
{
    public class TriviaAnswer
    {

        public TriviaAnswer(string response, bool isCorrect = false)
        {
            Response = response;
            IsCorrect = isCorrect;
        }

        // what is the answer written
        public string Response { get; set; }

        // is this answer correct?
        public bool IsCorrect { get; set; }
    }
}
