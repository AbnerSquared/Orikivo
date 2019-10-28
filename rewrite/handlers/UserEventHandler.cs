using System;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Handles and invokes events relating to a user.
    /// </summary>
    public class UserEventHandler // CommandExecuted
    {
        private readonly AsyncEvent<Func<User, Task>> _statsUpdatedEvent = new AsyncEvent<Func<User, Task>>();
        // User => the user of which whose stats were updated.
        public event Func<User, Task> StatsUpdated
        {
            add => _statsUpdatedEvent.Add(value);
            remove => _statsUpdatedEvent.Remove(value);
        }
        // When stats are updated, get all of the existing merits in the cache.
        // Compare each merit and their criteria, and return a collection of all successful merits.
    }
}
