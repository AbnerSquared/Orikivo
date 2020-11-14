using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Orikivo.Text;

namespace Orikivo
{
    /// <summary>
    /// A help service which provides information on all commands as an expansion to <see cref="CommandService"/>.
    /// </summary>
    public class InfoService
    {
        public static readonly string DefaultExample = "value";
        public static readonly string MentionExample = "@Orikivo Arcade#8156";
        public static readonly string TrailingExample = "this is all one value";

        private readonly CommandService _commands;
        private readonly InfoFormatter _formatter;

        private static bool FilterModule(ModuleInfo m, string n)
            => m.Name.Equals(n, StringComparison.OrdinalIgnoreCase);

        private static bool FilterGroup(ModuleInfo g, string n)
            => g.Group?.Equals(n, StringComparison.OrdinalIgnoreCase) ?? g.Aliases?.Contains(n, StringComparison.OrdinalIgnoreCase) ?? false;

        private static bool FilterCommand(CommandInfo c, string n)
            => c.Aliases.Contains(n.ToLower());

        private static bool FilterParameter(ParameterInfo p, string n)
            => p.Name.Equals(n, StringComparison.OrdinalIgnoreCase);

        public InfoService(CommandService commands, InfoFormatter formatter)
        {
            _commands = commands;
            _formatter = formatter;
        }

        // TODO: Move example building to the formatter
        private static void SetExample(OverloadNode command, string prefix)
        {
            var result = new StringBuilder();

            result.Append(prefix);
            result.Append(command.Name);

            if (command.Parameters.Count > 0)
            {
                result.Append(' ');
                result.AppendJoin(" ", command.Parameters.Select(GetExampleArg));
            }

            command.Example = result.ToString();
        }

        private static string GetExampleArg(ParameterNode parameter)
        {
            if (parameter.Tag.HasFlag(ParameterTag.Mentionable))
                return MentionExample;

            if (parameter.Tag.HasFlag(ParameterTag.Trailing))
                return TrailingExample;

            return DefaultExample;
        }

        public string GetPanel(string input = null, BaseUser user = null, string prefix = "[")
        {
            if (string.IsNullOrWhiteSpace(input))
                return _formatter.ViewMenu(this, user);

            ContextNode ctx = Search(input, out string error);

            if (!string.IsNullOrWhiteSpace(error))
                return error;

            bool allowTooltips = user?.Config?.Tooltips ?? true;
            var panel = new StringBuilder();

            // TODO: Move tooltip implementation into the formatter
            if (allowTooltips)
            {
                var tooltips = new List<string>(ctx.Tooltips);

                if (ctx is CommandNode c && c.Overloads.Count > 1)
                {
                    tooltips.Add($"Use `help {ctx.Name}+<index>` to learn more about a specific command overload.");
                }
                else if (ctx.Type == InfoType.Group)
                {
                    tooltips.Add($"Use `help {ctx.Name} <command>` to learn more about a specific command method within a group.");
                }
                else if (ctx.Type == InfoType.Module)
                {
                    tooltips.Add($"Use `help <command>` to learn more about a specific command.");
                }

                if (ctx is OverloadNode o && o.Parameters.Count > 0)
                {
                    tooltips.Add($"Use `help {ctx.Name}{(o.Count > 1 ? $"+{o.Index}" : "")}(<parameter>` to learn more about a specific parameter.");
                }

                panel.AppendLine($"{Format.Tooltip(tooltips)}\n");
            }

            if (ctx is OverloadNode overload)
            {
                SetExample(overload, prefix);
                ctx = overload;
            }

            panel.Append(_formatter.ViewContext(ctx, user));
            return panel.ToString();
        }

        // TODO: Merge the Search method into a single unique instance
        // TODO: Move all of the error text into the locale file
        public ContextNode Search(string input)
        {
            InfoContext ctx = InfoContext.Parse(input);

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
                    throw new ValueNotFoundException($"No commands could be found that matched the name '{ctx.Root}'.");

                // If a priority was specified, slim the search down to that specific command
                if (ctx.HasPriority)
                {
                    commands = commands.Where(x => x.Priority == ctx.Priority);

                    if (!Check.NotNullOrEmpty(commands))
                        throw new ValueNotFoundException($"The priority for the specified command does not exist.");

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
                        throw new ValueNotFoundException("The parameter specified could not be found within any of the commands");

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
                _ => throw new ValueNotFoundException("A ContextNode value could not be created with the given context.")
            };
        }

        public ContextNode Search(string content, out string error)
        {
            InfoContext ctx = InfoContext.Parse(content);

            var type = InfoType.Unknown;

            if (!ctx.IsSuccess)
            {
                error = $"> **Oh.**\n> The specified input could not be parsed.\n```{ctx.ErrorReason}```";
                return null;
            }

            // Get the initial location of the parent to peek
            ModuleInfo module = GetInnerModule(ctx, out error);

            if (!string.IsNullOrWhiteSpace(error))
                return null;

            ModuleInfo group = GetInnerGroup(ctx, out error, module);

            if (!string.IsNullOrWhiteSpace(error))
                return null;

            IEnumerable<CommandInfo> commands = null;
            ParameterInfo parameter = null;

            // If a type wasn't initialized, find the closest matching value
            InfoMatch bestMatch = ctx.Type.HasValue ? null : GetValue(ctx.Root, out error, group ?? module);

            if (!string.IsNullOrWhiteSpace(error))
                return null;

            type = ctx.Type.GetValueOrDefault(bestMatch?.Type ?? type);

            switch (type)
            {
                case InfoType.Module:
                    module = GetModule(ctx.Root, out error, group ?? module);

                    if (!string.IsNullOrWhiteSpace(error))
                        return null;

                    break;

                case InfoType.Group:
                    group = GetGroup(ctx.Root, out error, group ?? module);

                    if (!string.IsNullOrWhiteSpace(error))
                        return null;

                    break;

                case InfoType.Command:
                    commands = GetCommands(ctx.Root, group ?? module);
                    break;
            }

            if (type == InfoType.Command)
            {
                if (!Check.NotNullOrEmpty(commands))
                {
                    error = $"> **Oops!**\n> I could not find the command for the specified input.";
                    return null;
                }

                // If a priority was specified, slim the search down to that specific command
                if (ctx.HasPriority)
                {
                    commands = commands.Where(x => x.Priority == ctx.Priority);

                    if (!Check.NotNullOrEmpty(commands))
                    {
                        error = $"> **Oops!**\n> I could not find a matching command for the specified priority.";
                        return null;
                    }

                    if (commands.Count() > 1)
                        throw new MultiMatchException("The priority for the specified command exists in more than one instance.");

                    type = InfoType.Overload;
                }
                else if (commands.Count() == 1)
                {
                    type = InfoType.Overload;
                }

                // If a parameter was specified, attempt to find the best match possible
                if (Check.NotNull(ctx.Parameter))
                {
                    IEnumerable<ParameterInfo> parameters = new List<ParameterInfo>();
                    commands.ForEach(x => parameters = parameters.Concat(GetParameters(x, ctx.Parameter)));

                    if (!Check.NotNullOrEmpty(parameters))
                    {
                        error = $"> **Oops!**\n> I could not find a matching parameter for {(ctx.HasPriority ? "the specified command" : "any of the specified commands")}.";
                        return null;
                    }

                    if (parameters.Count() > 1)
                    {
                        // NOTE: Distinct() is used to filter out repetitive input searches
                        error = $"> **Huh?**\n> There were several matching parameters for the specified input:\n{string.Join("\n", parameters.Select(x => $"• `{ContextNode.GetId(x)}`").Distinct().OrderBy(x => x[3..]))}";
                        return null;
                    }

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
                _ => throw new ValueNotFoundException("A ContextNode value could not be created with the given context.")
            };
        }

        public List<string> GetAliases(string content)
        {
            try
            {
                ContextNode ctx = Search(content);

                return ctx.Aliases;
            }
            catch (ValueNotFoundException)
            {
                return new List<string>();
            }
        }

        public IEnumerable<InfoMatch> GetMatchingValues(string name)
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

        public IEnumerable<InfoMatch> GetMatchingValues(ModuleInfo parent, string name)
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
            IEnumerable<InfoMatch> values = GetMatchingValues(parent, name);

            if (!Check.NotNullOrEmpty(values))
                throw new ValueNotFoundException($"No matches were found when searching for a value of the name '{name}'.");

            if (values.Any(x => x.Type.EqualsAny(InfoType.Module, InfoType.Group)))
                return values.First(x => x.Type.EqualsAny(InfoType.Module, InfoType.Group));

            return values.First();
        }

        public InfoMatch GetValue(string name, out string error, ModuleInfo parent = null)
        {
            error = null;
            IEnumerable<InfoMatch> values = GetMatchingValues(parent, name);

            if (!Check.NotNullOrEmpty(values))
            {
                error = $"> **Oops!**\n> I could not find a matching value for the specified input.";
                return null;
            }

            if (values.Any(x => x.Type.EqualsAny(InfoType.Module, InfoType.Group)))
            {
                if (values.Count(x => x.Type.EqualsAny(InfoType.Module, InfoType.Group)) > 1)
                {
                    error = $"> **Huh?**\n> There were multiple results for the specified input:\n{string.Join("\n", values.Select(x => $"• `{x.Id}`").Distinct().OrderBy(x => x[3..]))}";
                    return null;
                }

                return values.First(x => x.Type.EqualsAny(InfoType.Module, InfoType.Group));
            }

            return values.First();
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

        public ModuleInfo GetInnerModule(InfoContext ctx, out string error)
        {
            error = null;
            ModuleInfo module = null;

            if (!Check.NotNullOrEmpty(ctx.Modules))
            {
                foreach (string name in ctx.Modules)
                {
                    module = GetModule(name, out error, module);

                    if (!string.IsNullOrWhiteSpace(error))
                        return null;
                }
            }

            return module;
        }

        public ModuleInfo GetModule(string name, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> modules = GetModules(parent, name);

            if (!Check.NotNullOrEmpty(modules))
                throw new ValueNotFoundException($"No matches were found when searching for matching modules of the name '{name}'.");

            if (modules.Count() > 1) // Add support for ambiguity.
                throw new MultiMatchException($"Multiple results were given when searching for a module of the name '{name}'.");

            return modules.First();
        }

        public ModuleInfo GetModule(string name, out string error, ModuleInfo parent = null)
        {
            error = null;
            IEnumerable<ModuleInfo> modules = GetModules(parent, name);

            if (!Check.NotNullOrEmpty(modules))
            {
                error = $"> **Oops!**\n> I could not find a module for the specified input.";
                return null;
            }

            if (modules.Count() > 1)
            {
                error = $"> **Huh?**\n> There were several matching modules for the specified input:\n{string.Join("\n", modules.Select(x => $"• `{ContextNode.GetId(x)}`").Distinct().OrderBy(x => x[3..]))}";
                return null;
            }

            return modules.First();
        }

        public IEnumerable<ModuleInfo> GetBaseModules()
            => Modules.Where(x => x.Parent == null);

        public IEnumerable<ModuleInfo> GetBaseModules(string name)
            => GetBaseModules().Where(m => FilterModule(m, name));

        // remove all visuals on modules with the IgnoreAttribute.
        public IEnumerable<ModuleInfo> Modules => _commands.Modules.Where(x => !x.Attributes.Any(x => x is IgnoreAttribute));

        internal IEnumerable<ModuleInfo> InternalModules => _commands.Modules;

        public IEnumerable<ModuleInfo> GetModules(string name)
            => Modules.Where(m => FilterModule(m, name));

        public IEnumerable<ModuleInfo> GetModules(ModuleInfo parent, string name, bool includeChildren = false)
        {
            if (parent == null)
                return GetModules(name);

            IEnumerable<ModuleInfo> modules = parent.Submodules.Where(m => FilterModule(m, name));

            if (includeChildren)
                parent.Submodules
                    .Select(m => GetModules(m, name, m.Submodules.Count > 0))
                    .ForEach(x => modules = modules.Concat(x));

            return modules;
        }

        // GROUPS
        public ModuleInfo GetInnerGroup(InfoContext ctx, ModuleInfo parent = null)
        {
            ModuleInfo group = null;

            if (!Check.NotNullOrEmpty(ctx.Groups))
                foreach (string name in ctx.Groups)
                    group = GetGroup(name, group ?? parent);

            return group;
        }

        public ModuleInfo GetInnerGroup(InfoContext ctx, out string error, ModuleInfo parent = null)
        {
            error = null;
            ModuleInfo group = null;

            if (!Check.NotNullOrEmpty(ctx.Groups))
                foreach (string name in ctx.Groups)
                {
                    group = GetGroup(name, out error, group ?? parent);

                    if (!string.IsNullOrWhiteSpace(error))
                        return null;
                }

            return group;
        }

        public ModuleInfo GetGroup(string name, ModuleInfo parent = null)
        {
            IEnumerable<ModuleInfo> groups = GetGroups(parent, name);

            if (!Check.NotNullOrEmpty(groups))
                throw new ValueNotFoundException($"No matches were found when searching for matching groups of the name '{name}'.");

            if (groups.Count() > 1) // Add support for ambiguity.
                throw new MultiMatchException($"Multiple results were given when searching for a group of the name '{name}'.");

            return groups.First();
        }

        public ModuleInfo GetGroup(string name, out string error, ModuleInfo parent = null)
        {
            error = null;
            IEnumerable<ModuleInfo> groups = GetGroups(parent, name);

            if (!Check.NotNullOrEmpty(groups))
            {
                error = $"> **Oops!**\n> I could not find a module for the specified input.";
                return null;
            }

            if (groups.Count() > 1)
            {
                error = $"> **Huh?**\n> There were several matching groups for the specified input:\n{string.Join("\n", groups.Select(x => $"• `{ContextNode.GetId(x)}`").Distinct().OrderBy(x => x[3..]))}";
                return null;
            }

            return groups.First();
        }

        public IEnumerable<ModuleInfo> GetGroups(string name)
            => Modules.Where(g => FilterGroup(g, name));

        public IEnumerable<ModuleInfo> GetGroups(ModuleInfo parent, string name, bool includeChildren = false)
        {
            if (parent == null)
                return GetGroups(name);

            IEnumerable<ModuleInfo> groups = parent.Submodules.Where(g => FilterGroup(g, name));

            if (includeChildren)
            {
                parent.Submodules
                    .Select(x => GetGroups(x, name, x.Submodules.Count > 0))
                    .ForEach(x => groups = groups.Concat(x));
            }

            return groups;
        }

        // COMMANDS
        public IEnumerable<CommandInfo> Commands => _commands.Commands;

        public CommandInfo GetCommand(string name)
        {
            IEnumerable<CommandInfo> commands = GetCommands(name);

            if (!Check.NotNullOrEmpty(commands))
                throw new ValueNotFoundException($"No matches were found when searching for matching command of the name '{name}'.");

            if (commands.Count() > 1)
                throw new MultiMatchException($"Multiple results were given when searching for a command of the name '{name}'.");

            return commands.First();
        }

        public IEnumerable<CommandInfo> GetCommands(string name)
            => Commands.Where(c => FilterCommand(c, name));

        public IEnumerable<CommandInfo> GetCommands(string name, ModuleInfo parent, bool includeChildren = false)
        {
            if (parent == null)
                return GetCommands(name);

            IEnumerable<CommandInfo> commands = parent.Commands.Where(x => x.Aliases.Contains(name.ToLower()));

            if (!Check.NotNull(parent.Group))
                if (includeChildren)
                    parent.Submodules
                        .Select(x => GetCommands(name, x))
                        .ForEach(x => commands = commands.Concat(x));

            return commands;
        }

        public IEnumerable<ParameterInfo> GetParameters(string name)
        {
            IEnumerable<CommandInfo> commands = Commands.Where(x => x.Parameters.Any(p => FilterParameter(p, name)));
            IEnumerable<ParameterInfo> parameters = new List<ParameterInfo>();

            commands.ForEach(x => parameters = parameters.Concat(x.Parameters));
            return parameters;
        }

        public IEnumerable<ParameterInfo> GetParameters(CommandInfo command, string name)
            => command.Parameters.Where(p => FilterParameter(p, name));
    }
}
