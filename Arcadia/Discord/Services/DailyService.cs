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

        public static Message ApplyAndDisplay(ArcadeUser user, DailyResultFlag flag)
        {
            long reward = Reward;
            string header = $"{Reward:##,0}";
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
                    break;

                case DailyResultFlag.Reset:
                    color = GammaPalette.NeonRed[Gamma.Max];
                    user.SetVar(Stats.Common.DailyStreak, 0);
                    break;

                case DailyResultFlag.Bonus:
                    color = GammaPalette.Glass[Gamma.Max];
                    header += $" + {Bonus:##,0}";
                    reward += Bonus;
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
            message.Content = $"*\"{Replies.GetReply(flag)}\"*";
            message.Embedder = embedder;

            return message.Build();
        }
    }
}