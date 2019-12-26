using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Orikivo
{
    // TODO: Integrate ReportContainer for the help service.
    /// <summary>
    /// A help service which provides information on all commands as an override to <see cref="CommandService"/>.
    /// </summary>
    public class InfoService
    {
        private readonly CommandService _commands; // root service
        private readonly IEnumerable<IReport> _reports; // optional collection of reports
        private readonly OriGuild _guild; // optional guild
        
        /// <summary>
        /// Constructs a generic help service.
        /// </summary>
        public InfoService(CommandService commandService, OriGlobal global)
        {
            Console.WriteLine("[Debug] -- Created a new help service. --");
            _commands = commandService; // Predicate<T>
            _reports = global.Reports?.Reports?.Cast<IReport>() ?? new List<IReport>();
        }

        /// <summary>
        /// Constructs a help service with information from a guild.
        /// </summary>
        public InfoService(CommandService commandService, OriGlobal global, OriGuild guild)
            : this(commandService, global)
        {
            _guild = guild;
        }

        public int GetReportCount(string moduleName)
        {
            return _reports.Where(x => GetRootModuleFromId(x.CommandId) == moduleName.ToLower()).Count();
        }

        private string GetRootModuleFromId(string commandId)
            => commandId.Split('.')[0];

        public string CreateDefaultContent()
        {
            List<ModuleDisplayInfo> displayModules = GetRootModuleDatas();

            StringBuilder sb = new StringBuilder();
            // Custom Info...?
            sb.AppendLine($"**Orikivo**\n**Modules** {OriFormat.Subscript($"({displayModules.Count})")}:");
            sb.AppendLine(string.Join("\n", displayModules.Select(x => WriteModule(x.Name, x.TotalCommands, GetReportCount(x.Name), x.Subtitle))));

            if (Checks.NotNullOrEmpty(_guild.Options.Commands))
                sb.Append(WriteModule("Custom", _guild.Options.Commands.Count));
            
            return sb.ToString();
        }

        // $"🔹Custom {OriFormat.Subscript($"(+{_guild.Options.Commands.Count})")}"
        private string WriteModule(string moduleName, int commandCount, int reportCount = 0, string subtitle = null)
            => $"{ContextUtils.GetSeverityIcon(reportCount)}{moduleName} {OriFormat.Subscript($"(+{commandCount})")}{(Checks.NotNull(subtitle) ? $": {subtitle}" : "")}";

        public string GetHelpFor(string content)
        {
            if (!Checks.NotNull(content))
                return CreateDefaultContent();

            ContextSearchResult result = GetContext(content);

            if (!result.IsSuccess)
                return result.Error.Value.ToString();

            return result.Value?.Content ?? CreateDefaultContent();
        }

        // TODO: Permit custom command search.
        // TODO: Create group searches that can be separate from their command name variant.

        /// <summary>
        /// Searches for and returns the closest matching value to the context specified.
        /// </summary>
        public ContextResult Search(string content)
            => Search(ContextInfo.Parse(content));

        public GuildCommand GetMainCustom(string customName)
        {
            IEnumerable<GuildCommand> customs = _guild.Options.Commands.Where(x => x.Name == customName);

            if (!Checks.NotNullOrEmpty(customs))
                throw new Exception("The custom command specified did not match any of the commands within the result.");

            if (customs.Count() > 1)
                throw new MultiMatchException("The custom command specified matched with multiple commands.", customs);

            return customs.First();
        }

        /// <summary>
        /// Searches for and returns the closest matching value to the context specified.
        /// </summary>
        public ContextResult Search(ContextInfo ctx)
        {
            ContextInfoType? type = null;

            //Console.WriteLine(0);
            if (!ctx.IsSuccess)
            {
                return ContextResult.FromError(ContextError.EmptyValue);
            }
            //Console.WriteLine(1);
            ModuleInfo module = null; // The inner-most module
            ModuleInfo group = null; // The inner-most group
            IEnumerable<CommandInfo> commands = null;
            ParameterInfo parameter = null;

            if (Checks.NotNull(_guild))
            {
                if (Checks.NotNullOrEmpty(_guild.Options.Commands))
                {
                    if (ctx.Modules.Count == 1)
                    {
                        if (ctx.Modules[0].ToLower() == "custom")
                        {
                            if (ctx.HasRoot)
                                return ContextResult.FromSuccess(GetMainCustom(ctx.Root));
                        }
                    }
                    else if (ctx.HasRoot)
                    {
                        if (ctx.Root == "custom")
                            return ContextResult.FromSuccess(_guild);
                    }
                }
            }

           // Console.WriteLine(2);
            module = GetInnerModule(ctx);
            group = GetInnerGroup(ctx, module);

            if (ctx.HasRoot)
            {
                Console.WriteLine(4);
                switch (ctx.Type)
                {
                    case ContextInfoType.Module:
                        //Console.WriteLine(40);
                        module = GetMainModule(ctx.Root, group ?? module);
                        break;

                    case ContextInfoType.Group:
                        //Console.WriteLine(41);
                        group = GetMainGroup(ctx.Root, group ?? module);
                        break;

                    case ContextInfoType.Command:
                        //Console.WriteLine(42);
                        commands = GetCommands(ctx.Root, group ?? module);
                        break;

                    default:
                        //Console.WriteLine(43);
                        ContextValue value = GetMainValue(ctx.Root, group ?? module);

                        switch (value.Type)
                        {
                            case ContextInfoType.Module:
                                //Console.WriteLine(430);
                                module = GetMainModule(ctx.Root, group ?? module);
                                break;

                            case ContextInfoType.Group:
                                //Console.WriteLine(431);
                                group = GetMainGroup(ctx.Root, group ?? module);
                                break;

                            default:
                                //Console.WriteLine(432);
                                commands = GetCommands(ctx.Root, group ?? module);
                                break;
                        }

                        type = value.Type;
                        break;
                }

                type = ctx.Type ?? type;
            }

            //Console.WriteLine(5);
            if (type != ContextInfoType.Command)
            {
                if (ctx.HasParameter || ctx.HasPriority)
                    throw new Exception("The context specified does not support priority or argument specification.");
            }
            else
            {
                if (!Checks.NotNullOrEmpty(commands))
                    throw new Exception("The context specified did not match any of the existing commands.");

                //Console.WriteLine(50);
                if (ctx.HasPriority) // Command+1
                {
                    commands = commands.Where(x => x.Priority == ctx.Priority);

                    if (!Checks.NotNullOrEmpty(commands))
                        throw new Exception("The priority specified did not match any of the commands within the result.");

                    if (commands.Count() > 1)
                        throw new MultiMatchException("The priority specified matched with multiple commands.", commands);
                }

                //Console.WriteLine(51);
                if (ctx.HasParameter) // Arg
                {
                    IEnumerable<ParameterInfo> args = new List<ParameterInfo>();

                    commands.ForEach(x => args = args.Concat(GetArgs(x, ctx.Parameter)));

                    if (!Checks.NotNullOrEmpty(args))
                        throw new Exception("The argument specified did not match any of the commands within the result.");

                    if (args.Count() > 1)
                        throw new MultiMatchException("The argument specified matched with multiple commands.", args);

                    parameter = args.First();
                    type = ContextInfoType.Parameter;
                }
            }

            //Console.WriteLine(6);
            return type switch
            {
                ContextInfoType.Parameter
                => ContextResult.FromSuccess(parameter),

                ContextInfoType.Command
                => ContextResult.FromSuccess(commands),

                ContextInfoType.Group
                => ContextResult.FromSuccess(group),

                ContextInfoType.Module
                => ContextResult.FromSuccess(module),

                _
                => ContextResult.FromError(ContextError.FailedMatch),
            };
        }

        /// <summary>
        /// Recursively searches for and attempts to get the internal module the context is pointing to.
        /// </summary>
        /// <returns>A ModuleInfo that acts at the internal module to search with. If there was no internal module pointer, it returns null.</returns>
        public ModuleInfo GetInnerModule(ContextInfo context)
        {
            ModuleInfo module = null;

            if (!Checks.NotNullOrEmpty(context.Modules))
            {
                foreach (string moduleName in context.Modules) // Get the inner most module provided in the search until a collision occurs.
                {
                    module = GetMainModule(moduleName, module);
                }
            }

            return module;
        }

        public ModuleInfo GetInnerGroup(ContextInfo context, ModuleInfo parent = null)
        {
            ModuleInfo group = null;

            if (Checks.NotNullOrEmpty(context.Groups))
            {
                foreach (string groupName in context.Groups)
                {
                    group = GetMainGroup(groupName, group ?? parent);
                }
            }

            return group;
        }

        /// <summary>
        /// Attempts to search for and return the first module that matches with its specified name.
        /// </summary>
        public ModuleInfo GetMainModule(string moduleName, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> modules = GetModules(parent, moduleName);

            if (!Checks.NotNullOrEmpty(modules))
                throw new ResultNotFoundException($"No matches were found{(Checks.NotNull(parent) ? $" within {parent.Name}" : "")} when searching for modules with the name '{moduleName}'.");

            if (modules.Count() > 1)
                throw new MultiMatchException($"Multiple results were given when searching for a module with the name '{moduleName}'.", modules);

            return modules.First();
        }

        /// <summary>
        /// Attempts to search for and return the first group that matches with its specified name.
        /// </summary>
        public ModuleInfo GetMainGroup(string groupName, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> groups = GetGroups(parent, groupName);

            if (!Checks.NotNullOrEmpty(groups))
                throw new ResultNotFoundException($"No matches were found{(Checks.NotNull(parent) ? $" within {parent.Name}" : "")} when searching for groups of the name '{groupName}'.");

            if (groups.Count() > 1)
                throw new MultiMatchException($"Multiple results were given when searching for a group with the name '{groupName}'.", groups);

            return groups.First();
        }

        /// <summary>
        /// Attempts to search for and return the first generic value that matches with its specified name.
        /// </summary>
        public ContextValue GetMainValue(string valueName, ModuleInfo parent = null)
        {
            List<ContextValue> values = GetMatchingValues(parent, valueName);
           // Console.WriteLine(string.Join('\n', values.Select(x => $"{x.Name}: {x.Type}")));

            if (!Checks.NotNullOrEmpty(values))
                throw new ResultNotFoundException($"No matches were found{(Checks.NotNull(parent) ? $" within {parent.Name}" : "")} when searching for a value with the name '{valueName}'.");

            //if (values.Count() > 1 && values.Where(x => x.Type.EqualsAny(ContextInfoType.Module, ContextInfoType.Group)).Count() > 1)
            //    throw new MultiMatchException($"Multiple results were given when searching for a value with the name '{valueName}'.", values);
            if (values.Where(x => x.Type.EqualsAny(ContextInfoType.Module, ContextInfoType.Group)).Count() > 0) // == 1
                return values.Where(x => x.Type.EqualsAny(ContextInfoType.Module, ContextInfoType.Group)).First();
            return values.First();
        }

        public ContextSearchResult GetContext(string content)
        {
            ContextInfo ctx = ContextInfo.Parse(content);
            ContextResult result = Search(content);

            if (!result.IsSuccess)
                return ContextSearchResult.FromError(result.Error ?? ContextError.UnknownError);

            return result.Type switch
            {
                ContextInfoType.Guild
                => ContextSearchResult.FromSuccess(new GuildDisplayInfo(result.Guild) as IDisplayInfo),

                ContextInfoType.Custom
                => ContextSearchResult.FromSuccess(new CustomDisplayInfo(_guild, result.Custom) as IDisplayInfo),

                ContextInfoType.Parameter
                => ContextSearchResult.FromSuccess(new ParameterDisplayInfo(result.Parameter) as IDisplayInfo),

                ContextInfoType.Command 
                => ContextSearchResult.FromSuccess(result.Commands.Count() > 1 || ctx.HasPriority ?
                new OverloadDisplayInfo(result.Commands.First()) as IDisplayInfo
                : new CommandDisplayInfo(result.Commands.ToList()) as IDisplayInfo),

                ContextInfoType.Overload
                => ContextSearchResult.FromSuccess(new OverloadDisplayInfo(result.Commands.First()) as IDisplayInfo),

                ContextInfoType.Group
                => ContextSearchResult.FromSuccess(new ModuleDisplayInfo(result.Module) as IDisplayInfo),

                ContextInfoType.Module
                => ContextSearchResult.FromSuccess(new ModuleDisplayInfo(result.Module) as IDisplayInfo),

                _
                => ContextSearchResult.FromError(result.Error ?? ContextError.FailedMatch),
            };
        }

        // TODO: Make all methods derive from ContextInfo. This way, the search method can be the same across all services.
        public List<string> GetAliasesFor(string commandId)
            => GetAliases(commandId.Substring(VarBase.COMMAND_SUBST.Length));

        // Get all known aliases for a context.
        public List<string> GetAliases(string context)
        {
            SearchResult result = _commands.Search(context); // search for a command with priority +
            if (!result.IsSuccess)
                return new List<string>();
            CommandInfo command = result.Commands.First().Command;
            List<string> aliases = command.Aliases.ToList();
            aliases.ForEach(x => x.Concat($"+{command.Priority}")); // make it to where you get the aliases of the main command, including its priority.
            return aliases;
        }

        /// <summary>
        /// Gets all generic context values that match a specified name.
        /// </summary>
        private List<ContextValue> GetMatchingValues(string name)
        {
            List<ContextValue> values = GetModules(name).Select(x => new ContextValue(x)).ToList();
            foreach (ModuleInfo module in Modules)
            {
                values = values.Concat(GetMatchingValues(module, name)).ToList();
            }
            foreach (ContextValue value in GetCommands(name).Select(x => new ContextValue(x)))
            {
                Console.WriteLine(value);
                if (values.Any(x => x == value))
                    continue;
                values.Add(value);
            }

            //Console.WriteLine("________");
            //Console.WriteLine(string.Join('\n', values.Select(x => x.ToString())));
            return values;
        }

        /// <summary>
        /// Gets all generic context values within a module that match a specified name.
        /// </summary>
        private List<ContextValue> GetMatchingValues(ModuleInfo module, string name)
        {
            if (module == null)
                return GetMatchingValues(name);

            List<ContextValue> values = new List<ContextValue>();
            foreach (ContextValue value in GetModules(module, name).Select(x => new ContextValue(x)))
            {
                //Console.WriteLine(value);
                values.Add(value);
            }
            // this is to remove duplicate commands.
            foreach (ContextValue value in GetCommands(name, module).Select(x => new ContextValue(x)))
            {
                //Console.WriteLine(value);
                if (values.Any(x => x == value))
                    continue;
                values.Add(value);
            }

            //Console.WriteLine("________");
            //Console.WriteLine(string.Join('\n', values.Select(x => x.ToString())));
            return values;
        }

        /// <summary>
        /// Gets all modules that match a specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ModuleInfo GetModule(string name)
        {
            try
            {
                return _commands.Modules.First(x => x.Name.ToLower() == name.ToLower());
            }
            catch(ArgumentNullException)
            {
                throw new Exception($"There is no ModuleInfo with the name \"{name}\".");
            }
        }

        public IEnumerable<ModuleInfo> Modules => _commands.Modules;
        public IEnumerable<CommandInfo> Commands => _commands.Commands;
        public IReadOnlyList<GuildCommand> CustomCommands => _guild?.Options.Commands;

        public Dictionary<Type, string> TypeInfos { get; private set; }


        private IEnumerable<ModuleInfo> GetParentModules()
            => Modules.Where(x => x.Group == null);
        // Get all modules by name
        private IEnumerable<ModuleInfo> GetParentModules(string name)
            => GetParentModules().Where(x => x.Name.ToLower() == name.ToLower()); // separate into own service.

        private IEnumerable<ModuleInfo> GetModules(string name)
            => Modules.Where(x => x.Name.ToLower() == name.ToLower());

        private IEnumerable<ModuleInfo> GetGroups(string name)
            => Modules.Where(x => x.Group?.ToLower() == name.ToLower());

        private IEnumerable<CommandInfo> GetCommands(string name)
            => Commands.Where(x => x.Aliases.Contains(name.ToLower()));

        private IEnumerable<ModuleInfo> GetModules(ModuleInfo parent, string name, bool includeChildren = false)
        {
            if (parent == null)
                return GetModules(name);
            IEnumerable<ModuleInfo> modules = parent.Submodules.Where(x => x.Name.ToLower() == name.ToLower());
            if (includeChildren)
                parent.Submodules.Select(x => GetModules(x, name, x.Submodules.Count > 0)).ToList().ForEach(x => modules = modules.Concat(x));
            return modules;
        }

        private IEnumerable<ModuleInfo> GetGroups(ModuleInfo parent, string name, bool includeChildren = false)
        {
            if (parent == null)
                return GetGroups(name);
            IEnumerable<ModuleInfo> groups = parent.Submodules.Where(x => x.Group.ToLower() == name.ToLower());
            if (includeChildren)
                parent.Submodules.Select(x => GetGroups(x, name, x.Submodules.Count > 0)).ToList().ForEach(x => groups = groups.Concat(x));
            return groups;
        }

        private IEnumerable<CommandInfo> GetCommands(string name, ModuleInfo parent = null, bool includeChildren = false)
        {
            if (parent == null)
                return GetCommands(name);

            IEnumerable<CommandInfo> commands = parent.Commands.Where(x => x.Aliases.Contains(name.ToLower()));

            if (includeChildren)
                parent.Submodules.Select(x => GetCommands(name, x)).ToList().ForEach(x => commands = commands.Concat(x));

            return commands;
        }

        /// <summary>
        /// Gets all parameters from a command matching a specified name.
        /// </summary>
        private IEnumerable<ParameterInfo> GetArgs(CommandInfo parent, string parameter)
            => parent.Parameters.Where(x => x.Name.ToLower() == parameter.ToLower());
        
        /// <summary>
        /// Gets that summary information of all parent modules.
        /// </summary>
        public List<ModuleDisplayInfo> GetRootModuleDatas()
        {
            List<ModuleDisplayInfo> modules = new List<ModuleDisplayInfo>();
            foreach (ModuleInfo module in Modules.Where(x => x.Parent == null))
                modules.Add(new ModuleDisplayInfo(module));
            return modules;
        }

        public IEnumerable<ModuleInfo> RootModules => Modules.Where(x => !Checks.NotNull(x.Parent));
    }
}
