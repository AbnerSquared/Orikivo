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
}
