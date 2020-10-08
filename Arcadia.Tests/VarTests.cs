using System;
using Xunit;

namespace Arcadia.Tests
{
    public class VarTests
    {
        [Fact]
        public void Test_ExpectInvalidVar()
        {
            Assert.False(Var.IsValid("invalidvar"));
        }

        [Fact]
        public void Test_ExpectValidVar()
        {
            Assert.True(Var.IsValid("valid:var"));
        }
    }

    public class CooldownTests
    {
        [Fact]
        public void Test_ExpectCorrectDayDifference()
        {
            var previous = new DateTime(2020, 10, 5);
            var current = new DateTime(2020, 10, 8);
            Assert.True(CooldownHelper.DaysSince(previous, current) == 3);
        }
    }
}
