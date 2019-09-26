using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // a class that enhances the features of randomization
    public class OriRandom
    {
        private Random _rnd { get { return RandomProvider.Instance; } }

        public IEnumerable<T> Shuffle<T>(IEnumerable<T> obj)
        {
            throw new NotImplementedException();
        }

        public T Select<T>(IEnumerable<T> obj, int times = 1)
        {
            throw new NotImplementedException();
        }

        public static T NextElement<T>(IEnumerable<T> obj)
            => obj.ElementAt(RandomProvider.Instance.Next(0, obj.Count()));

        public IEnumerable<T> SelectMany<T> (IEnumerable<T> obj, int times)
        {
            throw new NotImplementedException();
        }

        // should be diceresult
        public int Roll(Dice d, int times = 1)
        {
            /*
           
            // basic
            Math.Truncate( Random.Next(1, d.FaceCount * d._length) / d._length ) % d.FaceCount
            internal Range GetDiceRange()
             */

            int result = (int)(Math.Truncate((double)(_rnd.Next(1, d.FaceCount * d.Length) / d.Length)) % d.FaceCount);
            Console.WriteLine(result);
            return result;
        }

        public DiceResult RollMany(params (Dice, int)[] die)
        {
            throw new NotImplementedException();
        }

        public string GetColorHex(HexLengthType type = HexLengthType.Default)
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

        public OriColor GetColor()
        {
            throw new NotImplementedException();
        }

        public int Next(Range range)
        {
            throw new NotImplementedException();
        }
    }
}
