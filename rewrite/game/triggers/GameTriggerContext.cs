namespace Orikivo
{
    // a mini context for the trigger.
    public class GameTriggerContext
    {
        internal GameTriggerContext(GameTaskData data, Player author, string message)
        {
            Data = data;
            Author = author;
            Message = message;
        }

        public GameTaskData Data { get; }
        public Player Author { get; }
        public string Message { get; }
    }
}
