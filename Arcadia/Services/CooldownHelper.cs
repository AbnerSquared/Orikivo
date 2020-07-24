using System;

namespace Arcadia
{
    public static class CooldownHelper
    {
        public static void SetCooldown(ArcadeUser user, string id, TimeSpan duration)
        {
            // if a cooldown already exists, add on to it.
            if (user.InternalCooldowns.ContainsKey(id))
            {
                user.InternalCooldowns[id] = user.InternalCooldowns[id].Add(duration);
            }
            else
            {
                user.InternalCooldowns[id] = DateTime.UtcNow.Add(duration);
            }
        }

        public static bool CanExecute(ArcadeUser user, string id)
        {
            if (user.InternalCooldowns.ContainsKey(id))
            {
                return (DateTime.UtcNow - user.InternalCooldowns[id]).Ticks > 0;
            }

            return true;
        }
    }
}