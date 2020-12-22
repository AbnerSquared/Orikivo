namespace Arcadia
{
    public static class Constants
    {
        public static readonly long DefaultInventoryCapacity = 4000;
        public static readonly long DefaultBoosterCapacity = 1;
        public static readonly long DefaultOrderCapacity = 3;

        public static readonly double ChipToOrite = 0.5;
        public static readonly double TokenToOrite = 10;
        public static readonly double OriteToChip = 1 / ChipToOrite;

        public static readonly long MaxLevel = 100;
        public static readonly long LevelTierLength = 10;
        public static readonly long LevelFirstTierCost = 100;
        public static readonly long LevelTierCost = 1000;
        public static readonly double AscentRateGrowth = 0.15;
        public static readonly double AscentBaseRate = 1;
        public static readonly double AscentMaxRate = 2.5;

        public static readonly double BoostMinRate = 0;
        public static readonly double BoostMaxRate = 5;
        public static readonly double BoostMaxExpenseRate = 2;
        public static readonly double BoostFlatTolerance = 0.001;

        public static readonly long DoublyBaseChance = 40;
        public static readonly long DoublyMinChance = 0;
        public static readonly long DoublyMaxChance = 70;
        public static readonly long DoublyLoseBonusInterval = 3;

        public static readonly long GimiMaxRisk = 100;
        public static readonly long GimiMaxEarnRange = 20;
        public static readonly long GimiMinEarnRange = 5;
        public static readonly long GimiDefaultRisk = 50;
        public static readonly long GimiDefaultEarnRange = 10;
        public static readonly long GimiSeedRangeLength = 20;

        public static readonly long RouletteMaxWager = 1000;

        public static readonly double GameBonusRateAddition = 0.5;
        public static readonly double GameCappedRateDeduction = 0.4;
        public static readonly long GameDailyCap = 10;

        public static readonly long DailyBaseReward = 15;
        public static readonly long DailyBaseBonus = 50;
        public static readonly long DailyBonusInterval = 5;

        public static readonly long VoteBaseReward = 1;
        public static readonly long VoteBaseBonus = 2;
        public static readonly long VoteBonusInterval = 7;

        public static readonly long QuestBaseCapacity = 2;
        public static readonly long QuestDifficultyCost = 10;
        public static readonly long QuestBasePower = 2;

        public static readonly long ResearchBaseCost = 100;
        public static readonly double ResearchBaseTierScale = 0.5;
        public static readonly double ResearchNextTierItemCountScale = 2;
    }
}
