using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Services;
using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Represents a generic implicitly defined value.
    /// </summary>
    public class Var
    {
        public const char Placeholder = '*';
        public const char Separator = ':';
        public const char TextSeparator = '_';

        private static string WriteDefault(long value, VarType type = VarType.Stat)
        {
            return type switch
            {
                VarType.Time => Format.FullTime(new DateTime(value)),
                VarType.Attribute => $"{value:##,0}",
                VarType.Stat => $"{value:##,0}",
                _ => value.ToString()
            };
        }

        public static readonly List<Var> Definers = new List<Var>
        {
            new Var
            {
                Id = "items:total_discovered",
                Summary = "This represents all of your seen and known items discovered.",
                Type = VarType.Stat,
                ValueGetter = u => u.Stats.Count(x => x.Key.StartsWith("catalog:") && x.Value > 0)
            },
            new Var
            {
                Id = GimiStats.TimesLost,
                Summary = "This represents all of the times you have lost in **Gimi**.",
                Type = VarType.Stat,
                ValueGetter = u => Difference(u, GimiStats.TimesWon, GimiStats.TimesPlayed)
            },
            new Var
            {
                Id = TickStats.TimesLost,
                Summary = "This represents all of the times you have lost in **Doubler**.",
                Type = VarType.Stat,
                ValueGetter = u => Difference(u, TickStats.TimesWon, TickStats.TimesPlayed)
            },
            new Var
            {
                Id = TickStats.CurrentLossStreak,
                Summary = "Increases the chance of winning by 1% for every 3 losses in **Doubler**."
            },
            new Var
            {
                Id = Stats.LastAssignedQuest,
                Name = "Last Assigned Quest",
                Type = VarType.Time
            },
            new Var
            {
                Id = Stats.LastSkippedQuest,
                Name = "Last Skipped Quest",
                Type = VarType.Time
            },
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
                Type = VarType.Attribute,
                Summary = "This determines how many quests you can receive at a time."
            },
            new Var
            {
                Id = Vars.BoosterLimit,
                Name = "Booster Stack Limit",
                Type = VarType.Attribute,
                Summary = "This determines how many boosters you can stack at a time."
            },
            new Var
            {
                Id = Vars.Capacity,
                Name = "Inventory Capacity",
                Type = VarType.Attribute,
                Summary = "This determines how many items you are able to hold at a time.",
                DefaultValue = 4000,
                ValueWriter = InventoryViewer.WriteCapacity
            }
        };

        public static readonly List<VarGroup> Groups = new List<VarGroup>
        {
            new VarGroup
            {
                Id = "items",
                Name = "Items",
                Summary = "This is a collection of statistics used to track the usage of items.",
                Type = VarType.Stat
            },
            new VarGroup
            {
                Id = "var",
                Name = "Attribute",
                Summary = "This is a generic collection of variables primarily used to track custom attributes.",
                Type = VarType.Attribute
            },
            new VarGroup
            {
                Id = "catalog",
                Name = "Catalog Status",
                Summary = "This is a collection of variables used to track an item's known status.",
                Type = VarType.Attribute
            },
            new VarGroup
            {
                Id = "recipe",
                Name = "Recipe Status",
                Summary = "This is a collection of variables used to track the status of a recipe.",
                Type = VarType.Attribute
            },
            new VarGroup
            {
                Id = "shop",
                Name = "Shop Status",
                Summary = "This is a generic collection of variables used to track a shop's known status.",
                Type = VarType.Attribute
            },
            new VarGroup
            {
                Id = "cooldown",
                Name = "Cooldown",
                Summary = "This is a group of variables used to track cooldowns.",
                Type = VarType.Time
            },
            new VarGroup
            {
                Id = "gimi",
                Name = "Gimi",
                Summary = "This is a group of stats used for the **Gimi** casino machine.",
                Type = VarType.Stat,
                Writer = (user =>
                {
                    string winRate = $"**{((user.GetVar(GimiStats.TimesWon) / (double)user.GetVar(GimiStats.TimesPlayed)) * 100):##,0}**%";
                    long profit = user.GetVar(GimiStats.TotalWon) - user.GetVar(GimiStats.TotalLost);
                    var details = new StringBuilder();

                    details.AppendLine($"• **Play Count:** {user.GetVar(GimiStats.TimesPlayed):##,0} ({winRate} win rate)");
                    details.AppendLine($"• **Wins:** {user.GetVar(GimiStats.TimesWon):##,0} ({Icons.Balance} **{user.GetVar(GimiStats.TotalWon):##,0}**)");
                    details.AppendLine($"• **Losses:** {user.GetVar(GimiStats.TimesLost):##,0} ({Icons.Balance} **{user.GetVar(GimiStats.TotalLost):##,0}**)");
                    details.AppendLine($"• **Gold:** {user.GetVar(GimiStats.TimesGold):##,0}");
                    details.AppendLine($"• **Curse:** {user.GetVar(GimiStats.TimesCursed):##,0}");
                    details.AppendLine($"\n• **{(profit >= 0 ? "Profits" : "Expenses")}:** {(profit >= 0 ? Icons.Balance : Icons.Debt)} **{profit:##,0}**");
                    details.AppendLine($"• **Longest Win Streak:** {user.GetVar(GimiStats.LongestWin):##,0} ({Icons.Balance} **{user.GetVar(GimiStats.LargestWin):##,0}**)");
                    details.AppendLine($"• **Longest Loss Streak:** {user.GetVar(GimiStats.LongestLoss):##,0} ({Icons.Debt} **{user.GetVar(GimiStats.LargestLoss):##,0}**)");

                    return details.ToString();
                })
            },
            new VarGroup
            {
                Id = "doubler",
                Name = "Doubler",
                Summary = "This is a group of stats used for the **Doubler** casino machine.",
                Type = VarType.Stat
            }
        };

        public static long GetValue(ArcadeUser user, string id)
        {
            return GetDefiner(id)?.ValueGetter?.Invoke(user) ?? (user.Stats.TryGetValue(id, out long value) ? value : 0);
        }

        public static void Add(ArcadeUser user, long amount, string id)
        {
            user.AddToVar(id, amount);
        }

        public static void Add(ArcadeUser user, long amount, params string[] ids)
        {
            foreach (string id in ids)
                user.AddToVar(id, amount);
        }

        public static void SetValue(ArcadeUser user, string id, long value)
        {
            if (value == 0)
            {
                user.Stats.Remove(id);
                user.SetQuestProgress(id);
                return;
            }

            if (!user.Stats.TryAdd(id, value))
                user.Stats[id] = value;

            string upperId = GetUpperId(id);
            if (!string.IsNullOrWhiteSpace(upperId))
                SetIfGreater(user, upperId, id);
        }

        public static string GetUpperId(string id)
        {
            return GetDefiner(id)?.UpperId;
        }

        public static int Count(ArcadeUser user, VarType type)
        {
            return user.Stats.Count(x => TypeOf(x.Key) == type);
        }

        public static int Count(ArcadeUser user)
        {
            return user.Stats.Count;
        }

        public static string WriteName(string id)
        {
            return GetDefiner(id)?.Name ?? Humanize(id);
        }

        public static string WriteValue(ArcadeUser user, string id)
        {
            VarType type = TypeOf(id);

            return GetDefiner(id)?.ValueWriter?.Invoke(user.GetVar(id))
                   ?? GetGroupDefiner(GetGroup(id))?.ValueWriter?.Invoke(user.GetVar(id))
                   ?? WriteDefault(GetValue(user, id), type);
        }

        public static string ViewDetails(ArcadeUser user, string id, in IEnumerable<ArcadeUser> users = null)
        {
            if (!user.Stats.ContainsKey(id))
                return $"> {Icons.Warning} Unknown stat specified.";

            var details = new StringBuilder();

            string name = WriteName(id);
            string value = WriteValue(user, id);
            string header = string.IsNullOrWhiteSpace(name) ? $"• `{id}`" : $"`{id}`\n• **{name}**";

            details.AppendLine($"{header} = {value}");

            string summary = GetDefiner(id)?.Summary;

            if (!string.IsNullOrWhiteSpace(summary))
                details.AppendLine($"> {summary}");

            VarType type = TypeOf(id);

            if ((users?.Any() ?? false) && type == VarType.Stat)
                details.AppendLine(WriteLeaderboardRank(users, user, id));

            return details.ToString();
        }

        private static string WriteLeaderboardRank(in IEnumerable<ArcadeUser> users, ArcadeUser user, string id)
            => $"> **Global Leaderboard Rank**: **{Leaderboard.GetPosition(users, user, id):##,0}** out of **{users.Count():##,0}**";

        public static VarGroup GetGroupDefiner(string groupId)
            => Groups.FirstOrDefault(x => x.Id == groupId);

        public static IEnumerable<string> WithGroup(ArcadeUser user, string group)
        {
            return user.Stats.Keys.Where(x => EqualsGroup(x, group));
        }

        public static IEnumerable<string> WithKey(ArcadeUser user, string key)
        {
            return user.Stats.Keys.Where(x => EqualsKey(x, key));
        }

        public static VarType GetGroupType(string group)
            => GetGroupDefiner(group)?.Type ?? VarType.Stat;

        public static bool EqualsGroup(string id, string group)
        {
            return GetGroup(id) == group;
        }

        public static bool EqualsKey(string id, string key)
        {
            return GetKey(id) == key;
        }

        public static void AddToValue(ArcadeUser user, string id, long amount = 1)
        {
            user.AddToVar(id, amount);
        }

        public static void SetValue(ArcadeUser user, string id, DateTime time)
        {
            user.SetVar(id, time.Ticks);
        }

        public static long GetOrSet(ArcadeUser user, string id, long defaultValue)
        {
            SetIfEmpty(user, id, defaultValue);
            return GetValue(user, id);
        }

        public static void SetIfEmpty(ArcadeUser user, string id, long value)
        {
            if (GetValue(user, id) == 0)
                user.SetVar(id, value);
        }

        public static void SetIfGreater(ArcadeUser user, string a, string b)
        {
            if (GetValue(user, b) > GetValue(user, a))
                user.SetVar(a, user.GetVar(b));
        }

        public static void SetIfGreater(ArcadeUser user, string a, long b)
        {
            if (b > GetValue(user, a))
                user.SetVar(a, b);
        }

        public static void SetIfLesser(ArcadeUser user, string a, string b)
        {
            if (GetValue(user, b) < GetValue(user, a))
                user.SetVar(a, user.GetVar(b));
        }

        public static void SetIfLesser(ArcadeUser user, string a, long b)
        {
            if (b < GetValue(user, a))
                user.SetVar(a, b);
        }

        public static void Rename(ArcadeUser user, string a, string b)
        {
            if (user.Stats.ContainsKey(b))
                throw new ArgumentException("The new name for the specified var already exists");

            user.SetVar(a, 0, out long previous);
            user.SetVar(b, previous);
        }

        public static void Swap(ArcadeUser user, string a, string b)
        {
            user.SetVar(a, GetValue(user, b), out long previous);
            user.SetVar(b, previous);
        }

        public static long Sum(ArcadeUser user, string a, string b)
        {
            return GetValue(user, a) + GetValue(user, b);
        }

        public static long Difference(ArcadeUser user, string a, string b)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Math.Abs(GetValue(user, b) - GetValue(user, a));
        }

        public static ArcadeUser GetLesser(ArcadeUser a, ArcadeUser b, string id)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            long u = GetValue(a, id);
            long v = GetValue(b, id);

            return u == v ? null : u < v ? a : b;
        }

        public static ArcadeUser GetGreater(ArcadeUser a, ArcadeUser b, string id)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            long u = GetValue(a, id);
            long v = GetValue(b, id);

            return u == v ? null : u > v ? a : b;
        }

        public static long Difference(ArcadeUser a, ArcadeUser b, string id)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            return Math.Abs(GetValue(b, id) - GetValue(a, id));
        }

        public static long Sum(ArcadeUser user, string a, string b, params string[] rest)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Sum(user, a, b) + rest.Sum(user.GetVar);
        }

        public static string Min(ArcadeUser user, string a, string b)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                return "";

            if (string.IsNullOrWhiteSpace(a))
                return b;

            if (string.IsNullOrWhiteSpace(b))
                return a;

            long u = GetValue(user, a);
            long v = GetValue(user, b);

            return u == v ? "" : u < v ? a : b;
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

            long u = GetValue(user, a);
            long v = GetValue(user, b);

            return u == v ? "" : u > v ? a : b;
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

        public static bool IsType(string id, VarType type)
        {
            return TypeOf(id) == type;
        }

        public static bool MeetsCriterion(ArcadeUser user, VarCriterion criterion)
        {
            return GetValue(user, criterion.Id) >= criterion.ExpectedValue;
        }

        public static string Humanize(string id)
        {
            if (!IsValid(id))
                return ""; // throw new ArgumentException("Invalid ID specified");

            string key = GetKey(id);
            return $"{HumanizeGroup(id)}{Separator} {HumanizePartial(key)}";
        }

        private static string HumanizeGroup(string id)
        {
            string group = GetGroup(id);

            if (IsTemplate(id) && GetTemplateType(id) != TemplateType.Any)
            {
                return GetTemplateType(id) switch
                {
                    TemplateType.Item => ItemHelper.Exists(group) ? ItemHelper.NameOf(group) : $"`{group}`",
                    _ => HumanizePartial(group)
                };
            }

            return HumanizePartial(group);
        }

        private static string HumanizePartial(string input)
        {
            var reader = new StringReader(input);
            var text = new StringBuilder();

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
            }

            return text.ToString();
        }

        public static bool IsDefined(string id)
            => GetDefiner(id) != null;

        public static bool IsGroupDefined(string group)
            => Groups.Any(x => x.Id == group);

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
            // If there isn't a definer for this ID, try to find its matching group type
            return GetDefiner(id)?.Type ?? GetGroupType(GetGroup(id));
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

            if (IsGroupDefined(group))
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
                   || c == TextSeparator
                   || c == '.' || c == '#'
                   || c == '/';
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
            if (IsGroupDefined(group))
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
            if (IsGroupDefined(group))
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

            return reader.GetCursor() - groupCursor != 0;
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
            Type = type;
            DefaultValue = defaultValue;
            UpperId = upperId;
        }

        public string Id { get; private set; }

        public string Name { get; internal set; }

        // If this value is greater than the max_id, set the MaxId to this value
        // If unspecified, do nothing
        public string UpperId { get; private set; }

        public string Summary { get; internal set; }

        public Func<ArcadeUser, long> ValueGetter { get; internal set; }

        public Func<long, string> ValueWriter { get; internal set; }

        public TemplateType? Template { get; }

        public VarType? Type { get; private set; }

        public long DefaultValue { get; private set; }
    }
}
