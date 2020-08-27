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

        public Gimi(bool isAuto = false)
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
            IsAuto = isAuto;
            Create();
        }

        private bool IsAuto { get; }

        public int Risk { get; }

        public int Earn { get; }

        public int RiskOverload { get; }

        public int EarnExpander { get; }

        public int MaxRisk { get; }

        public int MaxEarn { get; }

        private int GoldSlot { get; }

        private int CurseSlot { get; }

        private bool Direction { get; }

        public List<int> GoldSlots { get; private set; }
        public List<int> CurseSlots { get; private set; }

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

        private void Create()
        {
            UpperEarn = GetEarnUpperBound();
            LowerEarn = GetEarnLowerBound();

            int maxSeed = GetMaxSeed();
            var goldSlots = new List<int>();

            if (!IsAuto)
            {
                goldSlots.Add(MinSeed);
                goldSlots.Add(maxSeed);
            }

            var curseSlots = new List<int> { MinSeed + 1 };

            if (!IsAuto)
                curseSlots.Add(maxSeed - 1);

            GoldSlots = goldSlots;
            CurseSlots = curseSlots;
        }

        public int LowerEarn { get; private set; }
        public int UpperEarn { get; private set; }

        public GimiResult Next()
        {
            int rawReturn = RandomProvider.Instance.Next(LowerEarn * RndLength, (UpperEarn + 1) * RndLength);
            int maxSeed = GetMaxSeed();

            int seed = RandomProvider.Instance.Next(MinSeed, maxSeed + 1);

            int reward;
            GimiResultFlag flag;

            if (!IsAuto && GoldSlots.Contains(seed))
            {
                flag = GimiResultFlag.Gold;
                // Because Pocket lawyer is now given on each gold, the reward is lowered to balance that
                reward = 50; // GetGoldReward();
            }
            else if (CurseSlots.Contains(seed))
            {
                flag = GimiResultFlag.Curse;
                reward = GetCurseReward();
            }
            else
            {
                bool isSameDir = seed > (int) Math.Truncate(maxSeed / (double) 2) == Direction;
                flag = isSameDir ? GimiResultFlag.Win : GimiResultFlag.Lose;
                reward = (int)Math.Truncate(rawReturn / (double)RndLength);
            }

            Logger.Debug($"Seed: {seed} ({flag})\nReward: {reward}");
            return new GimiResult(reward, flag, Risk);
        }
    }
}
