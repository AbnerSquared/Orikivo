using System.Collections.Generic;

namespace Orikivo
{
    // this is used to determine what task to switch to whenever the task is complete
    public class GameRoute
    {
        public GameRoute(TaskRoute route, string taskId)
        {
            Route = route;
            TaskId = taskId;
        }
        public TaskRoute Route { get; } // what this route is triggered by
        // if multiple types can lead to a success, specify the success id. otherwise, ignore.
        public string TaskId { get; } // the id of the task to start
        public string LastTaskId { get; internal set; } // the id of the last task.
    }

    // manages the display updates
    public class GameDisplayHandler
    {
        // a dictionary of all displays in the lobby
        // gamestate.inactive: lobby display
        // gamestate.active: game display
        // gamestate.watching: spectator display.
        Dictionary<GameState, Display> Displays { get; set; }
    }
}
