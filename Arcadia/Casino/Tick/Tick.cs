using System;
using Orikivo;

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

        public TickResult Next(long wager)
        {
            var ticks = 0;
            var alive = true;

            while (alive)
            {
                if (RandomProvider.Instance.Next(LowerBound, UpperBound) <= Chance)
                {
                    ticks++;
                    Chance--;
                    continue;
                }

                alive = false;
            }

            bool won = Method switch
            {
                TickWinMethod.Exact => ticks == ExpectedTick,
                _ => ticks >= ExpectedTick
            };

            TickResultFlag flag = won ? TickResultFlag.Win : TickResultFlag.Lose;
            float multiplier = GetMultiplier(ExpectedTick, Method);
            var reward = (long) Math.Floor(wager * multiplier);
            return new TickResult(wager, reward, flag, ExpectedTick, ticks, multiplier);
        }
    }
}
