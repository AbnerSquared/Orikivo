using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orikivo
{
    /// <summary>
    /// Represents the context of a method search.
    /// </summary>
    public struct InfoContext
    {   
        //                                    MODULES            GROUPS            ROOT        INDEX              TYPE       PARAM                 PAGE
        private const string MAIN_PARSER = @"^((?:[A-Za-z_]+\.)*)((?:[A-Za-z_]+ )*)([A-Za-z_]+)(?:(?:\+(\d{1,3}))?([\.\*\(]?)([A-Za-z_]*)\)?)?(?: +(\d{1,3}))? *"; // Regex.Match
        /*
            Input: module.module.module.group group group command+1(param) 2
            Group 0: Match ==> module.module.module.group group group command+1(param) 2
            Group 1: Modules ==> module.module.module.
            Group 2: Groups ==> group group group
            Group 3: Root ==> command
            Group 4: Index ==> 1
            Group 5: Type ==> (
            Group 6: Parameter ==> param
            Group 7: Page ==> 2
         */
        
        private const string MODULE_PARSER = @"(?:([A-Za-z_]+)\.)*";
        private const string GROUP_PARSER = @"(?:([A-Za-z_]+) )*";

        /// <summary>
        /// Gets the raw content that represents the search context.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets a <see cref="bool"/> that defines if the <see cref="InfoContext"/> was successfully parsed.
        /// </summary>
        public bool IsSuccess => !Check.NotNull(ErrorReason);

        /// <summary>
        /// Gets the reason the <see cref="InfoContext"/> failed to parse, if any.
        /// </summary>
        public string ErrorReason { get; private set; }

        /// <summary>
        /// Gets the list of specified modules, if any.
        /// </summary>
        public IEnumerable<string> Modules { get; private set; }

        /// <summary>
        /// Gets the list of specified groups, if any.
        /// </summary>
        public IEnumerable<string> Groups { get; private set; }

        /// <summary>
        /// Gets the main search context of the <see cref="InfoContext"/>.
        /// </summary>
        public string Root { get; private set; }

        /// <summary>
        /// Gets the parameter for the search context specified.
        /// </summary>
        public string Parameter { get; private set; }

        /// <summary>
        /// Gets the <see cref="InfoType"/> to explicitly search for, if defined.
        /// </summary>
        public InfoType? Type { get; private set; }

        /// <summary>
        /// Gets the priority index of the search context. If the specified <see cref="InfoType"/> is not <see cref="InfoType.Command"/>, this is ignored.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Gets a <see cref="bool"/> that determines if a priority can be specified for the search context.
        /// </summary>
        public bool HasPriority { get; private set; }

        /// <summary>
        /// Gets the page index for the search context. If this is unspecified, it defaults to 0.
        /// </summary>
        public int Page { get; private set; }

        /// <summary>
        /// Parses the specified content to a new <see cref="InfoContext"/>.
        /// </summary>
        public static InfoContext Parse(string content)
        {
            var ctx = new InfoContext();
            Match m = new Regex(MAIN_PARSER).Match(content);

            ctx.Content = content;

            if (!m.Success)
            {
                ctx.ErrorReason = "The content specified failed to successfully parse.";
                return ctx;
            }

            ctx.Modules = new Regex(MODULE_PARSER)
                .Matches(m.Groups[1].Value)
                .Where(x => x.Success)
                .Select(x => x.Groups[1].Value);

            ctx.Groups = new Regex(GROUP_PARSER)
                .Matches(m.Groups[2].Value)
                .Where(x => x.Success)
                .Select(x => x.Groups[1].Value);

            ctx.Root = m.Groups[2].Value + m.Groups[3].Value;

            ctx.HasPriority = m.Groups[4].Success;

            if (ctx.HasPriority)
                ctx.Priority = int.TryParse(m.Groups[4].Value, out int index) ? index : 0;

            ctx.Type = GetTypeValue(m.Groups[5].Value);

            // If an overload index is specified AND the type (if specified) is not a command
            if (ctx.Type.GetValueOrDefault(InfoType.Command) != InfoType.Command && m.Groups[4].Success)
            {
                ctx.ErrorReason = $"{ctx.Type?.ToString()}s do not support overload indexing.";
                return ctx;
            }

            ctx.Parameter = m.Groups[6].Value;

            // If a parameter is specified AND the type (if specified) is not a command
            if (ctx.Type.GetValueOrDefault(InfoType.Command) != InfoType.Command && Check.NotNull(m.Groups[6].Value))
            {
                ctx.ErrorReason = $"{ctx.Type.ToString()}s do not support parameters.";
                return ctx;
            }

            ctx.Page = (int.TryParse(m.Groups[7].Value, out int page) ? page : 1) - 1;

            return ctx;
        }

        private static InfoType? GetTypeValue(string marker)
            => marker switch
            {
                "." => InfoType.Module,
                "*" => InfoType.Group,
                "(" => InfoType.Command,
                _ => null
            };
    }
}
