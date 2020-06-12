namespace Arcadia.Games
{
    public class TriviaAnswer
    {

        public TriviaAnswer(string response, bool correct = false)
        {
            Response = response;
            Correct = correct;
        }

        // what is the answer written
        public string Response { get; set; }

        // is this answer correct?
        public bool Correct { get; set; }
    }
}
