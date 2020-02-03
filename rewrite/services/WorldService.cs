using Discord;
using Orikivo.Drawing;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // the service that handles all RPG related actions
    public class WorldService
    {
        /// <summary>
        /// Attempts to update a <see cref="User"/> with the <see cref="Reward"/> that is attached to the specified <see cref="Merit"/>.
        /// </summary>
        public static void ClaimMerit(User user, string id)
        {
            if (GameDatabase.Merits.ContainsKey(id) && user.HasMerit(id))
            {
                if (GameDatabase.Merits[id].Reward != null && (!user.Merits[id].IsClaimed ?? false))
                {
                    foreach ((Item item, int amount) in GameDatabase.Merits[id].Reward.ItemIds.Select(x => (GameDatabase.GetItem(x.Key), x.Value)))
                        user.AddItem(item.Id, amount);

                    user.Balance += GameDatabase.Merits[id].Reward.Money.GetValueOrDefault(0);

                    if (GameDatabase.Merits[id].Reward.Exp.HasValue)
                        user.UpdateExp(GameDatabase.Merits[id].Reward.Exp.Value.exp, GameDatabase.Merits[id].Reward.Exp.Value.type);

                    user.Merits[id].IsClaimed = true;
                }
            }
        }

        public static string GetNameOrDefault(string stat)
        {
            Dictionary<string, string> statNames = new Dictionary<string, string>
            { ["times_cried"] = "Times Cried" };

            return statNames.ContainsKey(stat) ? Format.Bold(statNames[stat]) : stat;
        }

        // display a list of merits for a specific user.
        public static Message GetMeritPanel(User user, MeritGroup? group = null)
        {
            if (group == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("**Merits**");
                sb.AppendLine();

                foreach (MeritGroup type in EnumUtils.GetValues<MeritGroup>())
                {
                    var merits = GameDatabase.Merits.Where(x => x.Value.Group == type);
                    int total = merits.Count();

                    var keys = merits.Select(x => x.Key);
                    int collected = user.Merits.Keys.Where(k => keys.Contains(k)).Count();

                    sb.AppendLine($"> **{type.ToString()}** `{RangeF.Convert(0, merits.Count(), 0.0f, 100.0f, collected)}%`");
                    if (GetMeritGroupSummary(type) != null)
                        sb.AppendLine($"> {GetMeritGroupSummary(type)}");
                }

                return new MessageBuilder { Content = sb.ToString() }.Build();
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("**Merits**");
                if (Checks.NotNull(group))
                    sb.AppendLine($"{group.Value.ToString()}");

                sb.AppendLine();

                foreach (KeyValuePair<string, Merit> merit in GameDatabase.Merits.Where(x => x.Value.Group == group))
                {
                    bool unlocked = user.HasMerit(merit.Key);

                    // LINE 1
                    sb.Append($"`{merit.Key}`");
                    sb.Append(" • ");
                    sb.Append($"**{merit.Value.Name}**");
                    sb.AppendLine();

                    // LINE 2 (?)
                    if (Checks.NotNull(merit.Value.Summary))
                        sb.Append($"⇛ {merit.Value.Summary}");

                    sb.AppendLine();

                    if (unlocked)
                        sb.AppendLine($"> Achieved **{user.Merits[merit.Key].AchievedAt.ToString("M/d/yyyy @ HH:mm tt")}**");

                    if (unlocked && merit.Value.Reward != null)
                    {
                        sb.Append("> Reward: ");

                        // TODO: Use a listing format system. just gather all of the proper naming
                        sb.Append("**");

                        sb.AppendJoin(", ", merit.Value.Reward.GetNames());

                        sb.Append("**");
                        sb.Append($" ({(user.Merits[merit.Key].IsClaimed.Value ? "Claimed" : "Unclaimed")})");
                    }

                    
                }

                return new MessageBuilder { Content = sb.ToString() }.Build();
            }
        }

        private static string GetMeritGroupSummary(MeritGroup group)
            => group switch
            {
                MeritGroup.Chaos => "Achievements somehow made possible.",
                MeritGroup.Misc => "Ungrouped accomplishments.",
                _ => null
            };
    }
}