using Orikivo.Drawing;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
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

        internal static string ViewDefault(User user)
        {
            bool showTooltips = user.Config?.Tooltips ?? false;

            StringBuilder panel = new StringBuilder();
            panel.AppendLine("> **Merits**");

            if (showTooltips)
                panel.AppendLine("> Use `merits <group>` to view a specific merit category.");

            panel.AppendLine();


            foreach (MeritGroup type in EnumUtils.GetValues<MeritGroup>())
            {
                IEnumerable<string> merits = Engine.Merits.Where(x => x.Value.Group == type).Select(x => x.Key);
                int collected = user.Merits.Keys.Where(k => merits.Contains(k)).Count();

                panel.Append($"> **{type.ToString()}**");
                panel.AppendLine(GetProgress(type, collected, merits.Count()));

                string summary = Summarize(type);

                if (Check.NotNull(summary))
                    panel.AppendLine($"> {summary}");
            }

            return panel.ToString();
        }

        private static string GetProgress(MeritGroup group, int collected, int total)
            => CanShowProgress(group) ? $" (`{RangeF.Convert(0, total, 0.0f, 100.0f, collected)}%`)" : "";

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
        internal static string ViewCategory(User user, MeritGroup group)
        {
            bool showTooltips = user.Config?.Tooltips ?? false;

            StringBuilder panel = new StringBuilder();

            panel.AppendLine("> **Merits**");
            panel.AppendLine($"> {group.ToString()}");

            panel.AppendLine();

            if (Engine.GetMerits(group).Count() == 0)
                return $"> There are no visible **Merits** found under **{group.ToString()}**.";


            if (showTooltips)
            {
                if (user.Merits.Any(x => !x.Value.IsClaimed.GetValueOrDefault(true)))
                    panel.Insert(0, "> Use `claim <id>` to claim the reward from a **Merit**.\n\n");
            }

            foreach (Merit merit in Engine.GetMerits(group))
            {
                panel.AppendLine(GetMeritSummary(user, merit));
            }

            return panel.ToString();
        }

        internal static string GetMeritSummary(User user, Merit merit)
        {
            StringBuilder summary = new StringBuilder();
            bool unlocked = user.HasMerit(merit.Id);

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
        internal static bool Claim(User user, string meritId)
        {
            if (Exists(user, meritId))
            {
                Merit merit = Engine.GetMerit(meritId);

                if (CanReward(user, merit))
                    ApplyReward(user, merit.Reward);

                user.Merits[meritId].IsClaimed = true;
                return true;
            }

            return false;
        }

        internal static string ClaimAndDisplay(User user, Merit merit)
            => ClaimAndDisplay(user, merit.Id);

        internal static string ClaimAndDisplay(User user, string meritId)
        {
            if (!Engine.Merits.ContainsKey(meritId))
                return "> The **Merit** you specified doesn't exist.";

            Merit merit = Engine.GetMerit(meritId);

            if (!user.Merits.ContainsKey(meritId))
                return "> You haven't met the criteria in order to be able to claim this **Merit**.";

            if (merit.Reward == null)
                return "> The **Merit** you specified doesn't have a reward.";

            if (user.Merits[meritId].IsClaimed.GetValueOrDefault(true))
                return "> You already claimed this **Merit**.";

            if (Claim(user, meritId))
            {
                return GetRewardSummary(merit.Reward);
            }

            return "> An unknown error has occurred.";
        }

        private static string GetRewardSummary(Reward reward)
            => string.Join("\n", reward.GetNames().Select(x => $"• +{x}"));

        // checks if the user has the merit AND the merit exists
        private static bool Exists(User user, string meritId)
            => Engine.Merits.ContainsKey(meritId) && user.HasMerit(meritId);

        private static bool ItemExists(string itemId)
            => Engine.Items.ContainsKey(itemId);

        // checks if the merit has a reward AND the user hasn't claimed it.
        private static bool CanReward(User user, Merit merit)
            => merit.Reward != null && (!user.Merits[merit.Id].IsClaimed ?? false);

        // this applies the reward to the user.
        private static void ApplyReward(User user, Reward reward)
        {
            foreach ((string itemId, int amount) in reward.ItemIds)
                user.AddItem(itemId, amount);

            user.Balance += reward.Money.GetValueOrDefault(0);

            if (reward.Exp != null)
                user.UpdateExp(reward.Exp.Value, reward.Exp.Type);
        }
    }
}