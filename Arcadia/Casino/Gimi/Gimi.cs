using Orikivo;
using Orikivo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Casino
{
    public class Gimi
    {
        private const int BaseMaxRisk = 100;
        private const int RiskOverloadSize = 25;
        private const int BaseMaxEarn = 20;
        private const int BaseMaxGold = 20;
        private const int BaseGoldChanceDenominator = 500;
        private const int BaseMaxCurseDenominator = 2;
        private const int BaseCurseChanceDenominator = 2;
        private const int RndLength = 20;
        private const int MinSeed = 0;

        public Gimi()
        {
            Risk = 50;
            Earn = 20;
            GoldSlot = 999;
            CurseSlot = 0;
            Direction = true;
            RiskOverload = 0;
            EarnExpander = 0;
            MaxEarn = GetMaxEarn();
            MaxRisk = GetMaxRisk();
        }

        public int Risk { get; }

        public int Earn { get; }

        public int RiskOverload { get; }

        public int EarnExpander { get; }

        public int MaxRisk { get; }

        public int MaxEarn { get; }

        private int GoldSlot { get; }

        private int CurseSlot { get; }

        private bool Direction { get; }

        private int GetMaxSeed()
            => MaxRisk * RndLength;

        private static double GetPercent(double value)
            => value / 100;

        private static double PercentOf(double value, double main)
            => GetPercent(value) * main;

        public int GetMaxRisk()
            => BaseMaxRisk + RiskOverload * RiskOverloadSize;

        public int GetMaxEarn()
            => BaseMaxEarn * (EarnExpander + 1);

        public int GetGoldReward()
            => BaseMaxGold * Earn;

        public double GetGoldChance()
            => (double) Risk / BaseGoldChanceDenominator;

        public double GetCurseChance()
            => BaseCurseChanceDenominator * GetGoldChance();

        public int GetCurseReward()
            => GetGoldReward() / BaseMaxCurseDenominator;

        public int GetEarnUpperBound()
        {
            if (Risk > MaxRisk / 2)
                return Earn;

            return (Earn / 2 - 1) * Risk / (MaxRisk / 2) + 1;
        }

        public int GetEarnLowerBound()
        {
            if (Risk <= MaxRisk / 2)
                return 1;

            return (Earn / 2 - 1) * Risk / (MaxRisk - MaxRisk / 2) + 1;
        }

        public GimiResult Next()
        {
            double goldChance = GetGoldChance();
            double curseChance = GetCurseChance();
            int earnUpperBound = GetEarnUpperBound();
            int earnLowerBound = GetEarnLowerBound();

            int rawReturn = RandomProvider.Instance.Next(earnLowerBound * RndLength, (earnUpperBound + 1) * RndLength);
            var baseReturnValue = (int) Math.Truncate(rawReturn / (double) RndLength);
            int maxSeed = GetMaxSeed();

            int seed = RandomProvider.Instance.Next(MinSeed, maxSeed + 1);
            var goldSize = (int) Math.Ceiling(PercentOf(goldChance, maxSeed));
            var curseSize = (int) Math.Ceiling(PercentOf(curseChance, maxSeed));

            int slotDiff = Math.Abs(GoldSlot - CurseSlot);
            bool goldDirection =  slotDiff >= goldSize || slotDiff >= curseSize;
            int goldDir = goldDirection ? 1 : -1;

            var debug = new StringBuilder();

            debug
                .AppendLine("Executed a Gimi generation")
                .AppendLine($"Reward Base: {baseReturnValue} [Min: {earnLowerBound}] [Max: {earnUpperBound}]")
                .AppendLine($"Gold Base: {GetGoldReward()} [Slots: 0, 2000] ({PercentOf(2 / (double) maxSeed, maxSeed)}%)")
                .AppendLine($"Curse Base: {GetCurseReward()} [Slots: 1, 1999] ({PercentOf(2 / (double)maxSeed, maxSeed)}%)")
                .AppendLine($"Seed: {seed} [Min: {MinSeed}] [Max: {maxSeed}] [WinDir: {(Direction ? 1 : -1)}]");

            var goldSlots = new List<int>();
            goldSlots.Add(MinSeed);
            goldSlots.Add(maxSeed);
            /*
            for (var i = 0; i < goldSize; i++)
            {
                int goldSlot = (GoldSlot + i * goldDir) % maxSeed;
                goldSlots.Add(goldSlot);

                debug.AppendLine($"Gold Slot {i}: {goldSlot} [Max: {maxSeed}]");
            }
            */
            var curseSlots = new List<int>();
            curseSlots.Add(MinSeed + 1);
            curseSlots.Add(maxSeed - 1);
            /*
            for (var i = 0; i < curseSize; i++)
            {
                int curseSlot = (CurseSlot + i * -goldDir) % maxSeed;

                if (goldSlots.Contains(curseSlot))
                    curseSlot = goldSlots.OrderBy(x => (x - curseSlot) * Math.Sign(x - CurseSlot)).First() - curseSlot; // get the largest difference

                debug.AppendLine($"Curse Slot {i}: {curseSlot} [Max: {maxSeed}]");
                curseSlots.Add(curseSlot);
            }
            */
            int reward;
            GimiResultFlag flag;

            if (goldSlots.Contains(seed))
            {
                flag = GimiResultFlag.Gold;
                reward = GetGoldReward();
            }
            else if (curseSlots.Contains(seed))
            {
                flag = GimiResultFlag.Curse;
                reward = GetCurseReward();
            }
            else
            {
                bool isSameDir = seed > (int) Math.Truncate(maxSeed / (double) 2) == Direction;
                flag = isSameDir ? GimiResultFlag.Win : GimiResultFlag.Lose;
                reward = baseReturnValue;

            }

            debug.AppendLine($"Result: {flag}");
            debug.Append($"Reward: {reward}");

            Logger.Debug(debug.ToString());

            return new GimiResult(reward, flag, Risk);
        }
    }
}
