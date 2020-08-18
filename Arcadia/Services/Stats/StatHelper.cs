using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Orikivo;

namespace Arcadia
{
    public static class StatHelper
    {
        public static readonly Dictionary<string, Descriptor> Descriptions = new Dictionary<string, Descriptor>
        {
            [TickStats.CurrentLossStreak] = new Descriptor
            {
                Summary = "Increases the chance of winning by 1% for every 3 losses in **Doubler**."
            }
        };

        public static string SummaryOf(string id)
        {
            if (Descriptions.ContainsKey(id))
            {
                if (Check.NotNull(Descriptions[id]?.Summary))
                {
                    return Descriptions[id].Summary;
                }
            }

            return null;
        }

        public static void Clear(ArcadeUser user, params string[] stats)
        {
            foreach (string stat in stats)
                user.SetStat(stat, 0);
        }

        // set A to B if B is > than A
        public static void SetIfGreater(ArcadeUser user, string a, string b)
        {
            if (user.GetStat(b) > user.GetStat(a))
                user.SetStat(a, user.GetStat(b));
        }

        public static long GetOrAdd(ArcadeUser user, string a, long defaultValue)
        {
            SetIfEmpty(user, a, defaultValue);
            return user.GetStat(a);
        }

        public static void SetIfGreater(ArcadeUser user, string a, long b)
        {
            if (b > user.GetStat(a))
                user.SetStat(a, b);
        }

        public static void SetIfEmpty(ArcadeUser user, string stat, long value)
        {
            if (user.GetStat(stat) == 0)
                user.SetStat(stat, value);
        }

        public static void Swap(ArcadeUser user, string a, string b)
        {
            long u = user.GetStat(a);
            user.SetStat(a, user.GetStat(b));
            user.SetStat(b, u);
        }

        public static void SetIfLesser(ArcadeUser user, string a, string b)
        {
            if (user.GetStat(b) < user.GetStat(a))
                user.SetStat(a, user.GetStat(b));
        }

        public static void SetIfLesser(ArcadeUser user, string a, long b)
        {
            if (b < user.GetStat(a))
                user.SetStat(a, b);
        }

        // gets the diff between 2 stats
        public static long Difference(ArcadeUser user, string a, string b)
        {
            return user.GetStat(b) - user.GetStat(a);
        }

        public static long Sum(ArcadeUser user, string a, string b)
            => user.GetStat(a) + user.GetStat(b);

        public static DateTime AsTime(ArcadeUser user, string statId)
            => new DateTime(user.GetStat(statId));

        public static TimeSpan GetRemainder(ArcadeUser user, string statId, TimeSpan cooldown)
            => cooldown - SinceLast(user, statId);

        public static TimeSpan SinceLast(ArcadeUser user, string statId)
            => TimeSpan.FromTicks(DateTime.UtcNow.Ticks - user.GetStat(statId));

        public static TimeSpan SinceTime(ArcadeUser user, string statId, DateTime time)
            => TimeSpan.FromTicks(time.Ticks - user.GetStat(statId));

        public static bool HasName(string id)
            => Descriptions.ContainsKey(id) && Check.NotNull(Descriptions[id]?.Name);

        internal static string NameOf(string statId)
        {
            return HasName(statId) ? Format.Bold(Descriptions[statId].Name) : Format.LineCode(statId);
        }

        private static IEnumerable<KeyValuePair<string, long>> GetVisibleStats(ArcadeUser user)
            => user.Stats.Where((key, value) =>
                !key.StartsWith(StatGroups.Cooldown)
                && value != 0
                && !key.StartsWith(ItemHelper.Items.Select(x => x.Id)))
                .OrderBy(x => x.Key);

        private static int GetPageCount(ArcadeUser user, int pageSize)
            => (int) Math.Ceiling(GetVisibleStats(user).Count() / (double) pageSize);
        // 35 / 25 => 1.23
        public static string Write(ArcadeUser user, bool isSelf = true, int page = 0, int pageSize = 25)
        {
            var result = new StringBuilder();
            result.Append("> ⏱️ **Stats");

            if (!isSelf)
                result.Append($": {user.Username}");

            result.AppendLine("**");

            int pageCount = GetPageCount(user, pageSize) - 1;
            IEnumerable<KeyValuePair<string, long>> stats = GetVisibleStats(user);

            page = page < 0 ? 0 : page > pageCount ? pageCount : page;

            if ((pageCount + 1) > 1)
            {
                result.AppendLine($"> (Page **{page + 1:##,0}**/**{pageCount + 1:##,0}**)");
            }

            result.AppendLine();

            int offset = page * pageSize;
            int i = 0;
            foreach ((string id, long value) in stats.Skip(offset))
            {
                if (i >= pageSize)
                    break;

                result.AppendLine($"`{id}`: {value}");
                i++;
            }

            if (i == 0)
                result.Append("> There doesn't seem to be any visible stats here.");

            return result.ToString();
        }
    }

    /// <summary>
    /// Represents a generic implicitly defined value.
    /// </summary>
    public class Var
    {
        public const char Placeholder = '*';
        public const char Separator = ':';
        public const char TextSeparator = '_';

        public static readonly List<Var> Vars = new List<Var>
        {
            new Var
            {
                Id = GimiStats.CurrentWinStreak,
                UpperId = GimiStats.LongestWin,
            }
        };

        public static readonly List<string> Groups = new List<string>
        {
            "gimi"
        };

        public static string Debug(params string[] dummies)
        {
            var info = new StringBuilder();

            foreach (string dummy in dummies)
            {
                info.AppendLine($"Mock = {dummy}");
                bool valid = IsValid(dummy);
                bool validGroup = IsValid(dummy, true);
                bool result = TryParse(dummy, out string finalized);
                bool enforcedResult = TryParse(dummy, out string finalizedEnforce, true);
                info.AppendLine($"Valid? {valid} (Enforced? {validGroup})");
                info.AppendLine($"Parsed? {result} = {finalized ?? "N/A"} (Enforced? {enforcedResult} = {finalizedEnforce ?? "N/A"}");
                info.AppendLine($"Has Definition? {GetDefiner(dummy) != null}");

                if (!valid)
                {
                    info.AppendLine("Ending test, not valid...");
                    info.AppendLine();
                    continue;
                }

                info.AppendLine($"Template? {IsTemplate(dummy)}");
                info.AppendLine($"Defined Template? {IsDefinedTemplate(dummy)}");

                if (IsTemplate(dummy))
                {
                    info.AppendLine($"Template = {GetTemplateType(dummy)}");

                    if (!IsDefinedTemplate(dummy))
                    {
                        var test = SetTemplate(dummy, Items.PocketLawyer);
                        info.AppendLine($"Set Template = {test}");
                        info.AppendLine($"Humanized Set Template = {Humanize(test)}");
                    }
                }

                info.AppendLine($"Humanized = {Humanize(dummy)}");
                info.AppendLine($"Group = {GetGroup(dummy)}");
                info.AppendLine($"Key = {GetKey(dummy)}");
                info.AppendLine($"Default Value = {GetDefaultValue(dummy)}");
                info.AppendLine($"Type = {TypeOf(dummy)}");
                info.AppendLine();
            }

            if (info.Length == 0)
                return "No dummy strings were given.";

            return info.ToString();
        }

        public static string Humanize(string id)
        {
            if (!IsValid(id))
                throw new ArgumentException("Invalid ID specified");

            var text = new StringBuilder();
            string group = GetGroup(id);

            if (IsTemplate(id) && GetTemplateType(id) != TemplateType.Any)
            {
                text.Append(ItemHelper.Exists(group) ? ItemHelper.NameOf(group) : $"`{group}`");
            }
            else
            {
                text.Append(HumanizePartial(group));
            }

            text.Append($"{Separator} ");

            string key = GetKey(id);
            text.Append(HumanizePartial(key));

            return text.ToString();
        }

        private static string HumanizePartial(string input)
        {
            var reader = new StringReader(input);
            var text = new StringBuilder();

            int i = 0;
            bool upper = true;
            while (reader.CanRead())
            {
                char c = reader.Read();

                if (c == TextSeparator)
                {
                    text.Append(' ');
                    upper = true;
                }
                else
                {
                    char v = upper ? char.ToUpper(c) : c;

                    if (upper)
                        upper = false;

                    text.Append(v);
                }

                i++;
            }

            return text.ToString();
        }

        public static bool IsDefined(string id)
            => GetDefiner(id) != null;

        public static bool IsGroupDefined(string group)
            => Groups.Contains(group);

        public static Var GetDefiner(string id)
        {
            if (!IsValid(id))
                return null;

            // If the ID given is a defined template, replace it to its original counter part
            if (IsDefinedTemplate(id))
                id = id.Replace(GetGroup(id), Placeholder.ToString());

            return Vars.FirstOrDefault(x => x.Id == id);
        }

        public static long GetDefaultValue(string id)
        {
            return GetDefiner(id)?.DefaultValue ?? 0;
        }

        public static VarType TypeOf(string id)
        {
            return GetDefiner(id)?.Type ?? VarType.Stat;
        }

        public static string SetTemplate(string template, string value)
        {
            TemplateType type = GetTemplateType(template);

            if (type == TemplateType.Item)
            {
                if (!ItemHelper.Exists(value))
                    throw new ArgumentException("Expected an item ID but does not exist");
            }

            string group = GetGroup(template);

            return template.Replace(group, value);
        }

        public static TemplateType GetTemplateType(string template)
        {
            if (!IsValid(template))
                throw new ArgumentException("Invalid template ID specified");

            string group = GetGroup(template);

            if (group == Placeholder.ToString())
                return GetDefiner(template)?.Template ?? TemplateType.Any;

            if (Groups.Contains(group))
                throw new ArgumentException("Expected a template ID");

            if (ItemHelper.Exists(group))
                return TemplateType.Item;

            throw new ArgumentException("Unable to find an implicitly matching template type");
        }

        private static bool IsCharValid(char c)
        {
            return c >= '0' && c <= '9'
                   || c >= 'A' && c <= 'Z'
                   || c >= 'a' && c <= 'z'
                   || c == TextSeparator;
        }

        public static string GetGroup(string id)
        {
            if (!IsValid(id))
                return null;

            var reader = new StringReader(id);

            return reader.ReadUntil(Separator);
        }

        public static string GetKey(string id)
        {
            if (!IsValid(id))
                return null;

            var reader = new StringReader(id);

            reader.SkipUntil(Separator, true);

            return reader.GetRemaining();
        }

        public static bool IsDefinedTemplate(string id)
        {
            string group = GetGroup(id);

            if (group == null)
                throw new ArgumentException("Unable to validate the specified ID");

            if (group == Placeholder.ToString())
                return false;

            // If a defined group exists for this template, return false
            if (Groups.Contains(group))
                return false;

            // If this stat was defined as an ANY template and a template was replaced
            if (Vars.FirstOrDefault(x => x.Id == id)?.Template == TemplateType.Any)
                return true;

            // Otherwise, try to find an explicit match, and if none is found, a template does not exist
            return ItemHelper.Exists(group);
        }

        public static bool IsTemplate(string id)
        {
            string group = GetGroup(id);

            if (group == null)
                throw new ArgumentException("Unable to validate the specified ID");

            if (group == Placeholder.ToString())
                return true;

            // If a defined group exists for this template, return false
            if (Groups.Contains(group))
                return false;

            // If this stat was defined and a template is specified
            if (GetDefiner(id)?.Template != null)
                return true;

            // Otherwise, try to find an explicit match, and if none is found, a template does not exist
            return ItemHelper.Exists(group);
        }

        public static bool IsGroupValid(string group)
        {
            if (string.IsNullOrWhiteSpace(group))
                return false;

            if (group == Placeholder.ToString())
                return true;

            var reader = new StringReader(group);

            if (reader.CanRead() && reader.Peek() == TextSeparator)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Placeholder || c == Separator)
                    return false;
            }

            return true;
        }

        public static bool IsKeyValid(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            var reader = new StringReader(key);

            if (reader.CanRead() && reader.Peek() == TextSeparator)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Placeholder || c == Separator)
                    return false;
            }

            return true;
        }

        public static bool IsValid(string id, bool enforceGroup = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            id = id.Trim();

            if (!id.Contains(Separator))
                return false;

            var reader = new StringReader(id);

            if (reader.CanRead() && reader.Peek() == TextSeparator)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();
                Console.WriteLine($"{c} / {reader.CanRead()} / {(reader.CanRead() ? reader.Peek().ToString() : "N/A")}");
                if (c == Placeholder && reader.CanRead() && reader.Peek() != Separator)
                    return false;

                if (c == Separator)
                    break;

                if (!IsCharValid(c) && c != Placeholder)
                    return false;

                Console.WriteLine(c);
            }

            if (reader.GetCursor() == 1)
                return false;

            if (reader.GetRead() != Placeholder.ToString())
                if (enforceGroup && !IsGroupDefined(reader.GetRead()))
                    return false;

            int groupCursor = reader.GetCursor();

            if (reader.CanRead() && reader.Peek() == TextSeparator)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Separator)
                    return false;

                Console.WriteLine(c);
            }

            Console.WriteLine($"{reader.GetCursor()} / {groupCursor}");
            if (reader.GetCursor() - groupCursor == 0)
                return false;

            return true;
        }

        public static string Parse(string id, bool enforceGroup = false)
        {
            var current = new StringBuilder();

            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("The specified ID is null or empty.");

            id = id.Trim();

            if (!id.Contains(Separator))
                throw new ArgumentException("Expected a separator but is unspecified");

            var reader = new StringReader(id);

            if (reader.CanRead() && reader.Peek() == TextSeparator)
                throw new ArgumentException("Expected a normal character but returned as a text separator");

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (c == Placeholder)
                    reader.Expect(Separator, "Expected a separator after the placeholder");

                if (c == Separator)
                    break;

                if (!IsCharValid(c) && c != Placeholder)
                    throw new ArgumentException("Invalid character given when parsing the key");

                current.Append(c);
            }

            if (current.Length == 0)
                throw new ArgumentException("Expected a group to be specified");

            if (current.ToString() != Placeholder.ToString())
                if (enforceGroup && !IsGroupDefined(current.ToString()))
                    throw new ArgumentException("Could not find the specified group");

            current.Append(Separator);
            int groupLength = current.Length;

            if (reader.CanRead() && reader.Peek() == TextSeparator)
                throw new ArgumentException("Expected a normal character but returned as a text separator");

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Separator)
                    throw new ArgumentException("Invalid character given when parsing the key");

                current.Append(c);
            }

            if (current.Length - groupLength == 0)
                throw new ArgumentException("Expected a key to be specified");

            return current.ToString();
        }

        public static bool TryParse(string id, out string result, bool enforceGroup = false)
        {
            var current = new StringBuilder();
            result = null;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            id = id.Trim();

            if (!id.Contains(Separator))
                return false;

            var reader = new StringReader(id);


            if (reader.CanRead() && reader.Peek() == TextSeparator)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (c == Placeholder && reader.CanRead() && reader.Peek() != Separator)
                    return false;

                if (c == Separator)
                    break;

                if (!IsCharValid(c) && c != Placeholder)
                    return false;

                Console.WriteLine(c);
                current.Append(c);
            }

            if (current.Length == 0)
                return false;

            if (current.ToString() != Placeholder.ToString())
                if (enforceGroup && !IsGroupDefined(current.ToString()))
                    return false;

            current.Append(Separator);
            int groupLength = current.Length;

            if (reader.CanRead() && reader.Peek() == TextSeparator)
                return false;

            if (!reader.CanRead())
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Separator)
                    return false;

                Console.WriteLine(c);
                current.Append(c);
            }

            Console.WriteLine(current.ToString());
            if (current.Length - groupLength == 0)
                return false;

            result = current.ToString();
            return true;
        }

        internal Var() {}

        internal Var(string key, TemplateType template = TemplateType.Any, VarType type = VarType.Stat, long defaultValue = 0)
        {
            if (!IsKeyValid(key))
                throw new ArgumentException("Could not validate the specified key to a Var.");

            Id = $"{Placeholder}{Separator}{key}";
            Template = template;
            Type = type;
            DefaultValue = defaultValue;
        }

        internal Var(string id, VarType type = VarType.Stat, long defaultValue = 0, string upperId = null)
        {
            if (!IsValid(id))
                throw new ArgumentException("Could not validate the specified ID to a Var.");

            if (!string.IsNullOrWhiteSpace(upperId) && !IsValid(upperId))
                throw new ArgumentException("Could not validate the specified upper ID to a Var.");

            Id = id;
        }

        public string Id { get; private set; }

        // If this value is greater than the max_id, set the MaxId to this value
        // If unspecified, do nothing
        public string UpperId { get; private set; }

        public TemplateType? Template { get; private set; }

        public VarType Type { get; private set; } = VarType.Stat;

        public long DefaultValue { get; private set; } = 0;
    }

    public enum TemplateType
    {
        Any = 0,
        Item = 1
    }

    public enum VarType
    {
        Stat = 1,

        Time = 2,

        Attribute = 3
    }
}
