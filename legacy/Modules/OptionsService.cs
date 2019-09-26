using Discord;
using Discord.WebSocket;
using Orikivo.Systems.Presets;
using Orikivo.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    /// <summary>
    /// Represents all of the working methods for OptionsModule.
    /// </summary>
    public static class OptionsService
    {
        public static async Task ReadOptionAsync(OrikivoCommandContext Context, string s)
        {
            if (!OptionsHelper.TryParseOption(s, out AccountOption ao))
            {
                await Context.Channel.ThrowAsync("Invalid context.", "The context specified led to no results.");
                return;
            }
            await Context.Channel.ReadAsync(ao.Interpret(), Context.Account);
        }

        public static async Task ReadOptionAsync(OrikivoCommandContext Context, AccountOption option)
        {
            await Context.Channel.ReadAsync(option.Interpret(), Context.Account);
        }

        public static async Task ReadOptionAsync(OrikivoCommandContext Context, int i)
        {
            if (!OptionsHelper.TryParseOption(i, out AccountOption ao))
            {
                await Context.Channel.ThrowAsync("Invalid context.", "The integer specified led to no results.");
                return;
            }
            await Context.Channel.ReadAsync(ao.Interpret(), Context.Account);
        }

        public static async Task ReadOptionAsync(OrikivoCommandContext Context, Emoji e)
        {
            if (!OptionsHelper.TryParseOption(e, out AccountOption ao))
            {
                await ReadOptionAsync(Context, e.Name);
                //await Context.Channel.ThrowAsync("Invalid context.", "The icon specified led to no results.");
                return;
            }
            await Context.Channel.ReadAsync(ao.Interpret(), Context.Account);
        }

        public static async Task ReadOptionsAsync(OrikivoCommandContext Context)
        {
            await Context.Channel.SendEmbedAsync(OptionsHelper.InterpretAll().PeekAll(Context.Account));
        }

        public static async Task SetOptionAsync(OrikivoCommandContext Context, AccountOption option, object obj)
        {
            AccountOptions opts = AccountOptions.Default;
            opts.TryGetValue(option, out object value);
            if (opts.TrySetValue(option, obj))
            {
                string msg = $"{option} - {(value.Exists() ? value.Read() : "Undefined")}";
                opts.TryGetValue(option, out value);
                msg += $" => {(value.Exists() ? value.Read() : "Undefined")}";
                await Context.Channel.SendMessageAsync(msg);
            }
        }
    }

    public static class AccountOptionExtender
    {
        public static Interpreter Interpret(this AccountOption ao)
            => OptionsHelper.GetInterpreter(ao);

        public static bool Matches(this string s, string input, MatchHandling m = MatchHandling.Match, MatchValueHandling v = MatchValueHandling.Equals)
        {
            s = (m == MatchHandling.Exact) ? s : s.ToString().CaseAs(CaseFormat.Lowercase);
            input = (m == MatchHandling.Exact) ? input : input.CaseAs(CaseFormat.Lowercase);

            bool success = false;
            switch (v)
            {
                case MatchValueHandling.StartsWith:
                    success = s.StartsWith(input);
                    break;
                case MatchValueHandling.EndsWith:
                    success = s.EndsWith(input);
                    break;
                case MatchValueHandling.Contains:
                    success = s.Contains(input);
                    break;
                default:
                    success = s == input;
                    break;
            }

            return success;
        }

        public static bool IsNullable(this Type t)
        {
            if (t.IsValueType)
                return true;
            if (t == typeof(string))
                return true;
            if (Nullable.GetUnderlyingType(t) != null)
                return true;
            return false;
        }

        public static bool IsNullable(this object obj)
        {
            if (obj == null)
                return true;
            Type t = obj.GetType();
            if (t.IsValueType)
                return true;
            if (Nullable.GetUnderlyingType(t) != null)
                return true;
            return false;
        }

        public static object GetValue(this AccountOption ao)
        {
            AccountOptions acc = AccountOptions.Default;
            if (acc.TryGetValue(ao, out object obj))
                return obj;
            throw new Exception("Invalid account option property.");
        }

        public static Type GetValueType(this AccountOption ao)
        {
            AccountOptions acc = AccountOptions.Default;
            if (acc.TryGetType(ao, out Type type))
                return type;
            throw new Exception("Invalid account option property.");
        }

        public static Type GetInnerType(this object obj)
        {
            Type t = obj.GetType();
            if (t.IsArray)
            {
                return t.GetElementType();
            }
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return t.GetGenericArguments()[0];
            }

            Type e = t.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(x => x.GetGenericArguments()[0]).FirstOrDefault();

            return e ?? t;

        }

        public static bool IsList(this object obj)
        {
            if (!obj.Exists())
                return false;
            return obj is IList &&
                obj.GetType().IsGenericType &&
                obj.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static string Read(this object obj)
        {
            string s = "Undefined";
            if (obj.Exists())
            {
                if (obj.IsList())
                {
                    IEnumerable<object> v = (obj as IEnumerable<object>).Cast<object>();
                    if (v.Funct())
                    {
                        s = string.Join(", ", v);
                    }
                    else
                    {
                        s = "Empty";
                    }
                }
                else
                {
                    s = obj.ToString();
                }
            }
            return s;
        }

        public static string ReadValue(this AccountOption ao)
        {
            AccountOptions acc = AccountOptions.Default;
            object value = ao.GetValue();
            string s = "Undefined";
            if (value.Exists())
            {
                if (value.IsList())
                {
                    IEnumerable<object> v = (value as IEnumerable<object>).Cast<object>();
                    if (v.Funct())
                    {
                        s = string.Join(", ", v);
                    }
                    else
                    {
                        s = "Empty";
                    }
                }
                else
                {
                    s = value.ToString();
                }
            }
            return s;
        }

        /// <summary>
        /// Returns a <see cref="string"/> using a specified <see cref="Interpreter"/>.
        /// </summary>
        public static string Peek(this Interpreter itr, OldAccount a)
        {
            AccountOptions opts = AccountOptions.Default;
            return $"{(itr.Icon.Exists() ? $"{itr.Icon.Pack(a)} " : "")}{(opts.SymbolNameDisplay ? itr.Name.MarkdownBold() : "")}{(itr.Value.Exists() ? $"{(opts.SymbolNameDisplay ? $" - {itr.Value}" : $" {itr.Value.MarkdownBold()}")}" : "")}";
        }

        public static Embed PeekAll(this List<Interpreter> itrs, OldAccount a)
        {
            EmbedBuilder eb = EmbedData.DefaultEmbed;
            StringBuilder sb = new StringBuilder();
            eb.WithTitle($"Options".MarkdownBold());
            foreach (Interpreter itr in itrs)
            {
                sb.AppendLine(itr.Peek(a));
            }
            eb.WithDescription(sb.ToString());
            return eb.Build();
        }

        public static async Task ReadAsync(this ISocketMessageChannel Channel, Interpreter itr, OldAccount a)
        {
            AccountOptions opts = AccountOptions.Default;
            if (opts.OutputFormat == OutputFormat.Markdown)
            {
                await Channel.SendMessageAsync(itr.Markdown());
            }
            else
            {
                await Channel.SendEmbedAsync(itr.Build(a));
            }
        }

        public static string Markdown(this Interpreter itr)
        {
            StringBuilder sb = new StringBuilder();
            if (itr.Value.Exists())
            {
                sb.AppendLine($"[Id({itr.Id})]");
            }
            if (itr.Icon.Exists())
            {
                sb.AppendLine($"[Icon(\"{itr.Icon}\")]");
            }
            sb.AppendLine($"[Type(\"{itr.Type.FullName}\")]");
            sb.AppendLine($"[Name(\"{itr.Name}\")]");
            sb.AppendLine($"[Definition(\"{itr.FlavorText}\")]");
            if (itr.Value.Exists())
            {
                sb.AppendLine($"[Value(\"{itr.Value}\")]");
            }
            return sb.ToString().DiscordBlock("cs");
        }

        public static Embed Build(this Interpreter itr, OldAccount a)
        {
            EmbedBuilder eb = EmbedData.DefaultEmbed;
            eb.WithTitle($"{(itr.Icon.Exists() ? $"{itr.Icon.Pack(a)} " : "")}{itr.Name.MarkdownBold()}");
            eb.WithDescription($"{(itr.Value.Exists() ? $"**Value**: {itr.Value}\n" : "")}{itr.FlavorText}");
            eb.WithFooter($"{(itr.Id.HasValue ? $"{itr.Id} | " : "")}{itr.Type.Name}");
            return eb.Build();
        }

        public static bool TryGetIcon(this PropertyInfo property, out Emoji icon)
        {
            icon = null;
            IconAttribute ico = property.GetCustomAttribute<IconAttribute>();
            if (ico.Exists())
            {
                icon = ico.Icon;
                return true;
            }
            return false;
        }

        public static bool TryGetDefinition(this PropertyInfo property, out string definition)
        {
            definition = null;
            DefinitionAttribute def = property.GetCustomAttribute<DefinitionAttribute>();
            if (def.Exists())
            {
                definition = def.Definition;
                return true;
            }
            return false;
        }

        public static bool TryGetAccountOption(this PropertyInfo property, out AccountOption option)
        {
            option = AccountOption.AutoFix;
            AccountOptionAttribute a = property.GetCustomAttribute<AccountOptionAttribute>();
            if (a.Exists())
            {
                option = a.Option;
                return true;
            }
            return false;
        }

        public static bool TryGetAccountOption(this Emoji emoji, out AccountOption option)
        {
            option = AccountOption.AutoFix;
            foreach (PropertyInfo p in AccountOptions.DefaultProperties)
            {
                if (p.TryGetIcon(out Emoji e))
                {
                    if (emoji.Name == e.Name)
                    {
                        if (p.TryGetAccountOption(out option))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool TryGetName(this PropertyInfo property, out string name)
        {
            name = null;
            DisplayNameAttribute n = property.GetCustomAttribute<DisplayNameAttribute>();
            if (n.Exists())
            {
                name = n.Name;
                return true;
            }
            return false;
        }
    }

    public static class OptionsHelper
    {
        public static bool TryParseOption(Emoji emoji, out AccountOption ao)
        {
            ao = AccountOption.AutoFix;
            if (emoji.TryGetAccountOption(out ao))
            {
                return true;
            }
            return false;
        }
        public static bool TryParseOption(string s, out AccountOption ao)
        {
            ao = AccountOption.AutoFix;
            Type option = typeof(AccountOption);
            foreach (string name in option.GetEnumNames())
            {
                name.Debug();
                if (name.Matches(s, MatchHandling.Match, MatchValueHandling.StartsWith))
                {
                    ao = (AccountOption)option.GetField(name).GetRawConstantValue();
                    return true;
                }
            }

            return false;
        }
        public static bool TryParseOption(int i, out AccountOption ao)
        {
            if (!Enum.TryParse($"{i}", out ao))
                return false;
            return true;
        }

        public static List<Interpreter> InterpretAll()
        {
            AccountOptions opts = AccountOptions.Default;
            List<Interpreter> itrs = new List<Interpreter>();
            foreach (PropertyInfo p in opts.ValidProperties)
            {
                object value = p.GetValue(opts);
                if (!value.Exists())
                {
                    if (opts.EmptyFormat == NullObjectHandling.Ignore)
                        continue;
                }

                itrs.Add(Interpret(p));
            }

            return itrs;
        }

        public static Interpreter Interpret(PropertyInfo p)
        {
            AccountOptions opts = AccountOptions.Default;
            p.TryGetIcon(out Emoji icon);
            p.TryGetName(out string name);
            p.TryGetDefinition(out string definition);
            p.TryGetAccountOption(out AccountOption option);
            return new Interpreter(p.PropertyType, name ?? p.Name, definition, value: p.GetValue(opts).Read(), icon: icon, id: (ulong)option);
        }

        public static Interpreter GetInterpreter(AccountOption ao)
        {
            AccountOptions opts = AccountOptions.Default;
            if (opts.TryGetOptionProperty(ao, out PropertyInfo p))
            {
                return Interpret(p);
            }
            throw new Exception("Invalid account option property.");
        }
    }
}
