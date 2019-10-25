using Discord;
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
    public class OriHelpService /*: IDisposable (Determine if this is really needed) */ 
    {
        private readonly CommandService _commandService;
        private readonly IReportContainer<IReport> _reports;
        private readonly List<CustomGuildCommand> _customCommands;
        
        /// <summary>
        /// Constructs a generic help service.
        /// </summary>
        public OriHelpService(CommandService commandService, OriGlobal global)
        {
            Console.WriteLine("[Debug] -- Created a new help service. --");
            _commandService = commandService;
            _reports = (IReportContainer<IReport>)global.Reports;
        }

        /// <summary>
        /// Constructs a help service with information from a guild.
        /// </summary>
        public OriHelpService(CommandService commandService, OriGlobal global, OriGuild guild) : this(commandService, global)
        {          
            _customCommands = guild.CustomCommands.Count > 0 ? guild.CustomCommands : null;
        }

        public string CreateDefaultContent()
        {
            List<ModuleDisplayInfo> displayModules = GetModules();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**Orikivo**\n**Modules** {OriFormat.Subscript($"({displayModules.Count})")}:");
            sb.AppendLine(string.Join("\n", displayModules.Select(x => $"🔹{x.Name} {OriFormat.Subscript($"(+{x.TotalCommands})")}{(x.Subtitle != null ? $": {x.Subtitle}" : "")}")));
            if (_customCommands != null)
                sb.Append($"🔹Custom {OriFormat.Subscript($"(+{_customCommands.Count})")}");
            return sb.ToString();
        }

        public string GetHelpFor(string content)
        {
            if (!Checks.NotNull(content))
                return CreateDefaultContent();
            return Search(content).Result?.Content;
        }

        // TODO: Permit custom command search.
        // TODO: Create group searches that can be separate from their command name variant.
        // TODO: Create ambiguity catchers.
        // TODO: Completely rewrite (again) how the context info is determined step-by-step.
        public ContextSearchResult Search(string content)
        {
            ContextInfo ctx = ContextInfo.Parse(content);
            ContextSearchResult result = new ContextSearchResult();
            ContextInfoType? resultType = null;

            if (!ctx.IsSuccess)
            {
                result.Error = ContextError.EmptyValue;
                return result;
            }

            ModuleInfo module = null;
            IEnumerable<ModuleInfo> modules = null;
            ModuleInfo group = null;
            IEnumerable<ModuleInfo> groups = null;
            CommandInfo command = null;
            IEnumerable<CommandInfo> commands = null;
            ParameterInfo parameter = null;
            IEnumerable<ParameterInfo> args = null;
            Console.WriteLine("[Debug] -- 1 --"); // Search all of the modules for a matching name if there is any.
            if (ctx.Modules != null)
            {
                if (ctx.Modules.Count > 0)
                {
                    module = GetModule(ctx.Modules[0]);
                    foreach (string name in ctx.Modules.Skip(1))
                    {
                        // gets all modules within the current module set
                        modules = GetModules(module, name);
                        if (modules.Count() == 0)
                            throw new Exception($"There were no matching ModuleInfo values found for the name \"{name}\".");
                        // if (modules.Count() > 1)
                        // handle having multiple matching values
                        module = modules.First();
                    }
                }
            }
            Console.WriteLine("[Debug] -- 2 --"); // Search all of the groups for a matching name if there is any.
            if (ctx.Groups != null)
            {
                foreach (string name in ctx.Groups)
                {
                    groups = GetGroups(group ?? module, name);
                    if (groups.Count() == 0)
                        throw new Exception($"There were no matching ModuleInfo group values found for the name \"{name}\".");
                    group = groups.First();
                }
            }
            Console.WriteLine("[Debug] -- 3 --");
            if (ctx.HasRoot)
            {
                switch (ctx.Type)
                {
                    case ContextInfoType.Module:
                        modules = GetModules(group ?? module, ctx.Root);
                        if (modules.Count() == 0)
                            throw new Exception($"There were no matching ModuleInfo values found for the name \"{ctx.Root}\".");
                        module = modules.First();
                        break;

                    case ContextInfoType.Group:
                        groups = GetGroups(group ?? module, ctx.Root);
                        if (groups.Count() == 0)
                            throw new Exception($"There were no matching ModuleInfo group values found for the name \"{ctx.Root}\".");
                        group = groups.First();
                        break;

                    case ContextInfoType.Command:
                        commands = GetCommands(group ?? module, ctx.Root);
                        break;

                    default:
                        List<ContextValue> values = GetMatchingValues(group ?? module, ctx.Root);
                        if (values.Count == 0)
                            throw new Exception($"There were no matching ContextValue values found for the name \"{ctx.Root}\".");
                        switch (values[0].Type)
                        {
                            case ContextInfoType.Module:
                                modules = GetModules(group ?? module, ctx.Root);
                                if (modules.Count() == 0)
                                    throw new Exception($"There were no matching ModuleInfo values found for the name \"{ctx.Root}\".");
                                module = modules.First();
                                break;

                            case ContextInfoType.Group:
                                groups = GetGroups(group ?? module, ctx.Root);
                                if (groups.Count() == 0)
                                    throw new Exception($"There were no matching ModuleInfo group values found for the name \"{ctx.Root}\".");
                                group = groups.First();
                                break;
                            default:
                                commands = GetCommands(group ?? module, ctx.Root);
                                break;
                        }
                        resultType = values[0].Type;
                        break;
                }
                resultType = ctx.Type ?? resultType;
            }
            Console.WriteLine("[Debug] -- 4 --");
            if (ctx.HasPriority)
            {
                if (ctx.Type.HasValue)
                    if (ctx.Type != ContextInfoType.Command)
                        throw new Exception("Only CommandInfo values can be indexed with an overload.");
                if (commands == null)
                    throw new Exception("There are no CommandInfo values to look through.");
                if (!commands.Any(x => x.Priority == ctx.Priority))
                    throw new Exception($"There were no CommandInfo values with a priority of '{ctx.Priority}'.");

                command = commands.Where(x => x.Priority == ctx.Priority).First();
            }
            Console.WriteLine("[Debug] -- 5 --");
            if (ctx.HasArg)
            {
                if (ctx.Type.HasValue)
                    if (ctx.Type != ContextInfoType.Command)
                        throw new Exception("Only CommandInfo values can be indexed with an argument.");

                args = command != null ? GetArgs(command, ctx.Arg) : new List<ParameterInfo>();
                if (command == null)
                {
                    if (commands != null)
                        commands.ToList().ForEach(x => args = args.Concat(GetArgs(x, ctx.Arg)));
                    else
                        throw new Exception("There are no CommandInfo values to look through for a parameter.");
                }
                if (args.Count() == 0)
                    throw new Exception($"There were no matching ParameterInfo values for the parameter \"{ctx.Arg}\".");
                parameter = args.First();
                resultType = ContextInfoType.Parameter;
            }
            Console.WriteLine("[Debug] -- 6 --");
            switch (result.Type)
            {
                case ContextInfoType.Parameter:
                    result.Result = new ParameterDisplayInfo(parameter);
                    break;
                case ContextInfoType.Command:
                    if (command != null)
                    {
                        if (commands.Count() > 1 || ctx.HasPriority)
                        {
                            result.Result = new OverloadDisplayInfo(command);
                        }
                        else
                            result.Result = new CommandDisplayInfo(command);
                    }
                    else
                        result.Result = new CommandDisplayInfo(commands.ToList());
                    break;
                case ContextInfoType.Group:
                    result.Result = new ModuleDisplayInfo(group);
                    break;
                case ContextInfoType.Module:
                    result.Result = new ModuleDisplayInfo(module);
                    break;
            }

            return result;
        }


        // TODO: Make all methods derive from ContextInfo. This way, the search method can be the same across all services.

        // Get all known aliases for a context.
        public List<string> GetAliases(string context)
        {
            SearchResult result = _commandService.Search(context);
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
            foreach (ModuleInfo module in _commandService.Modules)
                values = values.Concat(GetMatchingValues(module, name)).ToList();

            foreach (ContextValue value in GetCommands(name).Select(x => new ContextValue(x)))
            {
                if (values.Any(x => x.Name == value.Name && x.Type == value.Type))
                    continue;
                values.Add(value);
            }
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
                values.Add(value);
            // this is to remove duplicate commands.
            foreach (ContextValue value in GetCommands(module, name).Select(x => new ContextValue(x)))
            {
                if (values.Any(x => x.Name == value.Name && x.Type == value.Type))
                    continue;
                values.Add(value);
            }
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
                return _commandService.Modules.First(x => x.Name.ToLower() == name.ToLower());
            }
            catch(ArgumentNullException)
            {
                throw new Exception($"There is no ModuleInfo with the name \"{name}\".");
            }
        }

        public IEnumerable<ModuleInfo> Modules => _commandService.Modules;
        public IEnumerable<CommandInfo> Commands => _commandService.Commands;
        public IReadOnlyList<CustomGuildCommand> CustomCommands => _customCommands ?? null;

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

        private IEnumerable<CommandInfo> GetCommands(ModuleInfo parent, string name, bool includeChildren = false)
        {
            if (parent == null)
                return GetCommands(name);

            IEnumerable<CommandInfo> commands = parent.Commands.Where(x => x.Aliases.Contains(name.ToLower()));

            if (includeChildren)
                parent.Submodules.Select(x => GetCommands(x, name)).ToList().ForEach(x => commands = commands.Concat(x));

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
        public List<ModuleDisplayInfo> GetModules()
        {
            List<ModuleDisplayInfo> modules = new List<ModuleDisplayInfo>();
            foreach (ModuleInfo module in Modules.Where(x => x.Parent == null))
                modules.Add(new ModuleDisplayInfo(module));
            return modules;
        }

        // make private functions that organize how specific information is displayed on the corresponding types

        // be able to gather information on commands regarding reports, and alter the embed display based on
        // those reports mentioned

        // offer generalized info about other information the user might need
        // such as syntax, reading types, definitions, and examples of other systems on orikivo

        // 🔹 stable
        // 🔸 unstable
        // 🔺 critical
    }
}
