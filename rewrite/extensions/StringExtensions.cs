using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class StringExtensions
    {
        public static string Escape(this string s, params string[] args)
        {
            foreach (string x in args)
                s = s.Replace(x, $"\\{x}");
            return s;
        }

        public static string Escape(this string s, params char[] args)
        {
            foreach (char c in args)
                s = s.Replace($"{c}", $"\\{c}");
            return s;
        }

        public static string Escape(this string s, IEnumerable<char> args)
            => Escape(s, args.ToArray());
        public static string Escape(this string s, IEnumerable<string> args)
            => Escape(s, args.ToArray());

        public static string Remove(this string s, IEnumerable<string> args)
            => Remove(s, args.ToArray());
        public static string Remove(this string s, IEnumerable<char> args)
            => Remove(s, args.ToArray());
        public static string Remove(this string s, params char[] args)
        {
            foreach (char c in args)
                s = s.Replace($"{c}", string.Empty);
            return s;
        }

        public static string Remove(this string s, params string[] args)
        {
            foreach (string x in args)
                s = s.Replace(x, string.Empty);
            return s;
        }
    }
}
