using Discord;
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
        public static void ClaimMeritAsync(User user, string id)
        {
            if (GameDatabase.Merits.ContainsKey(id))
            {
                if (user.HasMerit(id))
                {
                    if (GameDatabase.Merits[id].Reward != null && (!user.Merits[id].IsClaimed ?? false))
                        
                        foreach ((Item item, int amount) in GameDatabase.Merits[id].Reward.ItemIds.Select(x => (GameDatabase.GetItem(x.Key), x.Value)))
                        {
                            user.AddItem(item.Id, amount);
                        }

                    user.Balance += GameDatabase.Merits[id].Reward.Money.GetValueOrDefault(0);
                    if (GameDatabase.Merits[id].Reward.Exp.HasValue)
                       user.UpdateExp(GameDatabase.Merits[id].Reward.Exp.Value.exp, GameDatabase.Merits[id].Reward.Exp.Value.type);
                }
            }
        }

        public static string GetStatNameOrDefault(string stat)
        {
            Dictionary<string, string> statNames = new Dictionary<string, string>
            { ["times_cried"] = "Times Cried" };

            return statNames.ContainsKey(stat) ? Format.Bold(statNames[stat]) : stat;
        }

        // display a list of merits for a specific user.
        public static Message GetMeritContentAsync(User user, MeritGroup? group = null)
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

                    sb.AppendLine($"**{type.ToString()} ({collected}/{(type == MeritGroup.Chaos ? "???" : total.ToString())})**");
                    if (GetMeritGroupSummary(type) != null)
                        sb.AppendLine($"`{GetMeritGroupSummary(type)}`");
                }

                return new MessageBuilder { Content = sb.ToString() }.Build();
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"**Merits ({group.Value.ToString()})**:");

                foreach (KeyValuePair<string, Merit> merit in GameDatabase.Merits.Where(x => x.Value.Group == group))
                {
                    bool unlocked = user.HasMerit(merit.Key);

                    sb.Append($"`{merit.Key}` **{merit.Value.Name}**");
                    if (unlocked && merit.Value.Reward != null)
                        sb.Append($" ({(user.Merits[merit.Key].IsClaimed.Value ? "Claimed" : "Unclaimed")})");
                    if (merit.Value.Summary != null)
                        sb.Append($": {merit.Value.Summary}");
                    sb.AppendLine();

                    if (unlocked)
                        sb.AppendLine($"> Unlocked **{user.Merits[merit.Key].AchievedAt.ToString("M/d/yyyy @ HH:mm")}**");
                }

                return new MessageBuilder { Content = sb.ToString() }.Build();
            }
        }

        private static string GetMeritGroupSummary(MeritGroup group)
            => group switch
            {
                MeritGroup.Chaos => "The impossible made possible.",
                MeritGroup.Misc => "A random collection of objectives.",
                _ => null
            };
    }
}