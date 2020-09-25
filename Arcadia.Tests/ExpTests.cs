using Xunit;

namespace Arcadia.Tests
{
    public class ExpTests
    {
        [Fact]
        public void Test_ExpectEqualExp()
        {
            Assert.True(ExpConvert.AsLevel(75, 0) == 0, $"0 != 0");
            Assert.True(ExpConvert.AsExp(0, 0) == 0, $"0 != 0");
            Assert.True(ExpConvert.AsLevel(100, 0) == 1, $"1 != 1");
            Assert.True(ExpConvert.AsExp(1, 0) == 100, $"100 != 100");
            Assert.True(ExpConvert.AsLevel(4500, 0) == 9, $"4500 != 9");
            Assert.True(ExpConvert.AsExp(9, 0) == 4500, $"4500 != 4500");
            Assert.True(ExpConvert.AsExp(10, 0) == 5500, $"5500 != 5500");
            Assert.True(ExpConvert.AsExp(30, 0) == 37500, $"37500 != 37500");
            Assert.True(ExpConvert.AsLevel(37500, 0) == 30, $"37500 != 30");
        }

        [Fact]
        public void Test_ExpectSubtractExp()
        {
            Assert.True(ExpConvert.ExpBetween(1, 9, 0) == 4400);
            Assert.True(ExpConvert.ExpToLevel(4350, 9, 0) == 150);
        }
    }
}
