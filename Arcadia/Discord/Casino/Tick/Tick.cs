﻿using System;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Casino
{
    public class Tick
    {
        public static readonly int BaseChance = 40;
        public static readonly int LowerBound = 0;
        public static readonly int UpperBound = 100;

        public Tick(int expectedTick, TickWinMethod method = TickWinMethod.Below)
        {
            Method = method;
            ExpectedTick = expectedTick;
            Chance = BaseChance;
        }

        public int ExpectedTick { get; }

        public int Chance { get; private set; }

        public TickWinMethod Method { get; }

        public static float GetMultiplier(int ticks, TickWinMethod method)
            => (float) (Math.Pow(2, ticks) * (method == TickWinMethod.Exact ? 1.5 : 1));

        public TickResult Next(long wager, long lossStreak = 0)
        {
            var ticks = 0;
            var alive = true;

            int chance = BaseChance;

            if (lossStreak > 0)
            {
                int chanceBonus = (int) Math.Floor(lossStreak / (double)3);
                chance += chanceBonus;

                if (chance > 70)
                    chance = 70;
            }

            while (alive)
            {
                if (RandomProvider.Instance.Next(LowerBound, UpperBound) <= chance)
                {
                    ticks++;
                    chance--;

                    if (chance <= 0)
                        break;

                    continue;
                }

                alive = false;
            }

            Chance = chance;
            bool won = Method switch
            {
                TickWinMethod.Exact => ticks == ExpectedTick,
                _ => ticks >= ExpectedTick
            };

            TickResultFlag flag = won ? TickResultFlag.Win : TickResultFlag.Lose;

            if (won && Method == TickWinMethod.Exact)
                flag = TickResultFlag.Exact;

            float multiplier = GetMultiplier(ExpectedTick, Method);
            var reward = (long) Math.Floor(wager * multiplier);

            Logger.Debug($"Tick died at {ticks} ({chance}%) [{flag} - {wager} => {reward} (x{multiplier})]");
            return new TickResult(wager, reward, flag, ExpectedTick, ticks, multiplier);
        }
    }
}
