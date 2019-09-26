using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orikivo.Utility
{
    public static class Calculator
    {
        public static double RangeShift(int value, int oldMin, int oldMax, int newMin, int newMax)
            => (((value - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
            
        public static double RangeShift(int value, Range from, Range to)
            => (((value - from.Min) * to.Length) / from.Length) + to.Min;

        public static int Add(params int[] values)
        {
            int result = 0;
            foreach (int i in values)
                result += i;
            return result;
        }

        public static int Subtract(params int[] values)
        {
            if (values.Length == 0)
                return 0;

            int result = values[0];
            foreach (int i in values.Skip(1))
                result -= i;
            return result;
        }

        public static double Derive(double b, double x, double n)
            => (b*n)*Math.Pow(x, n-1);

        public static double Integrate(double b, double x, double n)
            => (b/(n+1))*Math.Pow(x, n+1);

        public static double Solve(string expression)
        {
            string pattern = @"(\([A-za-z0-9\^\+\*\-/]+\)|[A-za-z0-9\^\+\*\-/]+)";
            Regex regex = new Regex(pattern);
            pattern.Debug();
            if (regex.IsMatch(expression))
            {
                Debugger.Write("-- Expression found. --");
                Match match = regex.Match(expression);
                match.Groups.ForEach(x => x.Debug($"Index {x.Index}; Length {x.Length}"));
                IEnumerable<string> values = match.Groups.Enumerate(x => x.Value).Skip(1);
                foreach (string value in values)
                {
                    string s = value.TryUnwrap("(", ")");
                    s.Debug();
                    string pattern3 = @"([/\+\-\*\^]{1})";
                    string pattern4 = $"{pattern}{pattern3}{pattern}";
                    Regex regex4 = new Regex(pattern4);
                    pattern4.Debug();

                    if (regex4.IsMatch(s))
                    {
                        Debugger.Write("-- Values found. --");
                        Match match4 = regex4.Match(s);
                        match4.Groups.ForEach(x => x.Debug($"Index {x.Index}; Length {x.Length}"));
                        IEnumerable<string> values4 = match4.Groups.Enumerate(x => x.Value).Skip(1);
                        foreach(string value4 in values4)
                        {

                        }

                    }
                }

                return 0;
            }
            Debugger.Write("-- Evaluation failed; Expression not found. --");
            return 0;
        }

        /* -- Expression calculation. --
         * [^/*+-] : symbols
           (...) : capturing group; solve these first before stepping back.
           x^n : x to the power of n
           x*n : x times n
           x/n : x divided by n
           x+n : x plus n
           x-n : x minus n
           root(n,x) : n root of x
           sqrt(x) : square root of x
           cbrt(x) : cubic root of x
           derive(x) : derivative of x
           integral(x) : integral of x
           x; n : expression break. solves both.
           x = n : variable supplier. using x from this point on will be n.
           [0,0] : inner expression placeholder; used to simplify equations before resolving.

            1+x
         */
    }
}