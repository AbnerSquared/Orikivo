using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Orikivo.Unstable;

namespace Orikivo
{
    // TODO: Integrate ReportContainer for the help service.
    /// <summary>
    /// A help service which provides information on all commands as an override to <see cref="CommandService"/>.
    /// </summary>
    public class InfoService
    {
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

        private string GetMainPanel(User user = null)
        {

            bool showReportStatus = user?.Config?.Debug ?? false;
            bool showTooltips = user?.Config?.Tooltips ?? true;

            StringBuilder panel = new StringBuilder();
            panel.Append("> **Help Menu**");

            if (showTooltips)
            {
                panel.AppendLine();
                panel.AppendLine("> Use `help <name>` to learn more about a command, module, or action.");
            }

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
                panel.AppendLine("**Modules**");

                foreach(ModuleNode module in GetBaseModules().Select(x => new ModuleNode(x)))
                {
                    panel.Append($"> **{module.Name}**");

                    if (Checks.NotNull(module.Subtitle) || module.Commands.Count > 0)
                        panel.Append(": ");

                    if (Checks.NotNull(module.Subtitle))
                        panel.AppendLine(module.Subtitle);

                    if (module.Commands.Count > 0)
                    {
                        if (Checks.NotNull(module.Subtitle))
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

            // TODO: Deal with actions later

            return panel.ToString();
        }

        public string GetPanel(string content = null, User user = null)
        {
            if (!Checks.NotNull(content))
                return GetMainPanel(user);

            // TODO: Include chapter parsing
            if (Guides.Any(x => x.Id == content.ToLower()))
                return Guides.First(x => x.Id == content.ToLower()).GetChapter(1);

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

            // get the initial location of where to search
            ModuleInfo module = GetInnerModule(ctx);
            ModuleInfo group = GetInnerGroup(ctx, module);
            IEnumerable<CommandInfo> commands = null;
            ParameterInfo parameter = null;

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

            // COMMAND
            if (type == InfoType.Command)
            {
                if (!Checks.NotNullOrEmpty(commands))
                    throw new ResultNotFoundException($"No commands could be found that matched the name '{ctx.Root}'.");

                // COMMAND+PRIORITY
                if (ctx.HasPriority)
                {
                    commands = commands.Where(x => x.Priority == ctx.Priority);

                    if (!Checks.NotNullOrEmpty(commands))
                        throw new ResultNotFoundException($"The priority for the specified command does not exist.");

                    if (commands.Count() > 1)
                        throw new MultiMatchException("The priority for the specified command exists in more than one instance.");

                    type = InfoType.Overload;
                }

                // COMMAND(PARAMETER
                if (Checks.NotNull(ctx.Parameter))
                {
                    IEnumerable<ParameterInfo> parameters = new List<ParameterInfo>();

                    commands.ForEach(x => parameters = parameters.Concat(GetParameters(x, ctx.Parameter)));

                    if (!Checks.NotNullOrEmpty(parameters))
                        throw new ResultNotFoundException("The parameter specified could not be found within any of the commands");

                    if (parameters.Count() > 1)
                        throw new MultiMatchException($"The parameter enlisted is specified for multiple commands.");

                    parameter = parameters.First();
                    type = InfoType.Parameter;
                }
            }

            // RETURNING
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

        public List<ContextValue> GetMatchingValues(string name)
        {
            List<ContextValue> values = GetModules(name).Select(x => new ContextValue(x)).ToList();

            foreach (ModuleInfo module in Modules)
                values = values.Concat(GetMatchingValues(module, name)).ToList();

            foreach (ContextValue value in GetCommands(name).Select(x => new ContextValue(x)))
            {
                if (values.Any(x => x == value))
                    continue;

                values.Add(value);
            }

            return values;
        }

        public List<ContextValue> GetMatchingValues(ModuleInfo parent, string name)
        {
            if (parent == null)
                return GetMatchingValues(name);

            List<ContextValue> values = new List<ContextValue>();

            foreach (ContextValue value in GetModules(parent, name).Select(x => new ContextValue(x)))
                values.Add(value);

            foreach(ContextValue value in GetCommands(name, parent).Select(x => new ContextValue(x)))
            {
                if (values.Any(x => x == value))
                    continue;

                values.Add(value);
            }

            return values;
        }

        public ContextValue GetValue(string name, ModuleInfo parent = null)
        {
            List<ContextValue> values = GetMatchingValues(parent, name);

            Console.WriteLine(string.Join("\n", values.Select(x => x.Name)));

            if (!Checks.NotNullOrEmpty(values))
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

            if (!Checks.NotNullOrEmpty(ctx.Modules))
                foreach (string name in ctx.Modules)
                    module = GetModule(name, module);

            return module;
        }

        public ModuleInfo GetModule(string name, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> modules = GetModules(parent, name);

            if (!Checks.NotNullOrEmpty(modules))
                throw new ResultNotFoundException($"No matches were found when searching for matching modules of the name '{name}'.");

            if (modules.Count() > 1) // Add support for ambiguity.
                throw new MultiMatchException($"Multiple results were given when searching for a module of the name '{name}'.");

            return modules.First();
        }

        public IEnumerable<ModuleInfo> GetBaseModules()
            => Modules.Where(x => x.Parent == null);

        public IEnumerable<ModuleInfo> GetBaseModules(string name)
            => GetBaseModules().Where(m => MODULE_MATCHER.Invoke(m, name));

        public IEnumerable<ModuleInfo> Modules => _commands.Modules;

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

            if (!Checks.NotNullOrEmpty(ctx.Groups))
                foreach (string name in ctx.Groups)
                    group = GetGroup(name, group ?? parent);
            
            return group;
        }

        public ModuleInfo GetGroup(string name, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> groups = GetGroups(parent, name);

            if (!Checks.NotNullOrEmpty(groups))
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

            if (!Checks.NotNull(parent.Group))
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
