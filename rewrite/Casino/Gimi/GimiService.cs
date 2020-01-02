using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // TODO: Create CasinoService mechanics.
    public class GimiService
    {
        public GimiService(User user)
        {
            //Risk = user.GetAttribute(GimiAttribute.Risk);
            //Earn = user.GetAttribute(GimiAttribute.Earn);
            //RiskOverload = user.GetUpgrade(GimiUpgrade.RiskOverload);
            //EarnExpander = user.GetUpgrade(GimiUpgrade.MaxExpander);
            MaxEarn = GetMaxEarn();
            MaxRisk = GetMaxRisk();
            //GoldSlot = user.GetAttribute(GimiAttribute.GoldSlot); // the number that is its winnable value.
            //CurseSlot = user.GetAttribute(GimiAttribute.CurseSlot); // the number that is its winnable value.
            //WinDir = user.GetAttribute(GimiAttribute.WinDirection) == 1;
            if (GoldSlot == 0)
                GoldSlot = GetSlot();
            if (CurseSlot == 0)
                CurseSlot = GetSlot();
        }

        public int Risk { get; }
        public int Earn { get; }
        public int RiskOverload { get; }
        public int EarnExpander { get; }
        public int MaxRisk { get; }
        public int MaxEarn { get; }
        private int GoldSlot { get; }
        private int CurseSlot { get; }
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
        private bool GetWinDirection()
            => RandomProvider.Instance.Next(0, 1001) > 500 ? true : false;

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

        public int Get()
        {
            int maxGold = GetMaxGold();
            double goldChance = GetGoldChance();

            int maxCurse = GetMaxCurse();
            double curseChance = GetCurseChance();

            int earnUpperBound = GetEarnUpperBound();
            int earnLowerBound = GetEarnLowerBound();

            // the amount that would be given on a normal success roll
            int baseReturnValue = (int)Math.Truncate((double)(RandomProvider.Instance.Next(earnLowerBound * _rndLength, (earnUpperBound + 1) * _rndLength) / _rndLength));
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

            Console.WriteLine($"[Debug] -- Reward: {baseReturnValue} [Min: {earnLowerBound}] [Max: {earnUpperBound}] --");
            Console.WriteLine($"[Debug] -- [Slots: {goldSlotCount}] Gold: {maxGold} ({goldChance}%) [Root: {GoldSlot}] --");
            Console.WriteLine($"[Debug] -- [Slots: {curseSlotCount}] Curse: {maxCurse} ({curseChance}%) [Root: {CurseSlot}] --");
            Console.WriteLine($"[Debug] -- Difference: {slotDif} [GoldDir: {goldDir}] [CurseDir: {curseDir}] --");
            Console.WriteLine($"[Debug] -- Seed: {seed} [Min: {minSeed}] [Max: {maxSeed}] [WinDir: {(WinDir ? 1 : -1)}] --");

            for (int i = 0; i < goldSlotCount; i++)
            {
                int goldSlot = (GoldSlot + (i * goldDir)) % maxSeed;
                goldSlots.Add(goldSlot);

                Console.WriteLine($"[Debug] -- Gold Slot {i}: {goldSlot} [Max: {maxSeed}] --");
            }

            for (int i = 0; i < curseSlotCount; i++)
            {
                int curseSlot = ((CurseSlot + (i * curseDir)) % maxSeed);
                if (goldSlots.Contains(curseSlot))
                    curseSlot = goldSlots.OrderBy(x => (x - curseSlot) * Math.Sign(x - CurseSlot)).First() - curseSlot; // get the largest difference

                Console.WriteLine($"[Debug] -- Curse Slot {i}: {curseSlot} [Max: {maxSeed}] --");
                curseSlots.Add(curseSlot);
            }

            if (goldSlots.Contains(seed))
            {
                returnValue = maxGold;
                Console.WriteLine($"[Debug] -- Gold --");
            }
            else if (curseSlots.Contains(seed))
            {
                returnValue = maxCurse * -1;
                Console.WriteLine($"[Debug] -- Cursed --");
            }
            else
            {
                int isSameDir = seed > ((int)Math.Truncate((double)(maxSeed / 2))) == WinDir ? 1 : -1;
                returnValue = baseReturnValue * isSameDir;

                Console.WriteLine($"[Debug] -- IsWin: {isSameDir} --");
            }

            Console.WriteLine($"[Debug] -- Returns: {returnValue} --");

            return returnValue;
        }
    }
}
