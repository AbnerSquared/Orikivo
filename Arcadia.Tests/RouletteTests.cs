using System;
using System.Linq;
using Arcadia.Casino;
using Xunit;

namespace Arcadia.Tests
{
    public class RouletteTests
    {
        [Fact]
        public void Test_EnsureValidSlotIndexes()
        {
            int[] blackIndexes =
            {
                2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26,
                28, 30, 32, 34, 36
            };

            int[] redIndexes =
            {
                1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29,
                31, 33, 35
            };

            int[] blackSlots =
            {
                15,  4,  2, 17,  6, 13, 11,  8, 10,
                24, 33, 20, 31, 22, 29, 28, 35, 26
            };
            int[] redSlots =
            {
                32, 19, 21, 25, 34, 27, 36, 30, 23,
                 5, 16,  1, 14,  9, 18,  7, 12,  3
            };

            Assert.True(Roulette.Pockets[0] == 0);
            Assert.True(Roulette.IsGreen(0));

            foreach (int index in blackIndexes)
            {
                Assert.Contains(Roulette.Pockets[index], blackSlots);
                Assert.True(Roulette.IsBlack(index));
            }

            foreach (int index in redIndexes)
            {
                Assert.Contains(Roulette.Pockets[index], redSlots);
                Assert.True(Roulette.IsRed(index));
            }
        }
    }
}
