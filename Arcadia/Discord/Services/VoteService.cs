using System;
using Orikivo;
using Orikivo.Drawing;
using DiscordBoats;

namespace Arcadia.Services
{
    public class VoteService
    {
        public static readonly long Reward = 1; // Token
        public static readonly long Bonus = 2; // Token
        public static readonly long BonusInterval = 7; // 7 votes in a row
        public static readonly TimeSpan Cooldown = TimeSpan.FromHours(12);
        public static readonly TimeSpan Reset = TimeSpan.FromHours(48);

        public static VoteResultFlag Next(BoatClient boatClient, ArcadeUser user)
        {
            long lastTicks = user.GetVar(CooldownVars.Vote);
            long streak = user.GetVar(Stats.Common.VoteStreak);

            TimeSpan sinceLast = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastTicks);

            if (lastTicks > 0)
            {
                if (sinceLast < Cooldown)
                    return VoteResultFlag.Cooldown;
            }

            bool hasVoted = boatClient.HasVotedAsync(user.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            if (!hasVoted)
                return VoteResultFlag.Unavailable;

            if (lastTicks > 0)
            {
                if (sinceLast > Reset)
                {
                    return VoteResultFlag.Reset;
                }
            }

            if ((streak + 1) % BonusInterval == 0)
                return VoteResultFlag.Bonus;

            return VoteResultFlag.Success;
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
                    TimeSpan sinceLast = StatHelper.SinceLast(user, CooldownVars.Vote);
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
                    user.SetVar(Stats.Common.VoteStreak, 0);
                    break;

                case VoteResultFlag.Bonus:
                    content = "You have received a bonus.";
                    color = GammaPalette.Glass[Gamma.Max];
                    header += $" + {Bonus:##,0}";
                    reward += Bonus;
                    break;

                case VoteResultFlag.Success:
                    content = "Thank you for voting!";
                    color = GammaPalette.Wumpite[Gamma.Max];
                    break;

                case VoteResultFlag.Unavailable:
                    content = $"Voting allows you to receive a **Token** on each vote you submit.\nYou are given an additional 2 **Tokens** on each 7 day streak.\n**Tokens** can be cashed out, or saved to purchase upcoming special items that can only be bought with this currency!\nYou can vote every **12** hours on any voting platform, and your streak is cut short after **48**  hours.\n\n> **Voting Portal**\n• [**Discord Boats**](https://discord.boats/bot/686093964029329413/vote)\n• [**Discord Bot List**](https://top.gg/bot/686093964029329413)";
                    color = GammaPalette.Oceanic[Gamma.Max];
                    icon = Icons.Tokens;
                    header = $"Voting";
                    reward = 0;
                    break;
            }

            if (flag != VoteResultFlag.Cooldown && flag != VoteResultFlag.Unavailable)
            {
                user.SetVar(CooldownVars.Vote, DateTime.UtcNow.Ticks);
                user.AddToVar(Stats.Common.VoteStreak);
                user.AddToVar(Stats.Common.TimesVoted);
                Var.SetIfGreater(user, Stats.Common.LongestVoteStreak, Stats.Common.VoteStreak);
                user.Give(reward, CurrencyType.Tokens);
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
}