namespace Orikivo
{
    // this is used to determine what task to switch to whenever the task is complete
    public class GameRoute
    {
        public GameRoute(TaskRoute route, string id)
        {
            Route = route;
            Id = id;
        }
        public TaskRoute Route { get; } // what this route is triggered by
        // if multiple types can lead to a success, specify the success id. otherwise, ignore.
        public string Id { get; } // the id of the task to start
    }
}
