using System;
using Orikivo.Static;

namespace Orikivo
{
    public static class IntegerExtension
    {
        public static string ToPlaceValue(this ushort i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this short i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this uint i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this int i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this ulong i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this long i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this float i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this double i) =>
            i.ToString("##,0.###");

        public static string ToPlaceValue(this decimal i) =>
            i.ToString("##,0.###");

        public static bool IsOdd(this int i) =>
            i.EqualsAny(UnicodeIndex.OddNumbers);
        
        public static bool IsEven(this int i) =>
            i.EqualsAny(UnicodeIndex.EvenNumbers);

        public static int InRange(this int i, int max)
            => (i > max) ? max : i; 

        public static int InRange(this int i, int min, int max)
            => (i < min) ? min : (i > max) ? max : i; 

        public static string ToPositionValue(this int i)
        {
            switch(i)
            {
                case 1: return $"{i.ToPlaceValue()}{"st".ToSuperscript()}";
                case 2: return $"{i.ToPlaceValue()}{"nd".ToSuperscript()}";
                case 3: return $"{i.ToPlaceValue()}{"rd".ToSuperscript()}";
                default: return $"{i.ToPlaceValue()}{"th".ToSuperscript()}";
            }
        }

        public static int Pow2(this int i)
            => (int)Math.Pow(i, 2);
    }
}