using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class StringExtensions
    {
        public static string Escape(this string s)
            => $"\\{s}";
        public static string Escape(this string s, IEnumerable<string> args)
        {
            args.ForEach(x => s = s.Replace(x, x.Escape()));
            return s;
        }
        public static string Escape(this string s, IEnumerable<char> args)
        {
            args.ForEach(c => s = s.Replace(c.ToString(), c.ToString().Escape()));
            return s;
        }
        public static string Escape(this string s, params char[] args)
            => Escape(s, args.ToList());
        public static string Escape(this string s, params string[] args)
            => Escape(s, args.ToList());

        public static string Remove(this string s, params string[] args)
            => Remove(s, args.ToList());
        public static string Remove(this string s, params char[] args)
            => Remove(s, args.ToList());
        public static string Remove(this string s, IEnumerable<char> args)
        {
            args.ForEach(c => s = s.Replace(c.ToString(), string.Empty));
            return s;
        }
        public static string Remove(this string s, IEnumerable<string> args)
        {
            args.ForEach(x => s = s.Replace(x, string.Empty));
            return s;
        }
    }
}
