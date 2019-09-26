using System;
using Discord;
using Orikivo.Providers;
using Orikivo.Static;
using Orikivo.Systems.Presets;

namespace Orikivo.Services
{
    // handles all risk generation
    public static class RiskManager
    {
        public static decimal MeasureSelectiveRisk(Dice d, params int[] sides)
            => MeasureRisk(d, sides.Length);

        public static decimal MeasureRisk(Dice d, int winnable)
            => 1 / ((decimal)winnable / (decimal)d.Sides);

        public static decimal MeasureRangedRisk(Dice d, int mp, bool dir)
        {
            int w = dir ? d.Sides - mp : mp - 1;
            return MeasureRisk(d, w);
        }

        public static decimal MeasureCoinRisk()
            => 1 / (decimal)(1d / 2d);

        public static decimal MeasureDoublerRisk (int times)
        {
            /*
            const int range = 45;
            const int limit = 100;

            decimal previous = 1;

            for (int i = 0; i < times; i++)
            {
                previous = (range * previous) / limit;
                previous.Debug();
            }

            return previous;*/

            return (decimal)(Math.Pow(2, times));
        }
    }

    public static class CasinoService
    {
        public static CasinoResult BetCoinFlip(OldAccount a, ulong wager, bool face = true)
        {
            WagerMode mode = WagerMode.CoinFlip;
            double reward = wager * 2;
            bool victory = false;

            bool flip = RandomProvider.Instance.Flip();
            if (flip == face)
            {
                victory = true;
            }

            decimal risk = RiskManager.MeasureCoinRisk();

            string input = face.AsCoinFlip();
            string outcome = flip.AsCoinFlip();

            return new CasinoResult(a, mode, wager, victory, risk, reward, input, outcome);
        }

        public static CasinoResult DoubleOrNothing(OldAccount a, ulong wager, int times)
        {
            WagerMode mode = WagerMode.Doubler;
            double reward = wager * Math.Pow(2, times);
            int wins = 0;
            bool victory = false;

            bool alive = true;

            while (alive)
            {
                if (RandomProvider.Instance.Next(0, 100) >= 45)
                {
                    wins += 1;
                    continue;
                }

                alive = false;
            }

            if (wins >= times)
            {
                victory = true;
            }

            string input = $"{times.ToPlaceValue()} Tick{(times > 1 ? "s":"")}";
            string outcome = $" Died at Tick {wins.ToPlaceValue()}";

            decimal risk = RiskManager.MeasureDoublerRisk(times);
            //decimal risk = (decimal)(reward / wager);
            CasinoResult result = new CasinoResult(a, mode, wager, victory, risk, reward, input, outcome);

            if (times > 1)
            {
                string summbase = "You needed {0} to earn {1}!";
                int left = (int)times - wins;
                string req = $"{(left > 1 ? $"to win {left} more times" : $"{"one".MarkdownBold()} more win")}";
                string money = $"{EmojiIndex.Balance}" + "{0}".MarkdownBold();
                result.WithLosingSummary(string.Format(summbase, req, string.Format(money, reward.ToPlaceValue())));
            }

            return result;
        }

        //exceptions should be caught before the bet.
        public static CasinoResult BetRangedRoll(OldAccount a, Dice d, int wager, int mp, bool dir = false)
        {
            WagerMode mode = WagerMode.RangedRoll;
            int lower = dir ? 1 : 2;
            int upper = dir ? (d.Sides - 1) : d.Sides;

            DiceRoll r = d.Roll();

            bool victory = false;
            if (dir)
            {
                if (r.Result > mp)
                {
                    victory = true;
                }
            }
            else
            {
                if (r.Result < mp)
                {
                    victory = true;
                }
            }

            decimal risk = RiskManager.MeasureRangedRisk(d, mp, dir);
            double reward = (double)Math.Round(wager * risk);
            // D6, Above 4
            string input = $"{d.ToString()}, {(dir ? "Above" : "Under")} {mp}";
            string outcome = $"{r.Result}";

            return new CasinoResult(a, mode, wager, victory, risk, reward, input, outcome);
        }

        public static CasinoResult BetSelectiveRoll(OldAccount a, Dice d, int wager, params int[] sides)
        {
            WagerMode mode = WagerMode.SelectiveRoll;
            DiceRoll r = d.Roll();

            bool victory = false;

            if (r.Result.EqualsAny(sides))
                victory = true;

            decimal risk = RiskManager.MeasureSelectiveRisk(d, sides);
            double reward = (double)Math.Round(risk * wager);

            string input = $"{d.ToString()}, [{string.Join(", ", sides)}]";
            string outcome = $"{r.Result}";

            return new CasinoResult(a, mode, wager, victory, risk, reward, input, outcome);
        }
    }

    public enum WagerMode
    {
        Doubler = 0,
        RangedRoll = 1,
        SelectiveRoll = 2,
        CoinFlip = 4
    }

    public enum WagerItem
    {
        Empty = 0,
        Dice = 1,
        Coin = 2,
        Tick = 4
    }

    public class CasinoResult
    {
        public static string Base = "{0} | {1} | {2}";
        public CasinoResult(OldAccount a, WagerMode mode, double wager, bool victory, decimal risk, double reward, string input, string outcome)
        {
            Player = a;
            Mode = mode;
            Item = FindItem();

            Wager = wager;
            Reward = reward;
            Risk = risk;
            Victory = victory;

            Input = input;
            Outcome = outcome;
        }

        public OldAccount Player { get; set; } // the id of the account that bet.
        public WagerItem Item { get; set; }
        public WagerMode Mode { get; set; } // the betting game mode that was played.
        public string Input { get; set; } // the string describing your input for the bet.
        public string Outcome { get; set; } // the string describing the exact roll, flip or point of the bet.
        public double Wager { get; set; } // how much the player bet.
        public double Reward { get; set; } // the winnings this bet yields.
        public decimal Risk { get; set; } // the actual raw percentage of change there was to lose.
        public bool Victory { get; set; } // if the person who wagered won.
        public string LosingSummary { get; set; } // an optional summary.

        public void WithLosingSummary(string summary)
        {
            LosingSummary = summary;
        }

        private string GetModeName()
        {
            switch (Mode)
            {
                case WagerMode.SelectiveRoll:
                    return "Selective Roll";
                case WagerMode.RangedRoll:
                    return "Ranged Roll";
                case WagerMode.Doubler:
                    return "Double or Nothing";
                case WagerMode.CoinFlip:
                    return "Coin Flip";
                default:
                    return "Unknown Mode";
            }
        }

        private WagerItem FindItem()
        {
            if (Mode.EqualsAny(WagerMode.SelectiveRoll, WagerMode.RangedRoll))
            {
                return WagerItem.Dice;
            }
            if (Mode.EqualsAny(WagerMode.Doubler))
            {
                return WagerItem.Tick;
            }
            if (Mode.EqualsAny(WagerMode.CoinFlip))
            {
                return WagerItem.Coin;
            }

            return WagerItem.Empty;
        }

        private string GetItemIcon()
        {
            switch(Item)
            {
                case WagerItem.Tick:
                    return $"{EmojiIndex.Tick}";
                case WagerItem.Dice:
                    return $"{EmojiIndex.Dice}";
                case WagerItem.Coin:
                    return $"{EmojiIndex.Coin}"; ;
                default:
                    return "";
            }
        }

        public Embed Generate()
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText(ToString());

            // Colors
            Color onEmptyColor = EmbedData.GetColor("steamerror");
            Color onLoseColor = EmbedData.GetColor("error");
            Color onWinColor = EmbedData.GetColor("origreen");

            // Titles
            string onWinTitle = "+ {0}";
            string onLoseTitle = "- {0}";
            string onOORTitle = "> {0}";

            // Money display
            string money = $"{EmojiIndex.Balance}" + "{0}".MarkdownBold();

            string defLoseDesc = "You lost at chance at {0}.";
            string defWinDesc = "You have earned " + "(x{0})".MarkdownBold() + " the initial bet!";
            string defEmptyDesc = "You do know you need money, right?";
            string defOORDesc = "You asked to wager a bit too much.";

            // exceptions based on balance.
            if (Player.Balance == 0)
            {
                e.WithColor(onEmptyColor);
                e.WithTitle(string.Format(money, "null"));
                e.WithDescription(defEmptyDesc);

                return e.Build();
            }
            if (Player.Balance - Wager < 0)
            {
                if (!Player.Config.Overflow)
                {
                    e.WithColor(onEmptyColor);
                    e.WithTitle(string.Format(onOORTitle, string.Format(money, Player.Balance.ToPlaceValue())));
                    e.WithDescription(defOORDesc);

                    return e.Build();
                }
                Wager = Player.Balance;
            }

            Player.Take((ulong)Wager);
            e.WithFooter(f);

            if (Victory)
            {
                Player.Give((ulong)Reward);
                e.WithColor(onWinColor);
                e.WithTitle(string.Format(onWinTitle, string.Format(money, Reward.ToPlaceValue())));
                e.WithDescription(string.Format(defWinDesc, Risk.ToString("##,0.0#")));

                return e.Build();
            }
            else
            {
                e.WithColor(onLoseColor);
                e.WithTitle(string.Format(onLoseTitle, string.Format(money, Wager.ToPlaceValue())));
                e.WithDescription(LosingSummary ?? string.Format(defLoseDesc, string.Format(money, Reward.ToPlaceValue())));

                return e.Build();
            }
        }

        public override string ToString()
        {
            // 0 - the landing result of the dice
            // 1 - the game mode name
            // 2 - the statistics of the game
            // aka the dice you rolled, the sides you bet on.
            return string.Format(Base, $"{GetItemIcon()}{Outcome}", GetModeName(), Input);
        }
    }
}