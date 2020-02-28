using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents an achievement for a <see cref="User"/>.
    /// </summary>
    public class Merit
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Sprite Icon { get; set; }

        public string Summary { get; set; }

        public MeritGroup Group { get; set; } = MeritGroup.Misc;

        public MeritRank Rank { get; set; }

        // TODO: Implement explicit Criteria (UserCriteria)
        public Func<User, bool> Criteria { get; set; }

        public Reward Reward { get; set; }

        public MeritData GetData()
            => new MeritData(DateTime.UtcNow, Reward == null ? null : (bool?) false);

        public string Claim(User user)
        {
            StringBuilder result = new StringBuilder();


            if (user.HasMerit(Id))
            {
                if (user.Merits[Id].IsClaimed ?? true || Reward == null)
                {
                    result.Append("This merit was either already claimed or doesn't have a reward.");
                    return result.ToString();
                }

                result.AppendLine("**Reward claimed!**");

                user.Merits[Id].IsClaimed = true;

                if (Reward.Exp.HasValue)
                {
                    user.UpdateExp(Reward.Exp.Value.exp, Reward.Exp.Value.type);
                    result.Append($"• +**");
                    result.Append(OriFormat.Notate(Reward.Exp.Value.exp));
                    result.Append("** Exp (");
                    result.Append(Reward.Exp.Value.type.ToString());
                    result.AppendLine(")");
                }

                if (Reward.Money.HasValue)
                {
                    user.Give((long)Reward.Money.GetValueOrDefault(0));
                    result.Append($"• +**");
                    result.Append(OriFormat.Notate(Reward.Money.GetValueOrDefault(0)));
                    result.Append("** Orite");
                    result.AppendLine();
                }

                foreach (KeyValuePair<string,int> item in Reward.ItemIds)
                {
                    user.AddItem(item.Key, item.Value);
                    result.Append($"• +**");
                    result.Append(WorldEngine.GetItem(item.Key).Name);
                    result.Append("**");

                    if (item.Value > 1)
                    {
                        result.Append(" (x");
                        result.Append(OriFormat.Notate(item.Value));
                        result.Append(")");
                    }

                    result.AppendLine();
                }

                return result.ToString();
            }

            result.Append("You haven't met the criteria to claim this merit.");
            return result.ToString();
        }
    }
}
