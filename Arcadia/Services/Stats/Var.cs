using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Services;
using Orikivo;

namespace Arcadia
{
    public class VarGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public Func<ArcadeUser, string> Writer { get; set; }
    }

    /// <summary>
    /// Represents a generic implicitly defined value.
    /// </summary>
    public class Var
    {
        public const char Placeholder = '*';
        public const char Separator = ':';
        public const char TextSeparator = '_';

        public static readonly List<Var> Definers = new List<Var>
        {
            new Var
            {
                Id = GimiStats.CurrentWinStreak,
                UpperId = GimiStats.LongestWin,
                Summary = "This represents your current winning streak in **Gimi**."
            },
            new Var
            {
                Id = Stats.QuestCapacity,
                Name = "Quest Capacity",
                Summary = "This determines how many quests you can receive at a time."
            },
            new Var
            {
                Id = Vars.BoosterLimit,
                Name = "Booster Stack Limit",
                Summary = "This determines how many boosters you can stack at a time."
            },
            new Var
            {
                Id = Vars.Capacity,
                Name = "Inventory Capacity",
                Summary = "This determines how many boosters you can stack at a time.",
                DefaultValue = 4000,
                ValueWriter = Inventory.WriteCapacity
            }
        };

        public static readonly List<VarGroup> Groups = new List<VarGroup>
        {
            new VarGroup
            {
                Id = "gimi",
                Name = "Gimi",
                Summary = "This is a group of stats used for the **Gimi** casino machine.",
                Writer = (user =>
                {
                    var details = new StringBuilder();






                    return details.ToString();
                })
            }
        };

        public static readonly List<string> GroupIds = new List<string>
        {
            "gimi"
        };

        public static int Count(ArcadeUser user)
        {
            return user.Stats.Count;
        }

        public static string ViewDetails(ArcadeUser user, string id)
        {
            if (!user.Stats.ContainsKey(id))
                return $"> {Icons.Warning} Unknown stat specified.";

            var details = new StringBuilder();

            string name = GetDefiner(id)?.Name ?? Humanize(id);
            string value = GetDefiner(id)?.ValueWriter?.Invoke(user.GetVar(id)) ?? $"{user.GetVar(id)}";

            if (!string.IsNullOrWhiteSpace(name))
            {
                details.AppendLine($"`{id}`");
                details.AppendLine($"• **{name}** = {value}");
            }
            else
            {
                details.AppendLine($"• `{id}` = {value}");
            }

            string summary = GetDefiner(id)?.Summary;

            if (!string.IsNullOrWhiteSpace(summary))
            {
                details.AppendLine($"> {summary}");
            }

            return details.ToString();
        }

        public static string ViewGroupDetails(ArcadeUser user, string groupId)
        {
            VarGroup group = Groups.FirstOrDefault(x => x.Id == groupId);

            var details = new StringBuilder();

            return details.ToString();
        }

        public static IEnumerable<string> WithGroup(ArcadeUser user, string group)
        {
            return user.Stats.Keys.Where(x => EqualsGroup(x, group));
        }

        public static IEnumerable<string> WithKey(ArcadeUser user, string key)
        {
            return user.Stats.Keys.Where(x => EqualsKey(x, key));
        }

        public static bool EqualsGroup(string id, string group)
        {
            return GetGroup(id) == group;
        }

        public static bool EqualsKey(string id, string key)
        {
            return GetKey(id) == key;
        }

        public static long Get(ArcadeUser user, string id)
        {
            return user.GetVar(id);
        }

        public static void AddTo(ArcadeUser user, string id, long amount = 1)
        {
            user.AddToVar(id, amount);
        }

        public static void Set(ArcadeUser user, string id, DateTime time)
        {
            user.SetVar(id, time.Ticks);
        }

        public static long GetOrSet(ArcadeUser user, string id, long defaultValue)
        {
            SetIfEmpty(user, id, defaultValue);
            return user.GetVar(id);
        }

        public static void SetIfEmpty(ArcadeUser user, string id, long value)
        {
            if (user.GetVar(id) == 0)
                user.SetVar(id, value);
        }

        public static void SetIfGreater(ArcadeUser user, string a, string b)
        {
            if (user.GetVar(b) > user.GetVar(a))
                user.SetVar(a, user.GetVar(b));
        }

        public static void SetIfLesser(ArcadeUser user, string a, string b)
        {
            if (user.GetVar(b) < user.GetVar(a))
                user.SetVar(a, user.GetVar(b));
        }

        public static void Swap(ArcadeUser user, string a, string b)
        {
            user.SetVar(a, user.GetVar(b), out long previous);
            user.SetVar(b, previous);
        }

        public static long Sum(ArcadeUser user, string a, string b)
        {
            return user.GetVar(a) + user.GetVar(b);
        }

        public static long Sum(ArcadeUser user, string a, string b, params string[] rest)
        {
            return Sum(user, a, b) + rest.Sum(user.GetVar);
        }

        public static string Min(ArcadeUser user, string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                return "";

            if (string.IsNullOrWhiteSpace(a))
                return b;

            if (string.IsNullOrWhiteSpace(b))
                return a;

            if (user.GetVar(a) == user.GetVar(b))
                return "";

            return user.GetVar(a) < user.GetVar(b) ? a : b;
        }

        public static string Min(ArcadeUser user, string a, string b, params string[] rest)
        {
            string min = Min(user, a, b);

            foreach (string id in rest)
                min = Min(user, min, id);

            return min;
        }

        public static string Max(ArcadeUser user, string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                return "";

            if (string.IsNullOrWhiteSpace(a))
                return b;

            if (string.IsNullOrWhiteSpace(b))
                return a;

            if (user.GetVar(a) == user.GetVar(b))
                return "";

            return user.GetVar(a) > user.GetVar(b) ? a : b;
        }

        public static string Max(ArcadeUser user, string a, string b, params string[] rest)
        {
            string max = Max(user, a, b);

            foreach (string id in rest)
                max = Max(user, max, id);

            return max;
        }

        public static void Reset(ArcadeUser user, string id)
        {
            user.SetVar(id, GetDefaultValue(id));
        }

        public static void Reset(ArcadeUser user, params string[] ids)
        {
            foreach (string id in ids)
                Reset(user, id);
        }

        public static void Clear(ArcadeUser user, string id)
        {
            user.SetVar(id, 0);
        }

        public static void Clear(ArcadeUser user, params string[] ids)
        {
            foreach (string id in ids)
                Clear(user, id);
        }

        //public static void ClearAll(ArcadeUser user)
        //{
        //    user.Stats = new Dictionary<string, long>();
        //}

        public static bool IsType(string id, VarType type)
        {
            return TypeOf(id) == type;
        }

        public static bool MeetsCriterion(ArcadeUser user, VarCriterion criterion)
        {
            return user.GetVar(criterion.Id) >= criterion.ExpectedValue;
        }

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
                return "";
                // throw new ArgumentException("Invalid ID specified");

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
            => GroupIds.Contains(group);

        public static Var GetDefiner(string id)
        {
            if (!IsValid(id))
                return null;

            // If the ID given is a defined template, replace it to its original counter part
            if (IsDefinedTemplate(id))
                id = id.Replace(GetGroup(id), Placeholder.ToString());

            return Definers.FirstOrDefault(x => x.Id == id);
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

            if (GroupIds.Contains(group))
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
            if (GroupIds.Contains(group))
                return false;

            // If this stat was defined as an ANY template and a template was replaced
            if (Definers.FirstOrDefault(x => x.Id == id)?.Template == TemplateType.Any)
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
            if (GroupIds.Contains(group))
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
                if (c == Placeholder && reader.CanRead() && reader.Peek() != Separator)
                    return false;

                if (c == Separator)
                    break;

                if (!IsCharValid(c) && c != Placeholder)
                    return false;
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
            }

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

                current.Append(c);
            }

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

        public string Name { get; internal set; }

        // If this value is greater than the max_id, set the MaxId to this value
        // If unspecified, do nothing
        public string UpperId { get; private set; }

        public string Summary { get; internal set; }

        public Func<long, string> ValueWriter { get; internal set; }

        public TemplateType? Template { get; private set; }

        public VarType Type { get; private set; } = VarType.Stat;

        public long DefaultValue { get; private set; } = 0;
    }
}