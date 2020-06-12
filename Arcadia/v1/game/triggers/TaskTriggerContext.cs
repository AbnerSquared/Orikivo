namespace Arcadia.Old
{
    /// <summary>
    /// A trigger context utilized for game tasks.
    /// </summary>
    public class TaskTriggerContext
    {
        internal TaskTriggerContext(GameTaskData data, Player author, string message)
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
