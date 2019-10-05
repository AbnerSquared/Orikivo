using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // a class that enhances the features of randomization
    public static class OriRandom
    {
        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> obj)
        {
            throw new NotImplementedException();
        }

        public static T Select<T>(IEnumerable<T> obj, int times = 1)
        {
            throw new NotImplementedException();
        }

        public static T NextElement<T>(IEnumerable<T> obj)
            => obj.ElementAt(RandomProvider.Instance.Next(0, obj.Count()));

        public static IEnumerable<T> SelectMany<T> (IEnumerable<T> obj, int times)
        {
            throw new NotImplementedException();
        }

        // should be diceresult
        public static int Roll(Dice d, int times = 1)
        {
            int result = (int)(Math.Truncate((double)(RandomProvider.Instance.Next(1, d.FaceCount * d.Length) / d.Length)) % d.FaceCount);
            Console.WriteLine(result);
            return result;
        }

        public static DiceResult RollMany(params (Dice, int)[] die)
        {
            throw new NotImplementedException();
        }

        public static string GetColorHex(HexLengthType type = HexLengthType.Default)
            => GetChars("0123456789ABCDEF", type.GetHashCode());

        // get a list of random characters using a specified tree.
        public static string GetChars(string branch, int len)
        {
            len = len > int.MaxValue ? int.MaxValue : (len < 1 ? 1 : len);
            char[] tree = new char[len];
            for (int i = 0; i < tree.Length; i++)
                tree[i] = branch[RandomProvider.Instance.Next(branch.Length)];
            return new string(tree);
        }

        public static OriColor GetColor()
        {
            throw new NotImplementedException();
        }

        public static int Next(Range range)
        {
            throw new NotImplementedException();
        }
    }
}
