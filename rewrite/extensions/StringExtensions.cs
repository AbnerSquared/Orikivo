using System;
using System.Collections.Generic;
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
    }
}
