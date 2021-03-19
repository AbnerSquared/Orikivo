using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Text;

namespace Arcadia
{
    /// <summary>
    /// Represents a generic implicitly defined value.
    /// </summary>
    public class Var
    {
        public static readonly char Placeholder = '*';
        public static readonly char Subgroup = '/';
        public static readonly char Delimiter = ':';
        public static readonly char TextDelimiter = '_';

        // TODO: Move to a static viewer class
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
                Id = Stats.Gimi.TimesLost,
                Summary = "This represents all of the times you have lost in **Gimi**.",
                Type = VarType.Stat,
                ValueGetter = u => Difference(u, Stats.Gimi.TimesWon, Stats.Gimi.TimesPlayed)
            },
            new Var
            {
                Id = Stats.Doubler.TimesLost,
                Summary = "This represents all of the times you have lost in **Doubler**.",
                Type = VarType.Stat,
                ValueGetter = u => Difference(u, Stats.Doubler.TimesWon, Stats.Doubler.TimesPlayed)
            },
            new Var
            {
                Id = Stats.Doubler.CurrentLossStreak,
                Summary = "Increases the chance of winning by 1% for every 3 losses in **Doubler**."
            },
            new Var
            {
                Id = Stats.Common.LastAssignedQuest,
                Name = "Last Assigned Quest",
                Type = VarType.Time
            },
            new Var
            {
                Id = Stats.Common.LastSkippedQuest,
                Name = "Last Skipped Quest",
                Type = VarType.Time
            },
            new Var
            {
                Id = Stats.Gimi.CurrentWinStreak,
                UpperId = Stats.Gimi.LongestWin,
                Summary = "This represents your current winning streak in **Gimi**."
            },
            new Var
            {
                // TODO: Do something like GetLocaleText(string id) => var_{id}_name AND var_{id}_summary0
                Id = Stats.Common.QuestCapacity,
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
            },
            new Var
            {
                Id = Stats.Multiplayer.LastGamePlayed,
                Name = "Last Game Played",
                Type = VarType.Time,
                Summary = "Represents the last time (in UTC) from which you played a multiplayer game.",
                ValueWriter = (ticks) => Format.Date(new DateTime(ticks))
            },
            new Var
            {
                Id = Vars.MonthlyArcade,
                Type = VarType.Stat,
                ValueWriter = v => $"**{v:##,0} AP**"
            },
            new Var
            {
                Id = Vars.MonthlyCasino,
                Type = VarType.Stat,
                ValueWriter = v => $"{Icons.Chips} **{v:##,0}**"
            },
            new Var
            {
                Id = Vars.MonthlyExp,
                Type = VarType.Stat,
                ValueWriter = v => $"{Icons.Exp} **{v.ToString("##,0")} XP**"
            },
            new Var
            {
                Id = Vars.MonthlyIncome,
                Type = VarType.Stat,
                ValueWriter = v => $"{Icons.Balance} **{v.ToString("##,0")}**"
            },
            new Var
            {
                Id = Vars.MonthlyQuests,
                Type = VarType.Stat,
                ValueWriter = v => $"**{v.ToString("##,0")} QP**"
            },
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
                Type = VarType.Attribute,
                Visible = false
            },
            new VarGroup
            {
                Id = "catalog",
                Name = "Catalog Status",
                Summary = "This is a collection of variables used to track an item's known status.",
                Type = VarType.Attribute,
                Visible = false
            },
            new VarGroup
            {
                Id = "multiplayer",
                Name = "Multiplayer",
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
                Type = VarType.Time,
                Visible = false
            },
            new VarGroup
            {
                Id = "gimi",
                Name = "Gimi",
                Summary = "This is a group of stats used for the **Gimi** casino machine.",
                Type = VarType.Stat,
                Writer = (user =>
                {
                    string winRate = $"**{((user.GetVar(Stats.Gimi.TimesWon) / (double)user.GetVar(Stats.Gimi.TimesPlayed)) * 100):##,0}**%";
                    long profit = user.GetVar(Stats.Gimi.TotalWon) - user.GetVar(Stats.Gimi.TotalLost);
                    var details = new StringBuilder();

                    details.AppendLine($"• **Play Count:** {user.GetVar(Stats.Gimi.TimesPlayed):##,0} ({winRate} win rate)");
                    details.AppendLine($"• **Wins:** {user.GetVar(Stats.Gimi.TimesWon):##,0} ({Icons.Balance} **{user.GetVar(Stats.Gimi.TotalWon):##,0}**)");
                    details.AppendLine($"• **Losses:** {user.GetVar(Stats.Gimi.TimesLost):##,0} ({Icons.Balance} **{user.GetVar(Stats.Gimi.TotalLost):##,0}**)");
                    details.AppendLine($"• **Gold:** {user.GetVar(Stats.Gimi.TimesGold):##,0}");
                    details.AppendLine($"• **Curse:** {user.GetVar(Stats.Gimi.TimesCursed):##,0}");
                    details.AppendLine($"\n• **{(profit >= 0 ? "Profits" : "Expenses")}:** {(profit >= 0 ? Icons.Balance : Icons.Debt)} **{profit:##,0}**");
                    details.AppendLine($"• **Longest Win Streak:** {user.GetVar(Stats.Gimi.LongestWin):##,0} ({Icons.Balance} **{user.GetVar(Stats.Gimi.LargestWin):##,0}**)");
                    details.AppendLine($"• **Longest Loss Streak:** {user.GetVar(Stats.Gimi.LongestLoss):##,0} ({Icons.Debt} **{user.GetVar(Stats.Gimi.LargestLoss):##,0}**)");

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

        public static bool Judge(ArcadeUser user, string id, Func<long, bool> judge)
            => judge?.Invoke(user.GetVar(id)) ?? true;

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

            string upperId = GetUpperBoundId(id);
            if (!string.IsNullOrWhiteSpace(upperId))
                SetIfGreater(user, upperId, id);
        }

        public static string GetUpperBoundId(string id)
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

        public static string WriteValue(string id, long value)
        {
            VarType type = TypeOf(id);

            return GetDefiner(id)?.ValueWriter?.Invoke(value)
                   ?? GetGroupDefiner(GetGroup(id))?.ValueWriter?.Invoke(value)
                   ?? WriteDefault(value, type);
        }

        public static string WriteValue(ArcadeUser user, string id)
        {
            VarType type = TypeOf(id);

            return GetDefiner(id)?.ValueWriter?.Invoke(user.GetVar(id))
                   ?? GetGroupDefiner(GetGroup(id))?.ValueWriter?.Invoke(user.GetVar(id))
                   ?? WriteDefault(GetValue(user, id), type);
        }

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
                throw new ArgumentException("The new name for the specified variable already exists");

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

        // return amount removed
        public static int ClearAll(ArcadeUser user, Func<KeyValuePair<string, long>, bool> predicate)
        {
            int amount = 0;
            foreach (KeyValuePair<string, long> stat in user.Stats)
            {
                if (predicate.Invoke(stat))
                {
                    user.Stats.Remove(stat.Key);
                    amount++;
                }
            }

            return amount;
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

        public static bool All(ArcadeUser user, VarOp match, long value, params string[] ids)
        {
            foreach (string id in ids)
            {
                if (!Matches(user, id, match, value))
                    return false;
            }

            return true;
        }

        public static bool Matches(ArcadeUser user, string id, VarOp match, long value)
        {
            long compare = user.GetVar(id);

            return match switch
            {
                VarOp.GTR => compare > value,
                VarOp.GEQ => compare >= value,
                VarOp.EQU => compare == value,
                VarOp.LEQ => compare <= value,
                VarOp.LSS => compare < value,
                VarOp.NEQ => compare != value,
                _ => throw new ArgumentOutOfRangeException(nameof(match))
            };
        }

        // TODO: Move to a static viewer class
        public static string Humanize(string id)
        {
            if (!IsValid(id))
                return ""; // throw new ArgumentException("Invalid ID specified");

            string name = GetDefiner(id)?.Name;

            if (!string.IsNullOrWhiteSpace(name))
                return name;

            string key = GetKey(id);
            return $"{HumanizeGroup(id)}{Delimiter} {HumanizePartial(key)}";
        }

        public static string HumanizeKey(string id)
        {
            string key = GetKey(id);

            return HumanizePartial(key);
        }

        // TODO: Move to a static viewer class
        public static string HumanizeGroup(string id)
        {
            string group = GetGroup(id);

            if (group != null && IsTemplate(id) && GetTemplateType(id) != TemplateType.Any)
            {
                return GetTemplateType(id) switch
                {
                    TemplateType.Item => ItemHelper.Exists(group) ? ItemHelper.NameOf(group) : $"`{group}`",
                    TemplateType.Shop => ShopHelper.Exists(group) ? ShopHelper.NameOf(group) : $"`{group}`",
                    _ => HumanizePartial(group)
                };
            }

            return HumanizePartial(group);
        }

        // TODO: Move to a static viewer class
        public static string HumanizePartial(string input)
        {
            var reader = new StringReader(input);
            var text = new StringBuilder();

            bool upper = true;
            while (reader.CanRead())
            {
                char c = reader.Read();

                if (c == TextDelimiter)
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
                   || c == TextDelimiter
                   || c == '.' || c == '#'
                   || c == '/';
        }

        public static string GetGroup(string id)
        {
            if (!IsValid(id))
                return null;

            var reader = new StringReader(id);

            return reader.ReadUntil(Delimiter);
        }

        public static string GetKey(string id)
        {
            if (!IsValid(id))
                return null;

            var reader = new StringReader(id);

            reader.SkipUntil(Delimiter, true);

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

            if (reader.CanRead() && reader.Peek() == TextDelimiter)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Placeholder || c == Delimiter)
                    return false;
            }

            return true;
        }

        public static bool IsKeyValid(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            var reader = new StringReader(key);

            if (reader.CanRead() && reader.Peek() == TextDelimiter)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Placeholder || c == Delimiter)
                    return false;
            }

            return true;
        }

        public static bool IsValid(string id, bool enforceGroup = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            id = id.Trim();

            if (!id.Contains(Delimiter))
                return false;

            var reader = new StringReader(id);

            if (reader.CanRead() && reader.Peek() == TextDelimiter)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();
                if (c == Placeholder && reader.CanRead() && reader.Peek() != Delimiter)
                    return false;

                if (c == Delimiter)
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

            if (reader.CanRead() && reader.Peek() == TextDelimiter)
                return false;

            while (reader.CanRead())
            {
                char c = reader.Read();

                if (!IsCharValid(c) || c == Delimiter)
                    return false;
            }

            return reader.GetCursor() - groupCursor != 0;
        }

        internal Var() {}

        internal Var(string key, TemplateType template = TemplateType.Any, VarType type = VarType.Stat, long defaultValue = 0)
        {
            if (!IsKeyValid(key))
                throw new ArgumentException("Could not validate the specified key to a Var.");

            Id = $"{Placeholder}{Delimiter}{key}";
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

        public string UpperId { get; private set; }

        public string Summary { get; internal set; }

        public Func<ArcadeUser, long> ValueGetter { get; internal set; }

        public Func<long, string> ValueWriter { get; internal set; }

        public TemplateType? Template { get; }

        public VarType? Type { get; private set; }

        public long DefaultValue { get; private set; }
    }
}
