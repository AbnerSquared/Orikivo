﻿using System;
using System.Collections.Generic;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Services
{
    public static class DailyService
    {
        public static readonly long Reward = 15;
        public static readonly long DefaultBonus = 25;
        public static readonly long BonusInterval = 5;
        public static readonly TimeSpan Cooldown = TimeSpan.FromHours(24);
        public static readonly TimeSpan Reset = TimeSpan.FromHours(48);

        private static readonly int DefaultBaseWeight = 40;
        private static readonly int DefaultDecayRate = 5;
        private static readonly int DefaultDecayCap = 5;
        private static readonly int CommonBaseRate = 50;
        private static readonly int CommonMinStart = 1; // Starts on the first consecutive bonus
        private static readonly int UncommonMinStart = 3; // Starts on the 3rd consecutive bonus
        private static readonly int RareMinStart = 5; // Starts on the 5th consecutive bonus
        private static readonly int CommonGrowthCap = 100;
        private static readonly int UncommonGrowthCap = 50;
        private static readonly int RareGrowthCap = 5;
        private static readonly int UncommonBaseRate = 5;
        private static readonly int RareBaseRate = 0;
        private static readonly int CommonGrowthRate = 5;
        private static readonly int UncommonGrowthRate = 2;
        private static readonly int RareGrowthRate = 1;
        // GROWTH, DECAY
        public static Reward GetBonus(long dailyStreak)
        {
            int bonusStreak = (int)(Math.Floor(dailyStreak / (double)BonusInterval) - 1);


            int defaultWeight = DefaultBaseWeight - Math.Min((DefaultDecayRate * Math.Max(bonusStreak, 0)), DefaultDecayCap);
            int commonWeight = GetWeighedChance(bonusStreak, CommonBaseRate, CommonMinStart, CommonGrowthRate, CommonGrowthCap);
            int uncommonWeight = GetWeighedChance(bonusStreak, UncommonBaseRate, UncommonMinStart, UncommonGrowthRate, UncommonGrowthCap);
            int rareWeight = GetWeighedChance(bonusStreak, RareBaseRate, RareMinStart, RareGrowthRate, RareGrowthCap);

            var loot = new LootTable
            {
                Entries = new List<LootEntry>
                {
                    new LootEntry
                    {
                        Money = DefaultBonus,
                        Weight = defaultWeight
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.CapsuleDaily1,
                        Weight = commonWeight
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.CapsuleDaily2,
                        Weight = uncommonWeight
                    },
                    new LootEntry
                    {
                        ItemId = Ids.Items.CapsuleDaily3,
                        Weight = rareWeight
                    }
                }
            };

            return loot.Next(1);
        }

        private static int GetWeighedChance(double streak, double baseRate, double minStart, double growthRate, double growthCap)
        {
            if (streak >= minStart)
                return (int)(baseRate + Math.Min((growthRate * Math.Max((streak - minStart), 0)), growthCap));

            return 0;
        }

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

            if (IsBonusUpNext(streak))
                return DailyResultFlag.Bonus;

            return DailyResultFlag.Success;
        }

        public static bool IsBonusUpNext(long streak)
            => (streak + 1) % BonusInterval == 0;

        private static string ShowStreakCounter(ArcadeUser user)
        {
            long streak = user.GetVar(Stats.Common.DailyStreak);
            string counter = $"> Current Streak: **{streak:##,0}**\n";
            long remainder = BonusInterval - (user.GetVar(Stats.Common.DailyStreak) % BonusInterval);

            if (remainder == 1)
                return $"{counter}> Next Bonus: **Up Next!**";

            return $"{counter}> Next Bonus: **{remainder} {Format.TryPluralize("Day", remainder != 1)}**";
        }

        private static string ShowStreakReset(ArcadeUser user)
        {
            long streak = user.GetVar(Stats.Common.DailyStreak);

            return $"> Your consecutive streak of **{streak:##,0}** has ended.";
        }

        public static long GetBonusStreak(long dailyStreak)
            => (long)Math.Floor(dailyStreak / (double)BonusInterval);

        public static string ShowStreakBonus(long dailyStreak, Reward reward)
        {
            long bonusCount = GetBonusStreak(dailyStreak);

            return $"> ⭐ **You have reached your {GetStreakOrdinal((int)bonusCount)} {(bonusCount > 1 ? "consecutive " : "")}bonus!**\n> You were rewarded:\n{reward.ToString()}";
        }

        private static string GetStreakOrdinal(int bonusCount)
        {
            return bonusCount switch
            {
                1 => "first",
                2 => "second",
                3 => "third",
                4 => "fourth",
                5 => "fifth",
                6 => "sixth",
                7 => "seventh",
                8 => "eighth",
                9 => "ninth",
                10 => "tenth",
                11 => "eleventh",
                12 => "twelveth",
                13 => "thirteenth",
                14 => "fourteenth",
                15 => "fifteenth",
                16 => "sixteenth",
                17 => "seventeenth",
                18 => "eightteenth",
                19 => "ninteenth",
                _ => Format.Ordinal(bonusCount)
            };
        }

        public static Message ApplyAndDisplay(ArcadeUser user, DailyResultFlag flag)
        {
            long reward = Reward;
            string header = $"{Reward:##,0}";
            string footer = null;
            ImmutableColor color = ImmutableColor.GammaGreen;
            var icon = "+ 💸";
            long dailyStreak = user.GetVar(Stats.Common.DailyStreak);

            switch (flag)
            {
                case DailyResultFlag.Cooldown:
                    TimeSpan sinceLast = StatHelper.SinceLast(user, CooldownVars.Daily);
                    TimeSpan rem = Cooldown - sinceLast;
                    DateTime time = DateTime.UtcNow.Add(rem);

                    color = GammaPalette.Amber[Gamma.Max];
                    header = Format.Countdown(rem);
                    icon = Icons.GetAnalogTime(time.Hour);
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
                    Reward bonus = GetBonus(dailyStreak + 1);
                    footer = ShowStreakBonus(dailyStreak + 1, bonus);
                    bonus.Apply(user);
                    // reward += Bonus;
                    break;
            }

            if (flag != DailyResultFlag.Cooldown)
            {
                user.SetVar(CooldownVars.Daily, DateTime.UtcNow.Ticks);
                Var.Add(user, 1, Stats.Common.DailyStreak, Stats.Common.TimesDaily);
                Var.SetIfGreater(user, Stats.Common.LongestDailyStreak, Stats.Common.DailyStreak);
                user.Give(reward);
            }

            var message = new MessageBuilder();
            var embedder = Embedder.Default;

            embedder.Color = color;
            embedder.Header = $"**{icon} {header}**";
            message.BaseContent = $"{footer}\n ";
            message.Content = $"\"{Replies.GetReply(flag, user)}\"";
            message.Embedder = embedder;

            return message.Build();
        }
    }
}