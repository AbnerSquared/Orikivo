using Orikivo;

namespace Arcadia.Casino
{
    public enum RouletteResultFlag
    {
        Win = 1,
        Lose = 2
    }

    public class Roulette
    {
        private static readonly int[] _slots =
        new[] {
                0,
                32, 15, 19,  4, 21,  2, 25, 17, 34,  6, 27, 13,
                36, 11, 30,  8, 23, 10,  5, 24, 16, 33,  1, 20,
                14, 31,  9, 22, 18, 29,  7, 28, 12, 35,  3, 26
            };

        public static bool IsGreen(int slot)
            => slot == 0;

        public static bool IsRed(int slot)
            => slot != 0 && (slot % 2 == 0);

        public static bool IsBlack(int slot)
            => slot != 0 && (slot % 2 == 1);

        public static int GetSlot()
            => RandomProvider.Instance.Next(0, 37);

            // GREEN
            // 0

            // REDS
            // 32 19 21 25 34 27 36 30 23  5 16  1 14  9 18  7 12  3

            // BLACKS
            // 15  4  2 17  6 13 11  8 10 24 33 20 31 22 29 28 35 26

        public static RouletteResult Next(ArcadeUser user)
        {
            return null;
        }
    }

    public class RouletteResult
    {
        public long Reward;
        public float Multiplier;
        public RouletteResultFlag Flag;
        public bool IsSuccess;

        public void Apply(ArcadeUser user)
        {

        }

        public Message ApplyAndDisplay(ArcadeUser user)
        {
            return null;
        }
    }
}
