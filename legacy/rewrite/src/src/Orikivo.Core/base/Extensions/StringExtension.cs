using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Orikivo.Static;
using Orikivo.Utility;
using System.Linq;
using Discord;
using System.Drawing;
using Orikivo.Networking;
using Discord.Commands;
using System.Text;
using Discord.WebSocket;

namespace Orikivo
{
    public static class StringExtension
    {
        public static string GetRootModule(this CommandInfo command)
        {
            ModuleInfo module = command.Module;
            while (module.Parent.Exists())
            {
                module = module.Parent;
            }

            return module.Name;
        }

        public static string ToFullOriTime(this DateTime time)
            => time.ToString($"`yyyy`.`MM`.`dd` • `hh`:`mm`:`ss`{time.ToString("tt").ToSuperscript()}");

        public static string ToOriTime(this DateTime time)
            => time.ToString($"`hh`:`mm`:`ss`{time.ToString("tt").ToSuperscript()}");

        /// <summary>
        /// Returns a string that is cut from the first occurrence of the specified marker. 
        /// </summary>
        public static string Cut(this string s, string marker)
        {
            s.Debug("obj");
            marker.Debug("checker");
            if (s.Contains(marker))
            {
                int i = s.IndexOf(marker);
                i.Debug("marker index");
                s.Debug("before");
                // length - (length - marker index)
                s = s.Substring(0, s.Length - (s.Length - i));
                s.Debug("after");
            }

            return s;
        }

        public static bool HasMatchingName(this ParameterInfo p, string s)
            => p.Name.ToLower() == s.ToLower();

        public static bool IsProperUrl(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;
            Regex r = new Regex(@"https?://\w+");
            Match m = r.Match(s);
            if (m.Success)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Replaces a string in which multiple strings are replaced.
        /// </summary>
        public static string ReplaceMany(this string content, string replacement, params string[] substrings)
        {
            const string token = "\\[*]";
            foreach (string s in substrings)
                content = content.Replace(s, token);

            return content.Replace(token, replacement);
        }

        public static bool TryParseEmpty(this string s, out object empty)
        {
            empty = null;
            if (s == "null")
            {
                return true;
            }
            return false;
        }

        public static bool TryParseList(this string s, out List<object> list)
        {
            "parsing list".Debug();
            list = null;
            if (s.IsWrappedIn("[", "]"))
            {
                s = s.Unwrap("[", "]");
                list = new List<object>();
                if (s.Contains(','))
                {
                    "splitting list".Debug();
                    string[] data = s.Split(',');
                    foreach(string d in data)
                    {
                        d.Trim().Debug("item");
                        list.Add(d.Trim());
                    }
                }
                else
                {
                    list.Add(s);
                }

                list.Cast<string>().Conjoin().Debug();
                return true;
            }
            "parse failed".Debug();
            return false;
        }

        /// <summary>
        /// Returns if the guild channel specified is a voice channel.
        /// </summary>
        public static bool IsVoiceChannel(this SocketGuildChannel ch)
        {
            SocketGuild g = ch.Guild;
            if (g.VoiceChannels.Any(x => x.Id == ch.Id))
                return true;

            return false;
        }

        public static bool IsTextChannel(this SocketGuildChannel ch)
        {
            SocketGuild g = ch.Guild;
            if (g.TextChannels.Any(x => x.Id == ch.Id))
                return true;

            return false;
        }

        public static string Sanitize(this string s)
            => Regex.Unescape(s);

        public static string Clean(this string s)
            => s.Escape("||", "```", "`", "**_", "_**","*", "**", "__", "~~");

        // OLD, Replace w/ Stylize.
        public static string Pack(this Emoji e, OldAccount a)
        {
            return a.Config.Mobile ? $"{e}" : e.Escape();//.MarkdownLine();
        }

        public static bool IsAnyDeafened(this SocketGuildUser u)
            => u.IsDeafened || u.IsSelfDeafened;

        public static bool CanSpeak(this SocketGuildUser u)
            => !(u.IsAnyDeafened() && u.IsAnyMuted() && u.IsSuppressed);

        public static bool IsAnyMuted(this SocketGuildUser u)
            => u.IsMuted || u.IsSelfMuted;

        public static string GetName(this UserStatus status)
        {
            if (status.EqualsAny(UserStatus.AFK, UserStatus.Idle))
                return "Away";
            if (status == UserStatus.DoNotDisturb)
                return "Busy";
            if (status.EqualsAny(UserStatus.Offline, UserStatus.Invisible))
                return "Offline";

            return "Online";
        }

        public static bool HasEmojis(this string s, out List<string> matches)
        {
            s.Debug("content string");
            matches = new List<string>();

            const int EMOJI_NAME_LIMIT = 32;
            // emoji names must be at least 2 chars long, and must contain alphanumeric and underscores chars
            //2 - 32


            Regex pattern = new Regex(@"<a?:\w{2,32}:\d{1,20}>");
            pattern.ToString().Debug("pattern");
            MatchCollection m = pattern.Matches(s);
            if (m.Count == 0)
            {
                return false;
                
            }
            List<Match> M = m.ToList();
            foreach (Match x in M)
            {
                x.Value.Debug();
                matches.Add(x.Value);
            }
            return true;
        }

        public static bool HasEmoji(this string s, out string match)
        {
            s.Debug("content string");
            match = null;

            const int EMOJI_NAME_LIMIT = 32;
            // emoji names must be at least 2 chars long, and must contain alphanumeric and underscores chars
            //2 - 32


            Regex pattern = new Regex(@"<a?:\w{2,32}:\d{1,20}>");
            pattern.ToString().Debug("pattern");
            Match m = pattern.Match(s);
            if (!m.Success)
            {
                return false;
            }

            match = m.Value;
            return true;
        }

        public static bool TryParseEmoji(this string s)
        {
            if (s.ToCharArray().Any(x => !(x.Equals(' ')) && !x.IsEmojiSymbol() && !x.IsAlphanumeric() && !x.IsDigit()))
            {
                return false;
            }

            return true;
        }

        public static bool IsUpper(this char c)
            => c == c.ToUpper();

        public static bool IsLower(this char c)
            => c == c.ToLower();

        public static char ToLower(this char c)
            => char.ToLower(c);

        /// <summary>
        /// Flips the casing style of a character.
        /// </summary>
        public static char FlipCasing(this char c)
            => c.IsUpper() ? c.ToLower() : c.ToUpper();

        /// <summary>
        /// Returns an IEnumerable in which all characters in a string have been reversed by order.
        /// </summary>
        public static IEnumerable<char> ReverseEnumerate(this string s)
            => s.ToCharArray().Reverse();

        /// <summary>
        /// Returns a string in which all casing styles of each character is flipped.
        /// </summary>
        public static string FlipCasing(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s.ToList())
                sb.Append(c.FlipCasing());

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string in which the order of characters written have been reversed.
        /// </summary>
        public static string Reverse(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s.ReverseEnumerate())
                sb.Append(c.TryReverse());

            return sb.ToString();
        }

        public static bool IsDirectional(this char c)
        {
            var dct = UnicodeIndex.Directional;
            if (dct.Keys.Contains(c) || dct.Values.Contains(c))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to reverse a character if directional. Otherwise, returns itself.
        /// </summary>
        public static char TryReverse(this char c)
            => c.IsDirectional() ? c.Reverse() : c;

        /// <summary>
        /// Returns a character that resembles its opposite direction counterpart.
        /// </summary>
        public static char Reverse(this char c)
        {
            Dictionary<char, char> d = UnicodeIndex.Directional;
            if (d.Keys.Contains(c))
                return d[c];

            if (d.Values.Contains(c))
            {
                foreach (KeyValuePair<char, char> pair in d)
                    if (c == pair.Value)
                        return pair.Key;
            }
            return c;
        }

        /// <summary>
        /// Returns a character with an adjusted casing style based on a Boolean.
        /// </summary>
        public static char FlipCasing(this char c, bool b)
            => b ? c.ToUpper() : c.ToLower();

        // return a string that alternates in casing
        public static string AlternateCasing(this string s)
        {
            bool caps = s[0].IsUpper();
            StringBuilder sb = new StringBuilder();
            List<char> chrs = s.ToList();
            foreach (char c in chrs)
            {
                if (!c.IsLetter())
                {
                    sb.Append(c);
                    continue;
                }
                sb.Append(c.FlipCasing(caps));
                caps = !caps;
            }

            return sb.ToString();
        }

        // spaces a string out for n length.
        public static string Span(this string s, int length)
        {
            string spacing = " ".Drag(length);
            StringBuilder sb = new StringBuilder();
            List<char> chrs = s.ToList();
            foreach (char c in chrs)
            {
                sb.Append(c);
                if (!chrs.IsLastItem(c))
                {
                    sb.Append(spacing);
                }
            }

            return sb.ToString();
        }

        public static bool IsLastItem<T>(this List<T> list, T item)
            => list.IndexOf(item) == list.Count - 1;

        // true for descending, false for ascending
        public static char[] SortCharacters(this string s, bool direction = true)
        {
            char[] chrs = s.ToCharArray();
            if (direction)
            {
                return chrs.OrderBy(x => x.ToString()).ToArray();
            }
            return chrs.OrderByDescending(x => x.ToString()).ToArray();
        }

        public static int GetLength(this EmbedFieldBuilder field)
        {
            return field.Name.Length + field.Value.ToString().Length;
        }

        public static IEnumerable<ModuleInfo> Parents(this IEnumerable<ModuleInfo> modules)
            => modules.Where(x => !x.IsSubmodule);

        public static string GetSummary(this ModuleInfo module)
            => module.Summary ?? "Empty.";

        public static void AddMany<T>(this List<T> main, params T[] items)
        {
            foreach (T item in items)
                main.Add(item);
        }

        public static List<T> Merge<T>(this List<T> main, params List<T>[] lists)
        {
            foreach (List<T> list in lists)
            {
                if (!list.Exists())
                    continue;

                main.AddRange(list);
            }

            return main;
        }

        /// <summary>
        /// Returns a total counter from a parameter collection of lists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static long GetTotalCount<T>(this IEnumerable<T>[] e)
        {
            long i = 0;
            foreach (IEnumerable<T> l in e)
                i += l.Count();
            
            return i;
        }

        public static string MarkdownSpoiler(this string s)
            => s.Wrap("||");


        /// <summary>
        /// Returns a string in which all of its characters are replaced by a specified value.
        /// </summary>
        public static string ReplaceAll(this string s, char c)
            => c.Drag(s.Length);

        public static string Censor(this string s, WordGuardControl control)
        {
            if (control == WordGuardControl.Inactive)
                return s;

            if (control == WordGuardControl.Careful)
                return s.MarkdownSpoiler();

            if (control == WordGuardControl.Controlled)
                return s[0] + s.Censor().Substring(1);

            return s.Censor();
        }

        /// <summary>
        /// Returns a string with a censored symbol spanning the same size.
        /// </summary>
        public static string Censor(this string s)
        {
            string[] args = s.Words();
            if (args.Length == 1)
                return s.ReplaceAll('*');

            for (int i = 0; i < args.Length; i++)
                args[i] = args[i].ReplaceAll('*');

            return args.Conjoin(" ");
        }

        /// <summary>
        /// Returns an array of strings split by a space.
        /// </summary>
        public static string[] Words(this string s)
            => s.Split(' ');

        /// <summary>
        /// Returns a censored string based off a collection of blacklisted words.
        /// </summary>
        public static string Guard(this string s, List<string> blacklist)
        {
            string[] args = s.Words();
            if (args.Length == 1)
                return s.Censor();

            for (int i = 0; i < args.Length; i++)
                if (args[i].EqualsAny(blacklist))
                    args[i] = args[i].Censor();

            return args.Conjoin(" ");
        }

        public static string Drag(this string s, int length)
        {
            StringBuilder sb = new StringBuilder();
            char[] d = s.ToCharArray();

            for (int x = 0; x < d.Length; x++)
                sb.Append(d[x].Drag(length));

            return sb.ToString();
        }

        public static string Drag(this char c, int length)
        {
            char[] s = new char[length];
            for(int i = 0; i < length; i++)
            {
                s[i] = c;
            }

            return new string(s);
        }

        public static Bitmap Render(this char c, PixelRenderingOptions options)
            => PixelEngine.RenderChar(c, options);

        public static char[] GetRenderMap(this char[] map)
            => map.Where(x => !x.EqualsAny(' ', '\n', '⠀'/*Invisible Braille*/)).ToArray();

        /// <summary>
        /// Returns the larged integer in a collection.
        /// </summary>
        public static int GetLargest(this IEnumerable<int> e)
            => e.OrderByDescending(x => x).First();
        
        public static Size GetRenderBox(this string s, FontFace font)
        {
            int letterwidth = font.Ppu.Width + font.Padding;
            int letterspace = font.Spacing;
            int letterheight = font.Ppu.Height + font.Padding;
            List<int> width = new List<int>();

            char[] map = s.ToCharArray();
            if (map.Any(u => u.Overhangs()))
            {
                //letterheight += font.Overhang;
            }

            int b = 0; // overhang extender.
            int x = 0; // font width
            int y = 1; // font height
            foreach (char c in map)
            {
                if (font.HasFallback.HasValue)
                    if (!font.HasFallback.Value)
                        if (!font.Search(font.GetDefault(), c, out (int i, int x, int y) index).Exists())
                            continue;

                if (c.Overhangs())
                {
                    b += 1;
                }
                if (c == '\n')
                {
                    c.Debug("Line Break");
                    width.Add(x);
                    x = 0;
                    y += 1;
                    continue;
                }

                if (c == ' ')
                {
                    c.Debug("Space");
                    x += letterspace;
                    continue;
                }

                if (c == '⠀')
                {
                    c.Debug("Braille Clear Block");
                    x += 4; // Default Braille Length.
                    continue;
                }

                c.Debug("normal letter");
                x += letterwidth;
            }
            width.Add(x);

            letterheight.Debug($"* {y}");
            letterheight *= y;

            letterheight += (font.Overhang * b);

            letterwidth = width.GetLargest();
            return new Size(letterwidth, letterheight);
        }

        public static bool Overhangs(this char c)
            => c.EqualsAny('q', 'p', 'y', 'j', 'g');

        public static Bitmap Render(this string s, PixelRenderingOptions options)
            => PixelEngine.RenderString(s, options);

        public static Bitmap Render(this string s, int padding, PixelRenderingOptions options, ulong? fontId = null)
            => PixelEngine.RenderString(s, options, fontId, padding);

        public static EmbedBuilder ToEmbedDescription(this string s, EmbedBuilder e = null) =>
            (e ?? new EmbedBuilder()).WithDescription(s);

        public static EmbedBuilder ToEmbedTitle(this string s, EmbedBuilder e = null) =>
            (e ?? new EmbedBuilder()).WithTitle(s);

        public static EmbedBuilder ToEmbedAuthor(this string s, EmbedBuilder e = null) =>
            (e ?? new EmbedBuilder()).WithAuthor(s.ToEmbedAuthor(e.Author));

        public static EmbedAuthorBuilder ToEmbedAuthor(this string s, EmbedAuthorBuilder a = null) =>
            (a ?? new EmbedAuthorBuilder()).WithName(s);

        public static EmbedBuilder ToEmbedFooter(this string s, EmbedBuilder e = null) =>
            (e ?? new EmbedBuilder()).WithFooter(s.ToEmbedFooter(e.Footer));

        public static EmbedFooterBuilder ToEmbedFooter(this string s, EmbedFooterBuilder f = null) =>
            (f ?? new EmbedFooterBuilder()).WithText(s);
        
        public static string DiscordLine(this string s) =>
            s.Wrap(UnicodeIndex.LineMarker);

        public static string DiscordBlock(this string s) =>
            s.Wrap(UnicodeIndex.CodeMarker);

        //(MarkdownIndex.Contains(type) ? type : MarkdownIndex.Default)
        public static string DiscordBlock(this string s, string type = null) =>
            $"{type}\n{s}".Wrap(UnicodeIndex.CodeMarker);
        
        public static string MarkdownItalics(this string s)
        {
            string x = s.Wrap(UnicodeIndex.ItalicMarker);
            return Options.EscapeMarkdown ? x.Escape('*') : x;
        }

        public static string MarkdownBold(this string s)
        {
            string x = s.Wrap(UnicodeIndex.BoldMarker);
            return Options.EscapeMarkdown ? x.Escape('*') : x;
        }

        public static string MarkdownBoldItalics(this string s)
        {
            string x = s.Wrap(UnicodeIndex.BoldItalicOpeningMarker, UnicodeIndex.BoldItalicClosingMarker);
            return Options.EscapeMarkdown ? x.Escape('_', '*') : x;
        }

        public static string ToItalics(this string s, CaseFormat c = CaseFormat.Ignore)
            => s.CaseAs(c).ToUnicodeString(UnicodeIndex.Italics);

        public static string ToBold(this string s, CaseFormat c = CaseFormat.Ignore)
            => s.CaseAs(c).ToUnicodeString(UnicodeIndex.Bold);

        public static string ToSuperscript(this string s, CaseFormat c = CaseFormat.Ignore)
            => s.CaseAs(c).ToUnicodeString(UnicodeIndex.Superscripts);

        public static string ToSubscript(this string s, CaseFormat c = CaseFormat.Ignore)
            => s.CaseAs(c).ToUnicodeString(UnicodeIndex.Subscripts);

        public static string FromItalics(this string s) =>
            s.FromUnicodeString(UnicodeIndex.Italics);

        public static string FromBold(this string s) =>
            s.FromUnicodeString(UnicodeIndex.Bold);

        public static string FromSuperscript(this string s) =>
            s.FromUnicodeString(UnicodeIndex.Superscripts);

        public static string FromSubscript(this string s) =>
            s.FromUnicodeString(UnicodeIndex.Subscripts);

        public static string FromAny(this string s) =>
            s.FromAnyUnicodeString();

        public static int IndexAsLength(this string s) =>
            s.Length + 1;
        
        public static int LengthAsIndex(this string s) =>
            s.Length - 1;

        private static string ToUnicodeString(this string s, Dictionary<char, string> lib)
        {
            string r = "";
            List<string> l = s.ToUnicodeList(lib);
            string m = GetMarker(lib);
            string p = "";
            string n = "";
            bool open = false;
            if (Options.DiscordSubstitution && m != null)
            {
                foreach (string i in l)
                {
                    n = i != null ? i.Unescape() : "";
                    if (p.IsWrappedIn(m))
                    {
                        if (open)
                        {
                            if (n.IsWrappedIn(m))
                            {
                                p = p.Unwrap(m);
                            }
                            else
                            {
                                p = p.UnwrapStart(m);
                                open = false;
                            }
                        }
                        else
                        {
                            if (n.IsWrappedIn(m))
                            {
                                p = p.UnwrapEnd(m);
                                open = true;
                            }
                        }
                    }
                    r += p;
                    p = n;
                }
                if (p != "")
                {
                    if (open)
                    {
                        p = p.UnwrapStart(m);
                        open = false;
                    }
                    r += p;
                }
                return Options.EscapeMarkdown ? r.Escape('*') : r;
            }
            else return l.Conjoin();
        }

        /// <summary>
        /// Conjoins a string collection, with an optional seperator.
        /// </summary>
        public static string Conjoin(this IEnumerable<string> l, string seperator = "")
            => string.Join(seperator, l);

        private static string FromUnicodeString(this string s, Dictionary<char, string> lib)
        {
            string r = "";
            List<string> unicodeList = s.FromUnicodeList();
            foreach (string u in unicodeList) r += u.FromUnicodeCharacter(lib);
            return r;
        }

        private static string FromAnyUnicodeString(this string s)
        {
            string r = "";
            List<string> l = s.FromUnicodeList();
            foreach (string u in l) r += u.FromAnyUnicodeCharacter();
            return r;
        }

        private static List<string> ToUnicodeList(this string s, Dictionary<char, string> lib)
        {
            List<string> r = new List<string>();
            foreach (char c in s) r.Add(c.ToUnicodeCharacter(lib));
            return r;
        }

        private static List<string> FromUnicodeList(this string s)
        {
            List<string> r = new List<string>();
            do
            {
                foreach (Dictionary<char, string> lib in UnicodeDictionary.All)
                {
                    foreach(KeyValuePair<char, string> pair in lib)
                    {
                        string x = pair.Value;
                        if (s.StartsWith(x))
                        {
                            r.Add(x);
                            s = s.Substring(x.Length);
                            continue;
                        }
                    }
                }
                foreach (List<char> lib in UnicodeDictionary.Defaults)
                {
                    foreach (char u in lib)
                    {
                        string x = u.ToString();
                        if (s.StartsWith(x))
                        {
                            r.Add(x);
                            s = s.Substring(x.Length);
                            continue;
                        }
                    }
                }
            } while (s.Length > 0);
            return r;
        }

        public static int Gap(this string s, string x) =>
            s.Length - x.Length;

        public static int Gap(this string s, char c) =>
            s.Length - 1;

        public static string GetMarker(Dictionary<char, string> lib)
        {
            if (lib.Equals(UnicodeIndex.Italics))
                return UnicodeIndex.ItalicMarker;

            if (lib.Equals(UnicodeIndex.Bold))
                return UnicodeIndex.BoldMarker;

            else
                return null;
        }

        public static string ToMarker(this string s, Dictionary<char, string> lib)
        {
            if (s.Equals(UnicodeIndex.Space.ToString()))
                return s;

            if (lib.Equals(UnicodeIndex.Italics))
                s = s.MarkdownItalics();
            
            else if (lib.Equals(UnicodeIndex.Bold))
                s = s.MarkdownBold();
            
            return s;
        }

        public static string FromMarker(this string s)
        {
            s = s.Unescape();

            if (s.IsWrappedIn(UnicodeIndex.BoldItalicOpeningMarker, UnicodeIndex.BoldItalicClosingMarker))
                s = s.Unwrap(UnicodeIndex.BoldItalicOpeningMarker, UnicodeIndex.BoldItalicClosingMarker);

            else if (s.IsWrappedIn(UnicodeIndex.BoldMarker))
                s = s.Unwrap(UnicodeIndex.BoldMarker);

            else if (s.IsWrappedIn(UnicodeIndex.ItalicMarker))
                s = s.Unwrap(UnicodeIndex.ItalicMarker);

            return s;
        }

        public static string ReplaceStart(this string s, char c) =>
            c + s.Substring(1);

        public static string ReplaceEnd(this string s, char c) =>
            s.Substring(0, s.Gap(c)) + c;

        public static string EscapeString(this string s)
            => $"\\{s}";

        public static string Escape(this string s)
        {
            string r = "";

            foreach (char c in s)
                r += c.Escape();

            return r;
        }

        public static string Escape(this string s, char arg)
        {
            string r = "";

            foreach (char c in s)
                r += c == arg ? c.Escape() : $"{c}";

            return r;
        }

        public static string Escape(this string s, params string[] args)
        {
            foreach(string x in args)
            {
                s.Replace(x, x.EscapeString());
            }
            return s;
        }

        public static string Escape(this string s, List<char> args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
                sb.Append(c.EqualsAny(args) ? c.Escape() : $"{c}");

            return sb.ToString();
        }

        public static bool EndsWithAny(this string s, params string[] args) =>
            args.Any(x => s.EndsWith(x));

        public static bool IsEmpty(this string s)
            => s == string.Empty;

        public static string Escape(this string s, params char[] args)
            => s.Escape(args.ToList());

        public static string Unescape(this string s)
            => s.Replace("\\", "");

        public static string TryUnwrap(this string s, string fragment) =>
            s.IsWrappedIn(fragment) ? s.Unwrap(fragment) : s;

        public static string MarkdownLink(this string s, string url)
            => $"[{s}]({url})";

        public static string TryParseGyazo(this string s)
        {
            const string GYAZO_GIF_URL = "https://i.gyazo.com/{0}.gif";
            Regex r = new Regex(@"https?://gyazo.com/(?<id>\w+)");
            Match m = r.Match(s);

            if (m.Success)
            {
                s = string.Format(GYAZO_GIF_URL, m.Groups["id"].Value);
            }
            return s;
        }

        public static bool ContainsAny(this string s, IEnumerable<string> args)
            => args.Any(x => s.Contains(x));

        public static bool ContainsAny(this string s, params string[] args)
            => args.Any(x => s.Contains(x));

        public static string TryUnwrap(this string s, string opener, string closer)
            => s.IsWrappedIn(opener, closer) ? s.Unwrap(opener, closer) : s;

        public static bool IsWrappedIn(this string s, string fragment)
            => s.StartsWith(fragment) && s.EndsWith(fragment);

        public static bool IsWrappedIn(this string s, string opener, string closer)
            => s.StartsWith(opener) && s.EndsWith(closer); 

        public static string Wrap(this string s, string x)
            => x + s + x;

        public static string Wrap(this string s, string a, string z)
            => a + s + z;

        public static string UnwrapStart(this string s, string x) =>
            s.Substring(s.StartsWith(x) ? x.Length : 0);

        public static string UnwrapEnd(this string s, string x) =>
            s.Substring(0, s.EndsWith(x) ? s.Gap(x) : s.Length);

        public static string Unwrap(this string s, string x) =>
            s.UnwrapStart(x).UnwrapEnd(x);

        public static string Unwrap(this string s, string a, string z) =>
            s.UnwrapStart(a).UnwrapEnd(z);

        public static string Emojify(this string s, bool large = true)
        {
            s = s.ToLower();
            string r = "";

            foreach (char c in s)
            {   
                if (c.Equals(UnicodeIndex.Space))
                    r += large? UnicodeIndex.LargeEmojiSpacer : UnicodeIndex.SmallEmojiSpacer;

                else
                    r += c.EmojifyCharacter(large);
            }

            return large ? r : UnicodeIndex.Invisible + r;
        }
    }
}