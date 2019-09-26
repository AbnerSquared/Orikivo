using Discord;
using Discord.Commands;
using Orikivo.Modules;
using Orikivo.Static;
using Orikivo.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Helpers
{
    public class CommandHelper
    {
        private OrikivoCommandContext Context { get; }
        private CommandService Service { get; }

        public CommandHelper(OrikivoCommandContext context, CommandService service)
        {
            Context = context;
            Service = service;
        }

        public IEnumerable<ModuleInfo> GetActiveModules()
        {
            return Context.Data.Modules.Where(x => x.Name != "Hidden" && x.IsActive(Service.Modules)).OrderBy(x => x.GetName());
        }

        public async Task SearchFunctionAsync(string context)
        {
            context = context.ToLower();
            IEnumerable<ModuleInfo> modules = GetActiveModules();
            IEnumerable<CommandInfo> commands = Service.Commands.Where(x => x.Module?.Name != "Hidden");

            string ctxCommand = "";
            int ctxBatchIndex = -1;
            string ctxArg = "";
            // beforehand, we need to check for special symbolizing...
            // check for batch index prefound.
            string[] splitIndexCheck = context.Split('+');
            if (splitIndexCheck.Length == 2)
            {
                ctxCommand = splitIndexCheck[0];
                int.TryParse(splitIndexCheck[1].Substring(0, splitIndexCheck[1].Contains('(') ? splitIndexCheck[1].IndexOf('(') : splitIndexCheck[1].Length), out ctxBatchIndex);
            }
            // check for argument prefounds
            string[] splitArg = context.Split('(');
            if (splitArg.Length == 2)
            {
                if (!splitArg[0].Contains('+'))
                    ctxCommand = splitArg[0];
                ctxArg = splitArg[1];
            }

            IEnumerable<CommandInfo> argCommands = commands.Where(x => x.HasMatchingAliases(ctxCommand));
            //.Where(x => x.Parameters.Any(y => y.HasMatchingName(ctxArg)))
            if (argCommands.Count() > 0)
            {
                if (ctxBatchIndex > -1)
                    argCommands = argCommands.Where(x => x.Priority == ctxBatchIndex);
                if (argCommands.Count() > 1)
                {
                    argCommands = argCommands.Where(x => x.Parameters.Funct()).Where(x => x.Parameters.Any(y => y.HasMatchingName(ctxArg)));
                    if (argCommands.Count() > 1)
                    {
                        await ThrowAmbiguity(argCommands);
                    }
                    else if (argCommands.Count() > 0)
                    {
                        ParameterInfo arg = argCommands.First().Parameters.Where(x => x.HasMatchingName(ctxArg)).First();
                        await Context.Channel.SendEmbedAsync(ShowArgAsync(arg));
                    }
                }
                else if (argCommands.Count() > 0)
                {
                    CommandInfo argCommand = argCommands.First();
                    if (!string.IsNullOrWhiteSpace(ctxArg))
                    {
                        if (argCommand.Parameters.Funct())
                        {
                            ParameterInfo arg = argCommand.Parameters.Where(x => x.HasMatchingName(ctxArg)).First();
                            await Context.Channel.SendEmbedAsync(ShowArgAsync(arg));
                        }
                        else
                        {
                            await Context.Channel.ThrowAsync("`NO_ARG`", "This mentioned command does not contain any arguments to search for.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendEmbedAsync(ShowCommandAsync(argCommand));
                    }
                }
            }

            // if the prefounds all fail, resort to default searching.

            // searching within the list of modules that exist.
            IEnumerable<ModuleInfo> moduleResults = modules.Where(x => x.HasMatchingAliases(context));

            if (moduleResults.Count() > 1) // in case of identical modules.
            {
                await ThrowAmbiguity(moduleResults);
            }
            else if (moduleResults.Count() > 0) // if the results is 1 exact.
            {
                ModuleInfo module = moduleResults.First();
                if (module.Group.Exists())
                    await Context.Channel.SendEmbedAsync(ShowGroupAsync(module));
                else
                    await Context.Channel.SendEmbedAsync(ShowModuleAsync(module));
            }
            else
            { // searching within the list of commands that exist.
                IEnumerable<CommandInfo> commandResults = commands.Where(x => x.HasMatchingAliases(context));

                if (commandResults.Count() > 1)
                {
                    if (commandResults.All(x => x.Module == commandResults.First().Module))
                    {
                        await Context.Channel.SendEmbedAsync(ShowCommandsAsync(commandResults));
                    }
                    else
                    {
                        await ThrowAmbiguity(commandResults);
                    }
                }
                else if (commandResults.Count() > 0)
                {
                    await Context.Channel.SendEmbedAsync(ShowCommandAsync(commandResults.First()));
                }
            }
        }

        public async Task ThrowAmbiguity(IEnumerable<CommandInfo> commands)
        {
            await Context.Channel.ThrowAsync("`AMBIGUITY`", $"Here are all of the commands with a matching name:\n{commands.Enumerate(x => x.Module.GetName() + "." + x.GetName()).Conjoin("\n")}");
        }

        public async Task ThrowAmbiguity(IEnumerable<ModuleInfo> modules)
        {
            await Context.Channel.ThrowAsync("`AMBIGUITY`", $"Here are all of the modules with a matching name:\n{modules.Enumerate(x => x.Parent?.GetName() + x.GetName()).Conjoin("\n")}");
        }

        public Embed ShowCommandAsync(CommandInfo command)
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            eb.WithColor(85, 172, 238); // sync with command status.
            StringBuilder sb = new StringBuilder();
            string priority = Service.Commands.Where(x => x.GetName() == command.GetName()).Count() > 1 ? Format.Code($"+{command.Priority}") : "";
            sb.AppendLine(WriteCommandReport(command) + GetCommandSyntax(command) + priority);
            List<string> aliases = command.Aliases.Funct() ? GetAliases(command) : null;
            if (aliases.Funct())
                sb.AppendLine($"({"AKA".ToSuperscript()}) {aliases.Conjoin(", ")}");
            if (command.Summary.Exists())
                sb.AppendLine(command.Summary);

            eb.WithFooter(command.Module.GetName());
            eb.WithDescription(sb.ToString());

            return eb.Build();
        }

        public EmbedFieldBuilder ShowCommandGroupAsync(CommandInfo command)
        {
            EmbedFieldBuilder fb = new EmbedFieldBuilder();
            fb.WithName(WriteCommandReport(command) + GetCommandSyntax(command));
            StringBuilder sb = new StringBuilder();
            List<string> aliases = command.Aliases.Funct() ? GetAliases(command) : null;
            if (aliases.Funct())
                sb.AppendLine($"({"AKA".ToSuperscript()}) {aliases.Conjoin(", ")}");
            if (command.Summary.Exists())
                sb.AppendLine(command.Summary);
            fb.WithValue(string.IsNullOrWhiteSpace(sb.ToString()) ? "UNSPECIFIED" : sb.ToString());
            return fb;

        }

        public EmbedFieldBuilder ShowCommandFieldAsync(CommandInfo command)
        {
            EmbedFieldBuilder fb = new EmbedFieldBuilder();
            fb.WithName(WriteCommandReport(command) + GetCommandSyntax(command) + $"+{command.Priority}".DiscordLine());
            StringBuilder sb = new StringBuilder();
            List<string> aliases = command.Aliases.Funct() ? GetAliases(command) : null;
            if (aliases.Funct())
                sb.AppendLine($"({"AKA".ToSuperscript()}) {aliases.Conjoin(", ")}");
            if (command.Summary.Exists())
                sb.AppendLine(command.Summary);
            fb.WithValue(string.IsNullOrWhiteSpace(sb.ToString()) ? "UNSPECIFIED" : sb.ToString());
            return fb;
        }

        public string GetCommandSyntax(CommandInfo command)
        {
            CommandSyntax syntax = new CommandSyntax(command);
            string space = string.IsNullOrWhiteSpace(command.GetName()) ? "" : " ";
            string group = command.Module.Group.Exists() ? $"{command.Module.Group}{space}" : "";
            return $"{group}{Format.Bold(command.GetName())}({syntax.GetArgs()})";
        }

        public Embed ShowCommandsAsync(IEnumerable<CommandInfo> commands)
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            foreach (CommandInfo command in commands)
                eb.AddField(ShowCommandFieldAsync(command));
            eb.Fields = eb.Fields.OrderBy(x => x.Name.Length).ToList();
            return eb.Build();
        }

        public Embed ShowMainAsync(int page = 1)
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            eb.WithDescription("orikivo : modules".DiscordBlock());
            // if the account wants to show tooltips...
            eb.WithFooter($"{Context.Server.Config.GetPrefix(Context)}help <module>");

            IEnumerable<ModuleInfo> modules = GetActiveModules().Parents();
            List<string> summaries = WriteModules(modules);
            return Embedder.Paginate(summaries, page, eb);
        }

        public Embed ShowModuleAsync(ModuleInfo module)
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            eb.WithTitle($"{WriteModuleReport(module)}{Format.Bold(module.GetName())}");
            eb.WithColor(85, 172, 238); // sync with report status

            int counter = GetCommandCount(module);
            string counterString = counter > 0 ? $"{counter} command{(counter > 1 ? "s" : "")}" : "";

            eb.WithFooter(counterString);
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> commands = WriteAllCommands(module).Merge(WriteAllGroups(module)).OrderBy(x => x);
            IEnumerable<string> submodules = WriteAllSubmodules(module).OrderBy(x => x);

            if (module.Summary.Exists())
                sb.AppendLine($"{module.Summary}\n");
            if (commands.Funct())
            {
                sb.AppendLine(Format.Bold("Commands"));
                sb.AppendLine(commands.Conjoin(" "));
            }
            if (submodules.Funct())
            {
                if (commands.Funct())
                    sb.AppendLine();
                sb.AppendLine("Submodules".MarkdownBold());
                sb.AppendLine(submodules.Conjoin("\n"));
            }

            eb.WithDescription(sb.ToString());
            return eb.Build();
        }

        public List<string> WriteAllCommands(ModuleInfo module)
        {
            List<string> results = new List<string>();
            Dictionary<string, int> commands = new Dictionary<string, int>();
            foreach (CommandInfo command in module.Commands)
            {
                string name = command.GetName();
                if (commands.ContainsKey(name))
                {
                    commands[name] += 1;
                    continue;
                }
                else
                    commands[name] = 0;
            }

            foreach (KeyValuePair<string, int> pair in commands)
            {
                if (pair.Value > 0)
                    results.Add($"{pair.Key}{$"+{pair.Value}".ToSuperscript()}".DiscordLine());
                else
                    results.Add(pair.Key.DiscordLine());
            }

            return results;
        }

        public List<string> WriteAllGroups(ModuleInfo module)
        {
            List<string> results = new List<string>();
            foreach (ModuleInfo submodule in module.Submodules.Where(x => x.Group.Exists()))
                results.Add($"{submodule.Group.DiscordLine()}{Format.Bold("\\*")}");

            return results;
        }

        public List<string> WriteModules(IEnumerable<ModuleInfo> modules)
        {
            List<string> results = new List<string>();
            foreach (ModuleInfo module in modules)
                results.Add(WriteModule(module));
            return results;
        }

        public List<string> WriteAllSubmodules(ModuleInfo module)
        {
            List<string> results = new List<string>();
            foreach (ModuleInfo submodule in module.Submodules.Where(x => !x.Group.Exists()))
                results.Add(WriteModule(submodule));

            return results;
        }

        public string WriteModule(ModuleInfo module)
        {
            return $"{GetModuleReportIcon(module)}{Format.Bold(module.GetName())} - {module.GetSummary()}";
        }

        public string GetModuleReportIcon(ModuleInfo module)
        {
            return $"{EmojiIndex.Unread.Pack(Context.Account)}";
        }

        public string GetCommandReportIcon(CommandInfo command)
        {
            return $"{EmojiIndex.Unread.Pack(Context.Account)}";
        }

        public string WriteModuleReport(ModuleInfo module)
        {
            // reading from the list of reports,
            // find out the amount of issues that relate...
            return $"`stable` • ";
        }

        public string WriteCommandReport(CommandInfo command)
        {
            // reading from the list of reports,
            // find out the amount of issues that relate...
            return $"`stable` • ";
        }

        public List<string> GetAliases(ModuleInfo module)
        {
            List<string> results = new List<string>();
            foreach (string alias in module.Aliases)
            {
                if (alias.Matches(module.Name))
                    continue;

                string context = alias;

                if (module.Parent.Exists())
                    if (module.Parent.Group.Exists())
                        context = alias.ReplaceMany("", module.Parent.Aliases.ToArray());

                if (string.IsNullOrWhiteSpace(context) || results.Contains(context) || context.Matches(module.Name))
                    continue;

                results.Add(context);
            }

            return results.OrderByDescending(x => x.Length).ToList();
        }

        public List<string> GetAliases(CommandInfo command)
        {
            List<string> results = new List<string>();
            foreach (string alias in command.Aliases)
            {
                if (alias.Matches(command.Name))
                    continue;

                string context = alias;

                if (command.Module.Group.Exists())
                {
                    if (command.Module.Aliases.Any(x => alias == $"{x} {command.Name}"))
                        continue;
                    context = alias.ReplaceMany("", command.Module.Aliases.ToArray());
                }

                // results.Any(x => x.Contains(context))
                if (string.IsNullOrWhiteSpace(context) || results.Contains(context) || context.Matches(command.Name))
                    continue;

                results.Add(context);
            }

            return results.OrderByDescending(x => x.Length).ToList();
        }

        public Embed ShowGroupAsync(ModuleInfo module)
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            StringBuilder sb = new StringBuilder();
            string title = Format.Bold(module.GetName());
            List<string> aliases = module.Aliases.Funct() ? GetAliases(module) : null;
            if (aliases.Funct())
                title += ($"({"AKA".ToSuperscript()}) {aliases.Enumerate(x => Format.Code(x)).Conjoin(", ")} *");
            eb.WithTitle(title);

            if (module.Summary.Exists())
            {
                sb.AppendLine($"{module.Summary}\n");
            }
            foreach (CommandInfo command in module.Commands)
                eb.AddField(ShowCommandGroupAsync(command));
            if (module.Submodules.Funct())
            {
                sb.AppendLine(Format.Bold("Subgroups"));
                foreach (ModuleInfo submodule in module.Submodules)
                {
                    sb.AppendLine(WriteModule(submodule));
                    List<string> subaliases = GetAliases(submodule);
                    if (subaliases.Funct())
                        sb.AppendLine($"({"AKA".ToSuperscript()}) {subaliases.Enumerate(x => Format.Code(x)).Conjoin(", ")}");
                }
            }

            eb.WithDescription(sb.ToString());
            eb.Fields = eb.Fields.OrderBy(x => x.Name.Length).ToList();
            return eb.Build();
        }

        public Embed ShowArgAsync(ParameterInfo arg)
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            StringBuilder sb = new StringBuilder();

            string groupName = arg.Command.Module.Group.Exists() ? $"{arg.Command.Module.Group} " : "";
            string priority = Service.Commands.Where(x => x.GetName() == arg.Command.GetName()).Count() > 1 ? Format.Code($"+{arg.Command.Priority}") : "";
            sb.AppendLine($"{Format.Bold(arg.Name)}[{groupName}{arg.Command.Name}(){priority}]");
            sb.AppendLine($"[**Type**] {arg.Type.Name}");
            // list modifiers : sb.AppendLine(WriteModifiers(arg));
            if (arg.Summary.Exists())
                sb.AppendLine(arg.Summary);
            eb.WithDescription(sb.ToString());
            return eb.Build();
        }

        public string WriteModifiers(ParameterInfo arg)
        {
            return "`MODIFIER`";
        }

        public int GetCommandCount(ModuleInfo module)
        {
            int counter = module.Commands.Count;
            foreach (ModuleInfo submodule in module.Submodules)
                counter += submodule.Commands.Count;
            return counter;
        }
    }

}
