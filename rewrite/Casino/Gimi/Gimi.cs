using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{


    // TODO: Create CasinoService mechanics.
    public class Gimi
    {
        public Gimi(User user)
        {
            Risk = 50;
            Earn = 20;
            GoldSlot = 999;
            CurseSlot = 0;
            WinDir = true;
            RiskOverload = 0;
            EarnExpander = 0;
            //Risk = user.GetAttribute(GimiAttribute.Risk);
            //Earn = user.GetAttribute(GimiAttribute.Earn);
            //RiskOverload = user.GetUpgrade(GimiUpgrade.RiskOverload);
            //EarnExpander = user.GetUpgrade(GimiUpgrade.MaxExpander);
            MaxEarn = GetMaxEarn();
            MaxRisk = GetMaxRisk();
            //GoldSlot = user.GetAttribute(GimiAttribute.GoldSlot); // the number that is its winnable value.
            //CurseSlot = user.GetAttribute(GimiAttribute.CurseSlot); // the number that is its winnable value.
            //WinDir = user.GetAttribute(GimiAttribute.WinDirection) == 1;
            //if (GoldSlot == 0)
            //    GoldSlot = GetSlot();
            //if (CurseSlot == 0)
            //    CurseSlot = GetSlot();
        }

        public int Risk { get; }
        public int Earn { get; }
        public int RiskOverload { get; }
        public int EarnExpander { get; }
        public int MaxRisk { get; }
        public int MaxEarn { get; }
        private int GoldSlot { get; } // 999
        private int CurseSlot { get; } // 0
        private bool WinDir { get; }

        private const int _baseMaxRisk = 100;
        private const int _riskOverloadSize = 25;
        private const int _minEarn = 2;
        private const int _baseMaxEarn = 20;
        private const int _baseMaxGold = 20;
        private const int _baseGoldChanceDenominator = 500;
        private const int _baseMaxCurseDenominator = 2;
        private const int _baseCurseChanceDenominator = 2;
        private const int _rndLength = 20;
        private const int _minSeed = 0;

        // randomly gets a number within the seed bounds
        private int GetSlot()
        {
            int slot = 0;
            List<int> _rnds = new List<int>();
            for (int i = 0; i < 3; i++)
                _rnds.Add(RandomProvider.Instance.Next(_minSeed, GetMaxSeed() + 1));
            _rnds.ForEach(x => slot += (MaxRisk * _rndLength) - x);
            return (int)Math.Truncate((decimal)(slot / 3));
        }
        
        private int GetMaxSeed()
            => MaxRisk * _rndLength;

        private double GetPercent(double value)
            => value / 100;
        private double GetPercentOf(double value, double main)
            => GetPercent(value) * main;

        public int GetMaxRisk()
            => _baseMaxRisk + (RiskOverload * _riskOverloadSize);

        public int GetMaxEarn()
            => _baseMaxEarn * (EarnExpander + 1);

        public int GetMaxGold()
            => _baseMaxGold * Earn;

        public double GetGoldChance()
            => (double)Risk / _baseGoldChanceDenominator;

        public double GetCurseChance()
            => _baseCurseChanceDenominator * GetGoldChance();

        public int GetMaxCurse()
            => GetMaxGold() / _baseMaxCurseDenominator;

        public int GetEarnUpperBound()
            => Risk <= (MaxRisk / 2) ? ((((Earn / 2) - 1) * Risk) / (MaxRisk / 2)) + 1 : Earn;

        public int GetEarnLowerBound()
            => Risk <= (MaxRisk / 2) ? 1 : ((((Earn / 2) - 1) * Risk) / (MaxRisk - (MaxRisk / 2))) + 1;

        public GimiResult Next()
        {
            int maxGold = GetMaxGold();
            double goldChance = GetGoldChance();

            int maxCurse = GetMaxCurse();
            double curseChance = GetCurseChance();

            int earnUpperBound = GetEarnUpperBound();
            int earnLowerBound = GetEarnLowerBound();

            // the amount that would be given on a normal success roll
            int baseReturnValue = (int)Math
                .Truncate((double)(RandomProvider.Instance.Next(earnLowerBound * _rndLength, (earnUpperBound + 1) * _rndLength) / _rndLength));
            int returnValue = 0;
            int minSeed = _minSeed;
            int maxSeed = GetMaxSeed();

            int seed = RandomProvider.Instance.Next(minSeed, maxSeed + 1);

            double _goldSlotCount = GetPercentOf(goldChance, maxSeed);
            double _curseSlotCount = GetPercentOf(curseChance, maxSeed);
            int goldSlotCount = (int)Math.Round(_goldSlotCount);
            int curseSlotCount = (int)Math.Round(_curseSlotCount);
            List<int> goldSlots = new List<int>();
            List<int> curseSlots = new List<int>();
            int slotDif = (GoldSlot - CurseSlot);
            bool _goldDir = (slotDif * Math.Sign(GoldSlot - CurseSlot)) >= goldSlotCount || (slotDif * Math.Sign(GoldSlot - CurseSlot)) >= curseSlotCount;
            int goldDir = _goldDir ? 1 : -1;
            int curseDir = goldDir * -1;
            GimiResultFlag flag = GimiResultFlag.Win;

            StringBuilder debug = new StringBuilder();

            debug.AppendLine($"Reward: {baseReturnValue} [Min: {earnLowerBound}] [Max: {earnUpperBound}]");
            debug.AppendLine($"[Slots: {goldSlotCount}] Gold: {maxGold} ({goldChance}%) [Root: {GoldSlot}]");
            debug.AppendLine($"Difference: {slotDif} [GoldDir: {goldDir}] [CurseDir: {curseDir}]");
            debug.AppendLine($"Seed: {seed} [Min: {minSeed}] [Max: {maxSeed}] [WinDir: {(WinDir ? 1 : -1)}]");
            for (int i = 0; i < goldSlotCount; i++)
            {
                int goldSlot = (GoldSlot + (i * goldDir)) % maxSeed;
                goldSlots.Add(goldSlot);

                debug.AppendLine($"Gold Slot {i}: {goldSlot} [Max: {maxSeed}]");
            }

            for (int i = 0; i < curseSlotCount; i++)
            {
                int curseSlot = ((CurseSlot + (i * curseDir)) % maxSeed);
                if (goldSlots.Contains(curseSlot))
                    curseSlot = goldSlots.OrderBy(x => (x - curseSlot) * Math.Sign(x - CurseSlot)).First() - curseSlot; // get the largest difference

                debug.AppendLine($"Curse Slot {i}: {curseSlot} [Max: {maxSeed}]");
                curseSlots.Add(curseSlot);
            }

            if (goldSlots.Contains(seed))
            {
                flag = GimiResultFlag.Gold;
                returnValue = maxGold;
                debug.AppendLine($"Gold");
            }
            else if (curseSlots.Contains(seed))
            {
                flag = GimiResultFlag.Curse;
                returnValue = maxCurse;
                debug.AppendLine($"Cursed");
            }
            else
            {
                int isSameDir = seed > ((int)Math.Truncate((double)(maxSeed / 2))) == WinDir ? 1 : -1;
                flag = isSameDir == 1 ? GimiResultFlag.Win : GimiResultFlag.Lose;
                returnValue = baseReturnValue;

                debug.AppendLine($"IsWin: {isSameDir}");
            }

            debug.AppendLine($"Returns: {returnValue}");

            Console.WriteLine(debug.ToString());

            return new GimiResult(returnValue, flag);
        }
    }
}
