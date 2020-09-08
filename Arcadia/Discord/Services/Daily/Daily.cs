using System;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Services
{
    public enum VoteResultFlag
    {
        Unavailable = 0,
        Success = 1,
        Cooldown = 2,
        Reset = 3,
        Bonus = 4
    }

    public class Vote
    {
        public static readonly long Reward = 1; // Token
        public static readonly long Bonus = 2; // Token
        public static readonly long BonusInterval = 7; // 7 votes in a row
        public static readonly TimeSpan Cooldown = TimeSpan.FromHours(12);
        public static readonly TimeSpan Reset = TimeSpan.FromHours(48);

        public static VoteResultFlag Next(ArcadeUser user)
        {
            return VoteResultFlag.Unavailable;
            /*
            long lastTicks = user.GetVar(Cooldowns.Vote);
            long streak = user.GetVar(Stats.VoteStreak);

            TimeSpan sinceLast = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastTicks);

            if (lastTicks > 0)
            {
                if (sinceLast < Cooldown)
                    return VoteResultFlag.Cooldown;
            }

            if (sinceLast > Reset)
            {
                return VoteResultFlag.Reset;
            }

            if ((streak + 1) % BonusInterval == 0)
                return VoteResultFlag.Bonus;

            return VoteResultFlag.Success;
            */
        }

        public static Message ApplyAndDisplay(ArcadeUser user, VoteResultFlag flag)
        {
            long reward = Reward;
            string header = $"{Reward:##,0}";
            ImmutableColor color = GammaPalette.Lemon[Gamma.Max];
            string icon = $"+ {Icons.Tokens}";
            string content = "Thank you for voting!";

            switch (flag)
            {
                case VoteResultFlag.Cooldown:
                    TimeSpan sinceLast = StatHelper.SinceLast(user, Cooldowns.Daily);
                    TimeSpan rem = Cooldown - sinceLast;
                    DateTime time = DateTime.UtcNow.Add(rem);

                    content = "You are on cooldown.";
                    color = GammaPalette.Amber[Gamma.Max];
                    header = Format.Countdown(rem);
                    icon = Icons.GetClock(time.Hour);
                    break;

                case VoteResultFlag.Reset:
                    content = "Your streak has been reset.";
                    color = GammaPalette.NeonRed[Gamma.Max];
                    user.SetVar(Stats.DailyStreak, 0);
                    break;

                case VoteResultFlag.Bonus:
                    content = "You have received a bonus.";
                    color = GammaPalette.Glass[Gamma.Max];
                    header += $" + {Bonus:##,0}";
                    reward += Bonus;
                    break;

                case VoteResultFlag.Unavailable:
                    content = $"Voting is an upcoming mechanic from which you receive a {Icons.Tokens} **Token** on each vote (with **2** more on each 7 day streak)!\n{Icons.Tokens} **Tokens** can be cashed out, or saved to purchase upcoming special items that can only be bought with this currency!\nYou can vote every **12** hours, and your streak is cut short after **48**  hours.";
                    color = GammaPalette.Oceanic[Gamma.Max];
                    icon = Icons.Tokens;
                    header = $"Voting";
                    reward = 0;
                    break;
            }

            var message = new MessageBuilder();
            var embedder = Embedder.Default;

            embedder.Color = color;
            embedder.Header = $"{icon} {header}";
            message.Content = content;
            message.Embedder = embedder;

            return message.Build();
        }

    }

    public class Daily
    {
        public static readonly long Reward = 15;
        public static readonly long Bonus = 50;
        public static readonly long BonusInterval = 5;
        public static readonly TimeSpan Cooldown = TimeSpan.FromHours(24);
        public static readonly TimeSpan Reset = TimeSpan.FromHours(48);

        public static DailyResultFlag Next(ArcadeUser user)
        {
            long lastTicks = user.GetVar(Cooldowns.Daily);
            long streak = user.GetVar(Stats.DailyStreak);

            TimeSpan sinceLast = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastTicks);

            if (lastTicks > 0)
            {
                if (sinceLast < Cooldown)
                    return DailyResultFlag.Cooldown;
            }

            if (sinceLast > Reset)
            {
                if (streak > 1)
                    return DailyResultFlag.Reset;
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
                    TimeSpan sinceLast = StatHelper.SinceLast(user, Cooldowns.Daily);
                    TimeSpan rem = Cooldown - sinceLast;
                    DateTime time = DateTime.UtcNow.Add(rem);

                    color = ImmutableColor.NeonRed;
                    header = Format.Countdown(rem);
                    icon = Icons.GetClock(time.Hour);
                    break;

                case DailyResultFlag.Reset:
                    color = GammaPalette.Default[Gamma.Max];
                    user.SetVar(Stats.DailyStreak, 0);
                    break;

                case DailyResultFlag.Bonus:
                    color = GammaPalette.Glass[Gamma.Max];
                    header += $" + {Bonus:##,0}";
                    reward += Bonus;
                    break;
            }

            if (flag != DailyResultFlag.Cooldown)
            {
                user.SetVar(Cooldowns.Daily, DateTime.UtcNow.Ticks);
                user.AddToVar(Stats.DailyStreak);
                user.AddToVar(Stats.TimesDaily);
                Var.SetIfGreater(user, Stats.LongestDailyStreak, Stats.DailyStreak);
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