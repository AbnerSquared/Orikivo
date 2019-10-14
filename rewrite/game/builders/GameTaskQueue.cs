namespace Orikivo
{
    // this is used to determine what task to switch to whenever the task is complete
    /// <summary>
    /// Defines where the result of a game task proceeds with in a game.
    /// </summary>
    public class GameTaskQueue
    {
        /// <summary>
        /// Creates a new GameTaskQueue that defines the task close reason as cancelled.
        /// </summary>
        public static GameTaskQueue Empty => new GameTaskQueue(TaskCloseReason.Cancel, null);

        /// <summary>
        /// Creates a new GameTaskQueue defining the parameters of the task to enqueue.
        /// </summary>
        /// <param name="closeReason">The reason this task queue is called.</param>
        /// <param name="taskId">The identity of a GameTask to call. When empty, this will be considered as the endpoint of a game.</param>
        public GameTaskQueue(TaskCloseReason closeReason, string taskId)
        {
            CloseReason = closeReason;
            TaskId = taskId;
        }

        /// <summary>
        /// The reason this task queue was called.
        /// </summary>
        public TaskCloseReason CloseReason { get; }

        /// <summary>
        /// The identity of the GameTask to call. When empty, this will be considered as the endpoint of a game.
        /// </summary>
        public string TaskId { get; } // the id of the task to start

        /// <summary>
        /// The identity of the GameTask this was created from.
        /// </summary>
        public string LastTaskId { get; internal set; }
    }
}
