using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Orikivo
{
    // TODO: Handle internal indexes, priority, etc.
    // TODO: Simplify the Regex, if possible.
    // this class is used to help read complete regex searches of help context
    public struct ContextInfo
    {
        public string Content { get; private set; }
        public bool IsSuccess { get; private set; }
        public ContextError? Error { get; private set; }
        public bool HasParameter => Checks.NotNull(Parameter);
        public bool HasPriority => _priority.HasValue;
        public bool HasRoot => Checks.NotNull(Root);
        public List<string> Modules { get; private set; }
        public List<string> Groups { get; private set; }
        private string _root;
        public string Root
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (Groups?.Count > 0)
                    sb.Append($"{string.Join(' ', Groups)} ");

                sb.Append(_root);

                return sb.ToString();
            }
        }

        public string Parameter { get; private set; }

        private int? _priority;
        public int Priority => _priority ?? 0;

        private int? _index;
        public int Index => _index ?? 0;

        public ContextInfoType? Type { get; private set; }

        public ContextSearchMethod? SearchFormat { get; private set; }

        public static ContextInfo Parse(string content)
        {
            ContextInfo ctx = new ContextInfo();

            if (!Checks.NotNull(content))
            {
                ctx.IsSuccess = false;
                ctx.Error = ContextError.EmptyValue;
                return ctx;
            }

            content = content.ToLower();

            Match m = OriRegex.ParseContext(content);

            ctx.IsSuccess = m.Groups[0].Success;
            if (ctx.IsSuccess)
                ctx.Content = content;

            ctx._index = int.TryParse(m.Groups[1].Value, out int i) ?
                i : int.TryParse(m.Groups[3].Value, out i) ?
                i : int.TryParse(m.Groups[8].Value, out i) ?
                i : (int?)null;


            ctx.SearchFormat = m.Groups[2].Success ?
                ContextSearchMethod.All : m.Groups[10].Success ?
                GetSearchFormat(m.Groups[10].Value) : (ContextSearchMethod?)null;

            ctx.Modules = m.Groups[4].Success ?
                OriRegex.GetContextModules(m.Groups[4].Value) : null;

            ctx.Groups = m.Groups[5].Success ?
                OriRegex.GetContextGroups(m.Groups[5].Value) : null;

            ctx._root = m.Groups[6].Value;

            ctx.Type = GetType(m.Groups[7].Value);

            ctx._priority = m.Groups[9].Success ?
                int.Parse(m.Groups[9].Value) : (int?)null;

            ctx.Parameter = m.Groups[11].Value;

            return ctx;
        }

        public static ContextInfoType? GetType(string type)
        {
            if (!Checks.NotNull(type))
                return null;

            return type switch
            {
                "m" => ContextInfoType.Module,
                "g" => ContextInfoType.Group,
                "c" => ContextInfoType.Command,
                _ => throw new Exception("The specified HelperContextType does not exist."),
            };
        }

        public static ContextSearchMethod GetSearchFormat(string format)
        {
            return format switch
            {
                "~" => ContextSearchMethod.List,
                "*" => ContextSearchMethod.Verbose,
                _ => ContextSearchMethod.Default,
            };
        }
    }
}
