namespace Orikivo.Drawing
{
    internal class DitherThresholdMap
    {
        internal static int[,] Ordered2x2 = { { 0, 2 },
                                              { 3, 1 } };

        internal static int[,] Ordered3x3 = { { 0, 7, 3 },
                                              { 6, 5, 2 },
                                              { 4, 1, 8 } };

        internal static int[,] Ordered4x4 = { {  0,  8,  2, 10 },
                                              { 12,  4, 14,  6 },
                                              {  3, 11,  1,  9 },
                                              { 15,  7, 13,  5 } };

        internal static int[,] Ordered8x8 = { {  0, 48, 12, 60,  3,  51, 15, 63 },
                                              { 32, 16, 44, 28, 35,  19, 47, 31 },
                                              {  8, 56,  4, 52, 11,  59,  7, 55 },
                                              { 40, 24, 36, 20, 43,  27, 39, 23 },
                                              {  2, 50, 14, 62,  1,  49, 13, 61 },
                                              { 34, 18, 46, 30, 33,  17, 45, 29 },
                                              { 10, 58,  6, 54,  9,  57,  5, 53 },
                                              { 42, 26, 38, 22, 41,  25, 37, 21 } };
    }
}
