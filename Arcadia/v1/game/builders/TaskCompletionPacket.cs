using System.Collections.Generic;

namespace Arcadia.Old
{
    public class TaskCompletionPacket
    {
        public TaskCriterion Criterion { get; set; }
        public List<GameUpdatePacket> OnSuccess { get; set; }

        // Defines where to go next upon this criteria being met.
        public TaskQueuePacket Enqueue { get; set; }
    }

    // create a function determining what to do upon starting from a specific task.
}
