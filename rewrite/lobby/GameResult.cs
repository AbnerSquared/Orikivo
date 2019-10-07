using System;

namespace Orikivo
{
    public class GameResult
    {
        private GameResult() { }
        internal static GameResult Empty => new GameResult { IsSuccess = false, ErrorReason = "Unknown error." };
        public static GameResult FromException(Exception ex)
        {
            GameResult result = new GameResult();
            result.IsSuccess = false;
            result.ErrorReason = ex.Message;
            return result;
        }
        // List<GameAction> Actions // a list of all triggers executed, actions done... etc...
        // this is used to handle updating all users that played.
        public bool IsSuccess { get; private set; }
        public string ErrorReason { get; private set; }
    }
}
