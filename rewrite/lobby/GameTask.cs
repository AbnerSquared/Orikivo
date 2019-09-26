using System;

namespace Orikivo
{
    // this is used to store specific phases
    public class GameTask
    {
        // a collection of attributes deriving from the main game handler.
        // private List<GameAttributes> _rootAttributes;
        
        // a collection of triggers that are read when a message is sent.
        // internal List<GameTrigger> Triggers;

        // an id used to identify the current task in operation.
        // public string Id

        // a collection of local attributes.
        // List<GameAttribute> Attributes;

        // the optional amount of time before the task is completed.
        public TimeSpan? Timeout { get; }

        public bool IsCompleted { get; } // this can derive from TaskCompletionSource<bool>

        // this is a list of all tasks that this current task can transition to, based on the criteria.
        // Dictionary<string, GameCriteria> Transitions;
    }
}
