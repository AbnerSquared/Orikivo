using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Orikivo
{
    // this class is used to help read complete regex searches of help context
    public struct ContextInfo
    {
        public string Content { get; private set; }
        public bool IsSuccess { get; private set; }
        public ContextError? ErrorReason { get; private set; }
        public bool HasArg { get { return !string.IsNullOrWhiteSpace(Arg); } }
        public bool HasPriority { get { return _priority.HasValue; } }
        public bool HasRoot { get { return !string.IsNullOrWhiteSpace(Root); } }
        public List<string> Modules { get; private set; }
        public List<string> Groups { get; private set; }
        private string _root;
        public string Root
        {
            get
            {
                if (Groups != null)
                    if (Groups.Count > 0)
                        return $"{string.Join(' ', Groups)} {_root}";
                return _root;
            }
        }

        public string Arg { get; private set; }
        internal int? _priority;
        public int Priority { get { return _priority ?? 0; } }
        internal int? _index;
        public int Index { get { return _index ?? 0; } }
        public ContextInfoType? Type { get; private set; }
        public ContextSearchFormat? SearchFormat { get; private set; }

        public static ContextInfo Parse(string content)
        {
            ContextInfo ctx = new ContextInfo();

            if (string.IsNullOrWhiteSpace(content))
            {
                ctx.IsSuccess = false;
                ctx.ErrorReason = ContextError.EmptyValue;
                return ctx;
            }

            content = content.ToLower();

            Match m = OriRegex.ParseContext(content);

            ctx.IsSuccess = m.Groups[0].Success;
            if (ctx.IsSuccess)
                ctx.Content = content;

            ctx._index = int.TryParse(m.Groups[1].Value, out int i) ? i : int.TryParse(m.Groups[3].Value, out i) ? i : int.TryParse(m.Groups[8].Value, out i) ? i : (int?)null;
            ctx.SearchFormat = m.Groups[2].Success ? ContextSearchFormat.All : m.Groups[10].Success ? GetSearchFormat(m.Groups[10].Value) : (ContextSearchFormat?)null;
            ctx.Modules = m.Groups[4].Success ? OriRegex.GetContextModules(m.Groups[4].Value) : null;
            ctx.Groups = m.Groups[5].Success ? OriRegex.GetContextGroups(m.Groups[5].Value) : null;
            ctx._root = m.Groups[6].Value;
            ctx.Type = GetType(m.Groups[7].Value);
            ctx._priority = m.Groups[9].Success ? int.Parse(m.Groups[9].Value) : (int?)null;
            ctx.Arg = m.Groups[11].Value;

            return ctx;
        }

        public static ContextInfoType? GetType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            switch (type)
            {
                case "m":
                    return ContextInfoType.Module;
                case "g":
                    return ContextInfoType.Group;
                case "c":
                    return ContextInfoType.Command;
                default:
                    throw new Exception("The specified HelperContextType does not exist.");
            }
        }

        public static ContextSearchFormat GetSearchFormat(string format)
        {
            switch (format)
            {
                case "~":
                    return ContextSearchFormat.List;
                case "*":
                    return ContextSearchFormat.Verbose;
                default:
                    return ContextSearchFormat.Default;
            }
        }
    }
}
