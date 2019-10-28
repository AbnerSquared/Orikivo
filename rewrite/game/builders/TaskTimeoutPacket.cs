using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class TaskTimeoutPacket
    {
        public TimeSpan Duration { get; set; }
        public List<GameUpdatePacket> OnSuccess { get; set; }
        public TaskQueuePacket Enqueue { get; set; }
    }

    // create a function determining what to do upon starting from a specific task.
}
