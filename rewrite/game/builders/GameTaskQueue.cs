namespace Orikivo
{
    // this is used to determine what task to switch to whenever the task is complete
    /// <summary>
    /// Defines where the result of a game task proceeds with in a game.
    /// </summary>
    public class TaskQueuePacket
    {
        /// <summary>
        /// Creates a new GameTaskQueue that defines the task close reason as cancelled.
        /// </summary>
        public static TaskQueuePacket Empty => new TaskQueuePacket(TaskQueueReason.Cancel, null);

        /// <summary>
        /// Creates a new GameTaskQueue defining the parameters of the task to enqueue.
        /// </summary>
        /// <param name="queueReason">The reason this task queue is called.</param>
        /// <param name="nextTaskId">The identity of a GameTask to call. When empty, this will be considered as the endpoint of a game.</param>
        public TaskQueuePacket(TaskQueueReason queueReason, string nextTaskId)
        {
            Reason = queueReason;
            NextTaskId = nextTaskId;
        }

        /// <summary>
        /// The reason this task queue was called.
        /// </summary>
        public TaskQueueReason Reason { get; }

        /// <summary>
        /// The identity of the GameTask to call. When empty, this will be considered as the endpoint of a game.
        /// </summary>
        public string NextTaskId { get; } // the id of the task to start

        /// <summary>
        /// The identity of the GameTask this was created from.
        /// </summary>
        public string TaskId { get; internal set; }
    }
}
