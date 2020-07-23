using Orikivo.Drawing;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;

namespace Arcadia
{

    public static class MeritHelper
    {
        public static readonly List<Merit> Merits =
            new List<Merit>
            {

            };

        public static Merit GetMerit(string id)
        {
            var merits = Merits.Where(x => x.Id == id);

            if (merits.Count() > 1)
                throw new ArgumentException("There were more than one Merits of the specified ID.");

            return merits.FirstOrDefault();
        }

        public static bool HasMerit(ArcadeUser user, string id)
            => user.Merits.ContainsKey(id);

        public static bool Exists(string id)
            => Merits.Any(x => x.Id == id);
    }
    /// <summary>
    /// Handles all methods relating to a <see cref="Merit"/>.
    /// </summary>
    internal static class MeritHandler
    {

        // notify a user about an unlocked merit
        // view information about a merit
        internal static void SendNotifications(User user, IEnumerable<Merit> merits)
        {
            foreach (Merit m in merits)
                user.Notifier.Append($"Merit unlocked: **{m.Name}**");
        }

        internal static string ViewDefault(ArcadeUser user)
        {
            bool showTooltips = user.Config?.Tooltips ?? false;

            StringBuilder panel = new StringBuilder();
            panel.AppendLine("> **Merits**");

            if (showTooltips)
                panel.AppendLine("> Use `merits <group>` to view a specific merit category.");

            panel.AppendLine();


            foreach (MeritGroup type in EnumUtils.GetValues<MeritGroup>())
            {
                IEnumerable<string> merits = MeritHelper.Merits.Where(x => x.Group == type).Select(x => x.Id);
                int collected = user.Merits.Keys.Count(k => merits.Contains(k));

                panel.Append($"> **{type.ToString()}**");
                panel.AppendLine(GetProgress(type, collected, merits.Count()));

                string summary = Summarize(type);

                if (Check.NotNull(summary))
                    panel.AppendLine($"> {summary}");
            }

            return panel.ToString();
        }

        private static string GetProgress(MeritGroup group, int collected, int total)
            => CanShowProgress(group) ? $" (`{RangeF.Convert(0, total, 0, 100, collected)}%`)" : "";

        // determines if the % of completion when viewing categories is displayed.
        private static bool CanShowProgress(MeritGroup group)
            => group switch
            {
                MeritGroup.Chaos => false,
                _ => true
            };

        // summarizes the merit group
        private static string Summarize(MeritGroup group)
            => group switch
            {
                MeritGroup.Chaos => "Represents the impossible.",
                _ => null
            };

        // view a specific merit category.
        internal static string ViewCategory(ArcadeUser user, MeritGroup group)
        {
            bool showTooltips = user.Config?.Tooltips ?? false;

            StringBuilder panel = new StringBuilder();

            panel.AppendLine("> **Merits**");
            panel.AppendLine($"> {group.ToString()}");

            panel.AppendLine();

            if (MeritHelper.Merits.Count(x => x.Group.HasFlag(group)) == 0)
                return Format.Warning($"There are no visible **Merits** found under **{group.ToString()}**.");

            if (showTooltips)
            {
                if (user.Merits.Any(x => !x.Value.IsClaimed.GetValueOrDefault(true)))
                    panel.Insert(0, "> Use `claim <id>` to claim the reward from a **Merit**.\n\n");
            }

            foreach (Merit merit in MeritHelper.Merits.Where(x => x.Group.HasFlag(group)))
            {
                panel.AppendLine(GetMeritSummary(user, merit));
            }

            return panel.ToString();
        }

        internal static string GetMeritSummary(ArcadeUser user, Merit merit)
        {
            var summary = new StringBuilder();
            bool unlocked = MeritHelper.HasMerit(user, merit.Id);

            // LINE 1
            summary.Append($"`{merit.Id}`");
            summary.Append(" • ");
            summary.Append($"**{merit.Name}**");
            summary.AppendLine();

            // LINE 2 (?)
            if (Check.NotNull(merit.Summary))
                summary.Append($"⇛ {merit.Summary}");

            summary.AppendLine();

            if (unlocked)
                summary.AppendLine($"> Achieved **{user.Merits[merit.Id].AchievedAt.ToString("M/d/yyyy @ HH:mm tt")}**");

            if (unlocked && merit.Reward != null)
            {
                summary.Append("> **Reward**: ");
                summary.AppendJoin(", ", merit.Reward.GetNames());
                summary.Append($" ({(user.Merits[merit.Id].IsClaimed.Value ? "Claimed" : "Unclaimed")})");
            }

            return summary.ToString();
        }



        // claim a merit; true if successful.
        internal static bool Claim(ArcadeUser user, string meritId)
        {
            if (Exists(user, meritId))
            {
                Merit merit = MeritHelper.GetMerit(meritId);

                //if (CanReward(user, merit))
                    //ApplyReward(user, merit.Reward);

                user.Merits[meritId].IsClaimed = true;
                return true;
            }

            return false;
        }

        internal static string ClaimAndDisplay(ArcadeUser user, Merit merit)
            => ClaimAndDisplay(user, merit.Id);

        internal static string ClaimAndDisplay(ArcadeUser user, string meritId)
        {
            if (!MeritHelper.Exists(meritId))
                return Format.Warning("The **Merit** you specified doesn't exist.");

            Merit merit = MeritHelper.GetMerit(meritId);

            if (!user.Merits.ContainsKey(meritId))
                return Format.Warning("You haven't met the criteria in order to be able to claim this **Merit**.");

            if (merit.Reward == null)
                return Format.Warning("The **Merit** you specified doesn't have a reward.");

            if (user.Merits[meritId].IsClaimed.GetValueOrDefault(true))
                return Format.Warning("You already claimed this **Merit**.");

            if (Claim(user, meritId))
                return GetRewardSummary(merit.Reward);

            return Format.Warning("An unknown error has occurred.");
        }

        private static string GetRewardSummary(Reward reward)
            => string.Join("\n", reward.GetNames().Select(x => $"• +{x}"));

        // checks if the user has the merit AND the merit exists
        private static bool Exists(ArcadeUser user, string meritId)
            => Engine.Merits.ContainsKey(meritId) && MeritHelper.HasMerit(user, meritId);

        // checks if the merit has a reward AND the user hasn't claimed it.
        private static bool CanReward(ArcadeUser user, Merit merit)
            => merit.Reward != null && (!user.Merits[merit.Id].IsClaimed ?? false);

        
    }
}