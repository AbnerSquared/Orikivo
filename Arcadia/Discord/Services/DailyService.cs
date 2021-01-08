using System;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Services
{
    public class DailyService
    {
        public static readonly long Reward = 15;
        public static readonly long Bonus = 50;
        public static readonly long BonusInterval = 5;
        public static readonly TimeSpan Cooldown = TimeSpan.FromHours(24);
        public static readonly TimeSpan Reset = TimeSpan.FromHours(48);

        public static DailyResultFlag Next(ArcadeUser user)
        {
            long lastTicks = user.GetVar(CooldownVars.Daily);
            long streak = user.GetVar(Stats.Common.DailyStreak);

            TimeSpan sinceLast = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastTicks);

            if (lastTicks > 0)
            {
                if (sinceLast < Cooldown)
                    return DailyResultFlag.Cooldown;

                if (sinceLast > Reset)
                {
                    if (streak > 1)
                        return DailyResultFlag.Reset;
                }
            }

            if ((streak + 1) % BonusInterval == 0)
                return DailyResultFlag.Bonus;

            return DailyResultFlag.Success;
        }

        private static string ShowStreakCounter(ArcadeUser user)
        {
            long streak = user.GetVar(Stats.Common.DailyStreak);
            string counter = streak < 5 ? "" : $"{streak:##,0} Streak • ";
            long remainder = BonusInterval - (user.GetVar(Stats.Common.DailyStreak) % BonusInterval);

            if (remainder == 1)
                return $"{counter}Upcoming bonus!";

            return $"{counter}{remainder} {Format.TryPluralize("day", remainder != 1)} until a bonus!";
        }

        private static string ShowStreakReset(ArcadeUser user)
        {
            long streak = user.GetVar(Stats.Common.DailyStreak);

            return $"Your streak of {streak:##,0} has ended.";
        }

        private static string ShowStreakBonus(ArcadeUser user)
        {
            long streak = user.GetVar(Stats.Common.DailyStreak);
            long bonusCount = (long)Math.Floor(streak / (double) 5);

            return $"You have earned {bonusCount} {(bonusCount > 1 ? "consecutive" : "")} {Format.TryPluralize("bonus", "bonuses", bonusCount != 1)}!";
        }

        public static Message ApplyAndDisplay(ArcadeUser user, DailyResultFlag flag)
        {
            long reward = Reward;
            string header = $"{Reward:##,0}";
            string footer = null;
            ImmutableColor color = ImmutableColor.GammaGreen;
            var icon = "+ 💸";

            switch (flag)
            {
                case DailyResultFlag.Cooldown:
                    TimeSpan sinceLast = StatHelper.SinceLast(user, CooldownVars.Daily);
                    TimeSpan rem = Cooldown - sinceLast;
                    DateTime time = DateTime.UtcNow.Add(rem);

                    color = GammaPalette.Amber[Gamma.Max];
                    header = Format.Countdown(rem);
                    icon = Icons.GetClock(time.Hour);
                    footer = ShowStreakCounter(user);
                    break;

                case DailyResultFlag.Reset:
                    color = GammaPalette.NeonRed[Gamma.Max];

                    if (user.GetVar(Stats.Common.DailyStreak) > 5)
                        footer = ShowStreakReset(user);

                    user.SetVar(Stats.Common.DailyStreak, 0);

                    break;

                case DailyResultFlag.Bonus:
                    color = GammaPalette.Glass[Gamma.Max];
                    // TODO: Use the daily streak to determine the bonus reward to give
                    ItemHelper.GiveItem(user, Ids.Items.CapsuleDaily1);
                    footer = ShowStreakBonus(user);
                    // reward += Bonus;
                    break;
            }

            if (flag != DailyResultFlag.Cooldown)
            {
                user.SetVar(CooldownVars.Daily, DateTime.UtcNow.Ticks);
                user.AddToVar(Stats.Common.DailyStreak);
                user.AddToVar(Stats.Common.TimesDaily);
                Var.SetIfGreater(user, Stats.Common.LongestDailyStreak, Stats.Common.DailyStreak);
                user.Give(reward);
            }

            var message = new MessageBuilder();
            var embedder = Embedder.Default;

            embedder.Color = color;
            embedder.Header = $"**{icon} {header}**";
            embedder.Footer = footer;
            message.Content = $"*\"{Replies.GetReply(flag)}\"*";
            message.Embedder = embedder;

            return message.Build();
        }
    }
}