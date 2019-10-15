using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Orikivo
{
    // used to get info on commands, modules, and reports statuses.
    public class OriHelpService : IDisposable
    {
        private readonly CommandService _commandService;
        private readonly List<CustomGuildCommand> _customCommands;
        
        public OriHelpService(CommandService commandService) // include report data alongside command data
        {
            Console.WriteLine("[Debug] -- Built a helper thread. --");
            _commandService = commandService;
        }

        public OriHelpService(CommandService commandService, OriGuild guild)
        {
            _commandService = commandService;
            // if there aren't any, why keep the list?
            
            _customCommands = guild.CustomCommands.Count > 0 ? guild.CustomCommands : null;
        }

        // include customCommands
        public string GetDefaultInfo()
        {
            List<ModuleDisplayInfo> displayModules = GetModules();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**Orikivo**\n**Modules** {OriFormat.Subscript($"({displayModules.Count})")}:");
            sb.AppendLine(string.Join("\n", displayModules.Select(x => $"🔹{x.Name} {OriFormat.Subscript($"(+{x.TotalCommands})")}{(x.FlavorText != null ? $": {x.FlavorText}" : "")}")));
            if (_customCommands != null)
                sb.Append($"🔹Custom {OriFormat.Subscript($"(+{_customCommands.Count})")}");
            return sb.ToString();
        }

        // allow custom command and aliases search index
        // fix group searches
        public ContextSearchResult Search(string content)
        {
            ContextInfo ctx = ContextInfo.Parse(content);
            ContextSearchResult result = new ContextSearchResult();
            ContextInfoType? resultType = null;

            if (!ctx.IsSuccess)
            {
                result.Type = null;
                result.IsSuccess = false;
                result.ErrorReason = ContextError.EmptyValue;
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
            result.Type = resultType;
            switch(result.Type)
            {
                case ContextInfoType.Parameter:
                    result.Parameter = new ParameterDisplayInfo(parameter);
                    break;
                case ContextInfoType.Command:
                    if (command != null)
                    {
                        if (commands.Count() > 1 || ctx.HasPriority)
                        {
                            result.Type = ContextInfoType.Overload;
                            result.Overload = new OverloadDisplayInfo(command);
                        }
                        result.Command = new CommandDisplayInfo(command);
                    }
                    else 
                        result.Command = new CommandDisplayInfo(commands.ToList());
                    break;
                case ContextInfoType.Group:
                    result.Group = new ModuleDisplayInfo(group);
                    break;
                case ContextInfoType.Module:
                    result.Module = new ModuleDisplayInfo(module);
                    break;
            }

            result.IsSuccess = true;
            return result;
        }

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
        private IEnumerable<ModuleInfo> GetModules(string name)
            => _commandService.Modules.Where(x => x.Name.ToLower() == name.ToLower());
        private IEnumerable<ModuleInfo> GetGroups(string name)
            => _commandService.Modules.Where(x => x.Group?.ToLower() == name.ToLower());
        private IEnumerable<CommandInfo> GetCommands(string name)
            => _commandService.Commands.Where(x => x.Aliases.Contains(name.ToLower()));
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
            //if (parent.Group != null)
            //    name = $"{parent.Group} {name}";
            IEnumerable<CommandInfo> commands = parent.Commands.Where(x => x.Aliases.Contains(name.ToLower()));
            if (includeChildren)
                parent.Submodules.Select(x => GetCommands(x, name)).ToList().ForEach(x => commands = commands.Concat(x));

            // Console.WriteLine($"-- GetCommands()\n{string.Join('\n', commands.Select(x => string.Join('\n', x.Aliases)))} --");
            return commands;
        }
        private IEnumerable<ParameterInfo> GetArgs(CommandInfo parent, string parameter)
            => parent.Parameters.Where(x => x.Name.ToLower() == parameter.ToLower());
        
        // compile all info from the command service into a single list.
        public List<ModuleDisplayInfo> GetModules()
        {
            List<ModuleDisplayInfo> modules = new List<ModuleDisplayInfo>();
            foreach (ModuleInfo module in _commandService.Modules.Where(x => x.Parent == null))
                modules.Add(new ModuleDisplayInfo(module));
            return modules;
        }

        // make private functions that organize how specific information is displayed on the corresponding types

        // be able to gather information on commands regarding reports, and alter the embed display based on
        // those reports mentioned

        // offer generalized info about other information the user might need
        // such as syntax, reading types, definitions, and examples of other systems on orikivo
        public void Dispose()
        {
            Console.WriteLine("[Debug] -- Disposed a helper thread. --");
        }

        // 🔹 stable
        // 🔸 unstable
        // 🔺 critical
    }
}
