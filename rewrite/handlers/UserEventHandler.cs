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

// TODO: Create Booster/CooldownHandler classes that can auto-manage deleting expired boosters and cooldowns.
// find a way to auto-remove inactive items
/*
Dictionary<string, BoosterInfo> _boosters = null;
Dictionary<string, CooldownInfo> _cooldowns = null;
if (boosters != null)
{
    _boosters = boosters;
    foreach (KeyValuePair<string, BoosterInfo> booster in boosters)
        if (booster.Value.IsExpired)
            _boosters.Remove(booster.Key);
}
if (cooldowns != null)
{
    _cooldowns = cooldowns;
    foreach (KeyValuePair<string, CooldownInfo> cooldown in cooldowns)
        if (!cooldown.Value.IsActive)
            _cooldowns.Remove(cooldown.Key);
    cooldowns = _cooldowns;
}*/
