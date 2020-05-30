namespace Orikivo
{
    public static class StringExtensions
    {
        // attempt to write down the strings content; otherwise string.empty.
        public static string ValueOrDefault(this string s)
            => Check.NotNull(s) ? s : "";

        public static string ValueOrDefault(this string s, string fallback)
            => Check.NotNull(s) ? s : fallback;

        public static bool ContainsAny(this string s, params string[] values)
        {
            foreach (string value in values)
            {
                if (s.Contains(value))
                    return true;
            }

            return false;
        }

        public static bool ContainsAny(this string s, params char[] values)
        {
            foreach (char c in values)
            {
                if (s.Contains(c))
                    return true;
            }

            return false;
        }

        public static bool StartsWithAny(this string s, params string[] values)
        {
            foreach (string value in values)
                if (s.StartsWith(value))
                    return true;

            return false;
        }

        public static bool EndsWithAny(this string s, params string[] values)
        {
            foreach (string value in values)
                if (s.EndsWith(value))
                    return true;

            return false;
        }

        public static string Escape(this string s)
            => $"\\{s}";

        public static string Escape(this string s, params char[] args)
        {
            args.ForEach(c => s = s.Replace(c.ToString(), c.ToString().Escape()));
            return s;
        }

        public static string Escape(this string s, params string[] args)
        {
            s.EndsWith("a");
            args.ForEach(x => s = s.Replace(x, x.Escape()));
            return s;
        }

        public static string Remove(this string s, params string[] args)
        {
            args.ForEach(x => s = s.Replace(x, string.Empty));
            return s;
        }

        public static string Remove(this string s, params char[] args)
        {
            args.ForEach(c => s = s.Replace(c.ToString(), string.Empty));
            return s;
        }
        
    }
}
