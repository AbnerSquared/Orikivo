namespace Orikivo.Drawing
{
    /*
     * public const char ADD_OPERATOR = '+';
        public const char SUB_OPERATOR = '-';
        public const char MULTIPLY_OPERATOR = '*';
        public const char DIV_OPERATOR = '/';
        public const char POW_OPERATOR = '^';
        // Allow parsing equations, PEMDAS
        // Parentheses, always resolve the parentheses first
        // Exponents
        // Multiply
        // Divide
        // Addition
        // Subtraction
        // (5 + 3 * 2 - 4(24 / 2 + 1))
        // 1. go to the inner most brackets
        // (24 / 2 + 1)
        // 2. D_ivide.
        // (12 + 1)
        // 3. A_dd.
        // (13)

        // 4. Go to the next inner most parentheses.
        // (5 + 3 * 2 - 4(13))
        // 5. No exponents... Multiply all multipliers, including values without an operator, next to a ().
        // (5 + (3 * 2) - (4 * 13))
        // (5 + 6 - 52)
        // 6. Add.
        // (11 - 52)
        // 7. Subtract
        // -41
        // The answer has been solved.
     */

    public static class Calc
    {
        // int, uint, byte, long, ulong, sbyte, short, ushort, double, float

        public static uint MinusRem(uint a, uint b)
        {
            uint rem = a - b < 0 ? b - a : 0;

            return rem;
        }

        public static ulong MinusRem(ulong a, ulong b)
        {
            ulong rem = a - b < 0 ? b - a : 0;

            return rem;
        }

        public static double Parity(double x)
        {
            return x % 2; // 0 if even, 1 if odd
        }

        /// <summary>
        /// Returns a 32-bit integer defining if the number is even (0) or odd (1).
        /// </summary>
        public static int Parity(int x)
            => x % 2;

    }
}
