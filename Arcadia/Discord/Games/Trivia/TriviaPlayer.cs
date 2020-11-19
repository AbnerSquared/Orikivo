namespace Arcadia.Multiplayer
{
    public class TriviaPlayer : PlayerBase
    {
        [Property("score")]
        public int Score { get; internal set; }

        [Property("streak")]
        public int Streak { get; internal set; }

        [Property("answer_state")]
        public TriviaAnswerState AnswerState { get; internal set; } = TriviaAnswerState.Pending;

        [Property("answer_position")]
        public int AnswerPosition { get; internal set; }

        [Property("total_correct")]
        public int TotalCorrect { get; internal set; }

        /// <inheritdoc />
        public override PlayerBase GetDefault()
        {
            return new TriviaPlayer
            {
                Score = 0,
                Streak = 0,
                AnswerState = TriviaAnswerState.Pending,
                AnswerPosition = 0,
                TotalCorrect = 0
            };
        }
    }
}
