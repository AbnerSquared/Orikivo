using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Services;
using Orikivo;

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
                return (DateTime.UtcNow - user.InternalCooldowns[id]) > TimeSpan.Zero;
            }

            return true;
        }

        public static string ViewAllTimers(ArcadeUser user)
        {
            var details = new StringBuilder();

            // Cooldowns are for daily, objective assign, objective skip, and item usage cooldowns
            IEnumerable<string> cooldowns = GetCooldownList(user);
            IEnumerable<string> expiry = GetExpiryList(user);

            if (!cooldowns.Any() && !expiry.Any())
            {
                details.AppendLine($"> 🕘 **Cooldowns**");
                details.Append("> You don't have any pending cooldowns or expiration timers.");
            }
            else if (expiry.Any() && !cooldowns.Any())
            {
                details.AppendLine($"> 💀 **Expiry**");
                details.AppendJoin("\n", expiry);
            }
            else if (!expiry.Any() && cooldowns.Any())
            {
                details.AppendLine($"> 🕘 **Cooldowns**");
                details.AppendJoin("\n", cooldowns);
            }
            else
            {
                details.AppendLine($"> 🕘 **Cooldowns**");
                details.AppendJoin("\n", cooldowns);
                details.AppendLine($"\n\n> 💀 **Expiry**");
                details.AppendJoin("\n", expiry);
            }

            return details.ToString();
        }

        public static string ViewCooldowns(ArcadeUser user)
        {
            var details = new StringBuilder();

            details.AppendLine($"> 🕘 **Cooldowns**");

            // Cooldowns are for daily, objective assign, objective skip, and item usage cooldowns
            IEnumerable<string> cooldowns = GetCooldownList(user);

            if (!cooldowns.Any())
            {
                details.AppendLine("> You don't have any pending cooldowns.");
                return details.ToString();
            }

            details.AppendJoin("\n", cooldowns);
            return details.ToString();
        }

        public static IEnumerable<string> GetCooldownList(ArcadeUser user)
        {
            var cooldowns = new List<string>();

            if (TryWriteCooldown(user, "Daily", Cooldowns.Daily, Daily.Cooldown, out string daily))
                cooldowns.Add(daily);

            if (TryWriteCooldown(user, "Objective Assign", Stats.LastAssignedQuest, QuestHelper.AssignCooldown, out string assign))
                cooldowns.Add(assign);

            if (TryWriteCooldown(user, "Objective Skip", Stats.LastSkippedQuest, QuestHelper.SkipCooldown, out string skip))
                cooldowns.Add(skip);

            return cooldowns;
        }

        private static bool TryWriteCooldown(ArcadeUser user, string name, string id, TimeSpan duration, out string row)
        {
            row = null;
            if (user.GetVar(id) > 0)
            {
                TimeSpan since = StatHelper.SinceLast(user, id);

                if (since < duration)
                {
                    row = $"> • **{name}:** **{Format.Countdown(duration - since)}** until reset";
                    return true;
                }
            }

            return false;
        }

        private static bool TryWriteExpiry(ArcadeUser user, string name, DateTime? expiresOn, int? usesLeft, out string row)
        {   // 4:23:24 (or 17 uses) left
            row = null;
            bool hasCountdown = false;

            if (!expiresOn.HasValue && !usesLeft.HasValue)
                return false;

            var line = new StringBuilder();
            line.Append($"> • **{name}:** ");

            if (expiresOn.HasValue)
            {
                TimeSpan remainder = expiresOn.Value - DateTime.UtcNow;

                if (remainder <= TimeSpan.Zero && (usesLeft ?? 0) <= 0)
                    return false;

                line.Append(Format.Countdown(remainder));
                hasCountdown = true;
            }

            if (usesLeft.HasValue)
            {
                if (usesLeft.Value <= 0 && (!expiresOn.HasValue || (expiresOn.Value - DateTime.UtcNow) <= TimeSpan.Zero))
                    return false;

                line.Append(hasCountdown
                    ? $" (or **{usesLeft.Value:##,0}** {Format.TryPluralize("use", usesLeft.Value)})"
                    : $"**{usesLeft.Value:##,0}** {Format.TryPluralize("use", usesLeft.Value)}");
            }

            line.Append(" left");

            row = line.ToString();
            return true;
        }

        public static IEnumerable<string> GetExpiryList(ArcadeUser user)
        {
            var expiry = new List<string>();

            return expiry;
        }

        public static string ViewExpiry(ArcadeUser user)
        {
            var details = new StringBuilder();

            details.AppendLine($"> 💀 **Expiry**");

            IEnumerable<string> expiry = GetExpiryList(user);

            if (!expiry.Any())
            {
                details.AppendLine("> You don't have any current expiration timers.");
                return details.ToString();
            }

            details.AppendJoin("\n", expiry);

            return details.ToString();
        }
    }
}