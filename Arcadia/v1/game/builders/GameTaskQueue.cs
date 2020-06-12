namespace Arcadia.Old
{
    /// <summary>
    /// Defines where the result of a game task proceeds with in a game.
    /// </summary>
    public class TaskQueuePacket
    {
        /// <summary>
        /// Creates a new queue packet that defines the task close reason as cancelled.
        /// </summary>
        public static TaskQueuePacket Empty => new TaskQueuePacket(TaskQueueReason.Cancel, null);

        /// <summary>
        /// Creates a new queue packet defining the parameters of the task to enqueue.
        /// </summary>
        /// <param name="queueReason">The reason this queue packet is called.</param>
        /// <param name="nextTaskId">The unique identifier of a game task to call. When empty, this will be considered as the endpoint of a game.</param>
        public TaskQueuePacket(TaskQueueReason queueReason, string nextTaskId)
        {
            Reason = queueReason;
            NextTaskId = nextTaskId;
        }

        /// <summary>
        /// The reason this queue packet was called.
        /// </summary>
        public TaskQueueReason Reason { get; }

        /// <summary>
        /// The unique identifier of the next game task to call. When empty, this will be considered as the endpoint of a game.
        /// </summary>
        public string NextTaskId { get; } // the id of the task to start

        /// <summary>
        /// The unique identifier of the game task this was created from.
        /// </summary>
        public string TaskId { get; internal set; }
    }
}
