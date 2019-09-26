using Orikivo.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Tests
{
    public static class CalculatorTest
    {
        public static void Solve()
        {
            string expression = "1+((1+1)+(2-2))-2(1)";
            Calculator.Solve(expression);
        }
    }
}
