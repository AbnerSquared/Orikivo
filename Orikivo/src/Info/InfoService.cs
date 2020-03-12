using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Orikivo.Desync;

namespace Orikivo
{

    // TODO: Integrate ReportContainer for the help service.
    /// <summary>
    /// A help service which provides information on all commands as an override to <see cref="CommandService"/>.
    /// </summary>
    public class InfoService
    {
        // TODO: Separate IconHandler, which can manage fallback emojis and custom emojis
        public const int YIELD_THRESHOLD = 1;
        public const int CRITICAL_THRESHOLD = 10;
        public const string STABLE_EMOJI = "\uD83D\uDD39"; // :small_blue_diamond:
        public const string YIELD_EMOJI = "\uD83D\uDD38"; // :small_orange_diamond:
        public const string CRITICAL_EMOJI = "\uD83D\uDD3A"; // :small_red_triangle:

        private readonly CommandService _commands;
        private readonly ReportContainer _reports;
        private readonly IEnumerable<GuildCommand> _guildCommands;

        private static readonly Func<ModuleInfo, string, bool> MODULE_MATCHER = (ModuleInfo m, string n) => m.Name.ToLower() == n.ToLower();
        private static readonly Func<ModuleInfo, string, bool> GROUP_MATCHER = (ModuleInfo m, string n) => m.Group?.ToLower() == n.ToLower();
        private static readonly Func<CommandInfo, string, bool> COMMAND_MATCHER = (CommandInfo c, string n) => c.Aliases.Contains(n.ToLower());
        private static readonly Func<ParameterInfo, string, bool> PARAM_MATCHER = (ParameterInfo p, string n) => p.Name.ToLower() == n.ToLower();

        // TODO: Fix the guide storage location
        public static List<GuideNode> DefaultGuides = new List<GuideNode>
        {
            new GuideNode
            {
                Id = "beginner",
                Title = "The **Beginner's** Guide",
                Tooltip = "**Beginner**: Learn more about how to use **Orikivo**.",
                Chapters = new List<GuideChapter>
                {
                    new GuideChapter
                    {
                        Number = 1,
                        Title = "Using the `help` command",
                        Content = new StringBuilder()
                        .AppendLine("• The `help` command may be mess at first glance, but it was made to allow you to understand any command to an extreme depth.\n")
                        .AppendLine("• A command can be implicitly or explicitly searched. (In quick terms, this just means that you can optionally write the name of the module that the command was in, and **I** will try to find the best match possible.)\n")
                        .AppendLine("• The `help` command allows you to learn more about the following:```Modules: a group of commands and modules\nGroups: a command that contains other commands written after its main name)\nCommands: a group of overloads\nOverloads: a method for a specific command\nParameters: an argument for an overload```")
                        .AppendLine("• By default, if nothing is specified, you will be shown the main help menu, which lists the guides, modules, and **actions** (see `help husks`).")
                        .AppendLine("• However, if you wish to learn more about any of them, you can normally simply write `help <name>`.\n")
                        .AppendLine("• Another part of what might make the `help` command confusing is that there is multiple ways a command can be executed, also known as **overloads**.")
                        .AppendLine("• If there is more than one overload for a command, it will be denoted by a `+index` marker at the end of a command, where index is a number marking the overload.")
                        .AppendLine("• When using the `help` command, you can explicitly specify the overload of a command by writing its index after the name of a command (`help command+index`).")
                        .AppendLine("• You can also learn more about the parameter of a command by writing the name of the parameter you wish learn about after the command name (`help command(parameter`).")
                        .ToString()
                    },
                    new GuideChapter
                    {
                        Number = 2,
                        Title = "Initializing Husks",
                        Content = new StringBuilder()
                        .AppendLine("Husks are an integral part of **Orikivo**. In order to start utilizing one, you can type `awaken`, which will set up your very own **Husk**.")
                        .AppendLine("**Husks** are a physical counterpart to your digital account. They are being controlled by your actions in the real world, which can be used to explore and gather materials that can be used to upgrade your digital progress.")
                        .ToString()
                    }
                }
            }
        };

        public InfoService(CommandService commands, OriGlobal global)
        {
            _commands = commands;
            _reports = global.Reports;

            Guides = DefaultGuides;
        }

        public InfoService(CommandService commands, OriGlobal global, OriGuild guild) : this(commands, global)
        {
            _guildCommands = guild.Options.Commands;
        }

        // BASE
        public static string GetSeverityIcon(int reportCount)
            => reportCount >= CRITICAL_THRESHOLD ?
            CRITICAL_EMOJI : reportCount >= YIELD_THRESHOLD ?
            YIELD_EMOJI : STABLE_EMOJI;

        public static string GetSeverityIcon(ContextNode node)
            => GetSeverityIcon(node.Reports.Where(x => x.State == ReportState.Open).Count());

        public string GetActions(User user)
        {
            StringBuilder panel = new StringBuilder();
            // Husk system
            if (user.Husk != null)
            {
                bool canMove = Engine.CanMove(user, user.Husk);
                panel.AppendLine();
                panel.AppendLine("**Actions**");
                panel.Append("• ");
                panel.AppendLine(user.Husk.Location.Summarize());

                ModuleInfo main = _commands.Modules.First(x => x.Name == "Actions");
                List<CommandNode> actions = new List<CommandNode>();
                
                foreach (CommandInfo action in main.Commands)
                {
                    if (actions.Any(x => x.Name == action.Name))
                        continue;

                    // Flag checking
                    CheckFlagsAttribute flags = action.Preconditions.GetAttribute<CheckFlagsAttribute>();
                    RequireLocationAttribute check = action.Attributes.GetAttribute<RequireLocationAttribute>();

                    if (flags != null)
                    {
                        if (!flags.Check(user.Brain))
                            continue;
                    }

                    if (check != null)
                    {
                        if (!canMove)
                            continue;

                        if (!check.Check(user.Husk.Location))
                            continue;
                    }

                    actions.Add(new CommandNode(GetCommands(action.Name, main)));
                }

                panel.AppendJoin(" • ", actions.Select(delegate (CommandNode x)
                {
                    string term = $"`{x.Name}`";

                    if (x.Overloads.Count > 1)
                        term += $"**+{x.Overloads.Count - 1}**";

                    return term;
                }));

                
            }

            return panel.ToString();
        }

        private string GetMainPanel(User user = null)
        {

            bool showReportStatus = user?.Config?.Debug ?? false;
            bool showTooltips = user?.Config?.Tooltips ?? true;

            StringBuilder panel = new StringBuilder();
            panel.AppendLine("> **Help Menu**");

            if (showTooltips)
                panel.AppendLine("> Use `help <name>` to learn more about a command, module, or action.");

            // TODO: Handle report status icon management
            if (Guides.Count > 0)
            {
                panel.AppendLine();
                panel.AppendLine("**Guides**");

                foreach (GuideNode guide in Guides)
                    panel.AppendLine($"> {guide.Tooltip}");
            }

            if (Modules.Count() > 0)
            {
                panel.AppendLine();
                panel.AppendLine("**Categories**");

                foreach(ModuleNode module in GetBaseModules().Select(x => new ModuleNode(x)))
                {
                    panel.Append("> ");

                    if (showReportStatus)
                    {
                        panel.Append(GetSeverityIcon(module));
                    }

                    panel.Append($"**{module.Name}**");

                    if (Check.NotNull(module.Subtitle) || module.Commands.Count > 0)
                        panel.Append(": ");

                    if (Check.NotNull(module.Subtitle))
                        panel.AppendLine(module.Subtitle);

                    if (module.Commands.Count > 0)
                    {
                        if (Check.NotNull(module.Subtitle))
                            panel.Append("> ");

                        int inserted = 0;
                        foreach(CommandNode command in module.Commands.OrderBy(x => x.Name))
                        {
                            if (inserted >= 3)
                                break;

                            if (inserted > 0)
                                panel.Append(" ");

                            panel.Append($"`{command.Name}`");

                            inserted++;
                        }

                        if (module.Commands.Count() - inserted > 0)
                            panel.Append($" (+**{module.Commands.Count() - inserted}** more)");

                        panel.AppendLine();
                    }
                }
            }

            panel.Append(GetActions(user));

            return panel.ToString();
        }

        public string GetPanel(string content = null, User user = null)
        {
            if (!Check.NotNull(content))
                return GetMainPanel(user);

            // TODO: Clean up chapter parsing (Regex).
            bool isGuideName = Guides.Any(x => content.ToLower().StartsWith(x.Id));
            bool hasIndex = isGuideName && content.Split(' ').Count() == 2;

            if (isGuideName)
            {
                if (hasIndex)
                {
                    if (int.TryParse(content.Split(' ')[1], out int index))
                        return Guides.First(x => content.ToLower().StartsWith(x.Id)).GetChapter(index);
                }
                else if (Guides.Any(x => x.Id == content.ToLower()))
                    return Guides.First(x => x.Id == content.ToLower()).GetChapter(1);

            }

            ContextNode ctx = Search(content);

            StringBuilder panel = new StringBuilder();

            if (user?.Config?.Tooltips ?? false)
            {
                if (ctx.Type == InfoType.Command)
                {
                    if ((ctx as CommandNode).Overloads.Count > 1)
                        panel.AppendLine($"> Use `help {ctx.Name}+<index>` to learn more about a specific command method.");
                }
                else if (ctx.Type == InfoType.Group)
                {
                    panel.AppendLine($"> Use `help {ctx.Name} <command>` to learn more about a specific command method within a group.");
                }
            }

            panel.Append(ctx.ToString());


            return panel.ToString();
        }

        public ContextNode Search(string content)
        {
            InfoContext ctx = InfoContext.Parse(content);

            InfoType type = InfoType.Unknown;

            if (!ctx.IsSuccess)
                throw new Exception($"The InfoContext failed to parse: {ctx.ErrorReason}");

            // Get the initial location of the parent to peek
            ModuleInfo module = GetInnerModule(ctx);
            ModuleInfo group = GetInnerGroup(ctx, module);
            IEnumerable<CommandInfo> commands = null;
            ParameterInfo parameter = null;

            // If a type wasn't initialized, find the closest matching value
            type = ctx.Type.GetValueOrDefault(GetValue(ctx.Root, group ?? module).Type);

            switch(type)
            {
                case InfoType.Module:
                    module = GetModule(ctx.Root, group ?? module);
                    break;

                case InfoType.Group:
                    group = GetGroup(ctx.Root, group ?? module);
                    break;

                case InfoType.Command:
                    commands = GetCommands(ctx.Root, group ?? module);
                    break;
            }

            if (type == InfoType.Command)
            {
                if (!Check.NotNullOrEmpty(commands))
                    throw new ResultNotFoundException($"No commands could be found that matched the name '{ctx.Root}'.");

                // If a priority was specified, slim the search down to that specific command
                if (ctx.HasPriority)
                {
                    commands = commands.Where(x => x.Priority == ctx.Priority);

                    if (!Check.NotNullOrEmpty(commands))
                        throw new ResultNotFoundException($"The priority for the specified command does not exist.");

                    if (commands.Count() > 1)
                        throw new MultiMatchException("The priority for the specified command exists in more than one instance.");

                    type = InfoType.Overload;
                }

                // If a parameter was specified, attempt to find the best match possible
                if (Check.NotNull(ctx.Parameter))
                {
                    IEnumerable<ParameterInfo> parameters = new List<ParameterInfo>();
                    commands.ForEach(x => parameters = parameters.Concat(GetParameters(x, ctx.Parameter)));

                    if (!Check.NotNullOrEmpty(parameters))
                        throw new ResultNotFoundException("The parameter specified could not be found within any of the commands");

                    if (parameters.Count() > 1)
                        throw new MultiMatchException($"The parameter enlisted is specified for multiple commands.");

                    parameter = parameters.First();
                    type = InfoType.Parameter;
                }
            }

            // Return the node value based on the finalized type that was found or given
            return type switch
            {
                InfoType.Module => new ModuleNode(module),
                InfoType.Group => new ModuleNode(group),
                InfoType.Overload => new OverloadNode(commands.First()),
                InfoType.Command => new CommandNode(commands),
                InfoType.Parameter => new ParameterNode(parameter),
                _ => throw new ResultNotFoundException("A ContextNode value could not be created with the given context.")
            };
        }

        public List<string> GetAliases(string content)
        {
            try
            {
                ContextNode ctx = Search(content);

                return ctx.Aliases;
            }
            catch (ResultNotFoundException)
            {
                return new List<string>();
            }
        }

        public List<InfoMatch> GetMatchingValues(string name)
        {
            List<InfoMatch> values = GetModules(name).Select(x => new InfoMatch(x)).ToList();

            foreach (ModuleInfo module in Modules)
                values = values.Concat(GetMatchingValues(module, name)).ToList();

            foreach (InfoMatch value in GetCommands(name).Select(x => new InfoMatch(x)))
            {
                if (values.Any(x => x == value))
                    continue;

                values.Add(value);
            }

            return values;
        }

        public List<InfoMatch> GetMatchingValues(ModuleInfo parent, string name)
        {
            if (parent == null)
                return GetMatchingValues(name);

            List<InfoMatch> values = new List<InfoMatch>();

            foreach (InfoMatch value in GetModules(parent, name).Select(x => new InfoMatch(x)))
                values.Add(value);

            foreach(InfoMatch value in GetCommands(name, parent).Select(x => new InfoMatch(x)))
            {
                if (values.Any(x => x == value))
                    continue;

                values.Add(value);
            }

            return values;
        }

        public InfoMatch GetValue(string name, ModuleInfo parent = null)
        {
            List<InfoMatch> values = GetMatchingValues(parent, name);

            Console.WriteLine(string.Join("\n", values.Select(x => x.Name)));

            if (!Check.NotNullOrEmpty(values))
                throw new ResultNotFoundException($"No matches were found when searching for a value of the name '{name}'.");

            if (values.Where(x => x.Type.EqualsAny(InfoType.Module, InfoType.Group)).Count() > 0)
                return values.Where(x => x.Type.EqualsAny(InfoType.Module, InfoType.Group)).First();

            return values.First();
        }


        // REPORTS

        public IEnumerable<Report> GetReports(ModuleInfo module)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Report> GetReports(CommandInfo command)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Report> GetReports(ContextNode node)
        {
            throw new NotImplementedException();
        }


        // MODULES
        public ModuleInfo GetInnerModule(InfoContext ctx)
        {
            ModuleInfo module = null;

            if (!Check.NotNullOrEmpty(ctx.Modules))
                foreach (string name in ctx.Modules)
                    module = GetModule(name, module);

            return module;
        }

        public ModuleInfo GetModule(string name, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> modules = GetModules(parent, name);

            if (!Check.NotNullOrEmpty(modules))
                throw new ResultNotFoundException($"No matches were found when searching for matching modules of the name '{name}'.");

            if (modules.Count() > 1) // Add support for ambiguity.
                throw new MultiMatchException($"Multiple results were given when searching for a module of the name '{name}'.");

            return modules.First();
        }

        public IEnumerable<ModuleInfo> GetBaseModules()
            => Modules.Where(x => x.Parent == null);

        public IEnumerable<ModuleInfo> GetBaseModules(string name)
            => GetBaseModules().Where(m => MODULE_MATCHER.Invoke(m, name));

        // remove all visuals on modules with the HideAttribute.
        public IEnumerable<ModuleInfo> Modules => _commands.Modules.Where(x => !x.Attributes.Any(x => x is IgnoreAttribute));

        public IEnumerable<ModuleInfo> GetModules(string name)
            => Modules.Where(m => MODULE_MATCHER.Invoke(m, name));

        public IEnumerable<ModuleInfo> GetModules(ModuleInfo parent, string name, bool includeChildren = false)
        {
            if (parent == null)
                return GetModules(name);

            IEnumerable<ModuleInfo> modules = parent.Submodules.Where(m => MODULE_MATCHER.Invoke(m, name));

            if (includeChildren)
                parent.Submodules
                    .Select(m => GetModules(m, name, m.Submodules.Count > 0))
                    .ToList()
                    .ForEach(x => modules = modules.Concat(x));

            return modules;
        }


        // GUIDES
        public List<GuideNode> Guides { get; private set; }

        public GuideNode GetGuide(string name)
            => Guides.First(x => x.Id.ToLower() == name.ToLower());

        // GROUPS
        public ModuleInfo GetInnerGroup(InfoContext ctx, ModuleInfo parent = null)
        {
            ModuleInfo group = null;

            if (!Check.NotNullOrEmpty(ctx.Groups))
                foreach (string name in ctx.Groups)
                    group = GetGroup(name, group ?? parent);
            
            return group;
        }

        public ModuleInfo GetGroup(string name, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> groups = GetGroups(parent, name);

            if (!Check.NotNullOrEmpty(groups))
                throw new ResultNotFoundException($"No matches were found when searching for matching groups of the name '{name}'.");

            if (groups.Count() > 1) // Add support for ambiguity.
                throw new MultiMatchException($"Multiple results were given when searching for a group of the name '{name}'.");

            return groups.First();
        }

        public IEnumerable<ModuleInfo> GetGroups(string name)
            => Modules.Where(g => GROUP_MATCHER.Invoke(g, name));

        public IEnumerable<ModuleInfo> GetGroups(ModuleInfo parent, string name, bool includeChildren = false)
        {
            if (parent == null)
                return GetGroups(name);

            IEnumerable<ModuleInfo> groups = parent.Submodules.Where(g => GROUP_MATCHER.Invoke(g, name));

            if (includeChildren)
                parent.Submodules
                    .Select(x => GetGroups(x, name, x.Submodules.Count > 0))
                    .ToList()
                    .ForEach(x => groups = groups.Concat(x));

            return groups;
        }


        // COMMANDS
        public IEnumerable<CommandInfo> Commands => _commands.Commands;


        public IEnumerable<CommandInfo> GetCommands(string name)
            => Commands.Where(c => COMMAND_MATCHER.Invoke(c, name));

        public IEnumerable<CommandInfo> GetCommands(string name, ModuleInfo parent, bool includeChildren = false)
        {
            if (parent == null)
                return GetCommands(name);

            IEnumerable<CommandInfo> commands = parent.Commands.Where(x => x.Aliases.Contains(name.ToLower()));

            //parent.Commands.ForEach(x => Console.WriteLine("COMMAND::" + x.Name + ":: " + string.Join(", ", x.Aliases)));

            if (!Check.NotNull(parent.Group))
                if (includeChildren)
                    parent.Submodules.Select(x => GetCommands(name, x)).ToList().ForEach(x => commands = commands.Concat(x));

            return commands;
        }

        public IEnumerable<ParameterInfo> GetParameters(string name)
        {
            IEnumerable<CommandInfo> commands = Commands.Where(x => x.Parameters.Any(p => PARAM_MATCHER.Invoke(p, name)));
            IEnumerable<ParameterInfo> parameters = new List<ParameterInfo>();

            commands.ToList().ForEach(x => parameters = parameters.Concat(x.Parameters));

            return parameters;
        }

        public IEnumerable<ParameterInfo> GetParameters(CommandInfo command, string name)
            => command.Parameters.Where(p => PARAM_MATCHER.Invoke(p, name));
    }
}
