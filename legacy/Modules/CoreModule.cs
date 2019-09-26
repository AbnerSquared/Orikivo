using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using Orikivo.Static;
using System.Threading.Tasks;
using Orikivo.Systems.Presets;
using Microsoft.Extensions.Configuration;
using Discord.Rest;
using System.Diagnostics;
using Orikivo.Networking;
using System.IO;
using Orikivo.Utility;
using Orikivo.Helpers;

namespace Orikivo.Modules
{
    [Name("Core")]
    [Summary("The root directory of Orikivo. Contains various barebones functions.")]
    public class CoreModule : ModuleBase<OrikivoCommandContext>
    {
        private CommandService _service;
        private IServiceProvider _provider;

        public CoreModule(CommandService service, IServiceProvider provider)
        {
            _service = service;
            _provider = provider;
        }

        public async Task GetLatencyAsync()
        {
            var before = new EmbedBuilder();
            before.WithDescription("Pinging...".MarkdownBold());
            before.WithColor(EmbedData.GetColor("error"));

            var after = before;
            after.WithColor(EmbedData.GetColor("origreen"));

            await CalculateLatencyAsync(before.Build(), after);
        }

        public async Task CalculateLatencyAsync(Embed before, EmbedBuilder after)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            RestUserMessage message = await Context.Channel.SendMessageAsync(embed: before);
            stopwatch.Stop();

            string ping = $"{EmojiIndex.Ping} {"Pong!".MarkdownBold()}";
            string latency = $"**Local** ({stopwatch.ElapsedMilliseconds} ms) **/** **Gateway** ({Context.Client.Latency} ms)";

            after.WithTitle(ping);
            after.WithDescription(latency);
            await message.ModifyAsync(x => { x.Embed = after.Build(); });
        }

        public async Task GetModulesAsync(int page = 1)
        {
            List<ModuleInfo> current = _service.Modules.Where(x => !x.IsSubmodule).ToList();
            List<ModuleInfo> all = Context.Data.Modules.Where(x => !x.IsSubmodule).ToList();

            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (ModuleInfo m in all)
            {
                m.GetType().Debug();

                if (current.Contains(m))
                {
                    sb.Append(EmojiIndex.Active);
                }
                else
                {
                    sb.Append(EmojiIndex.Inactive);
                }
                sb.Append($" {m.GetName().MarkdownBold()}");
                sb.ToString().Debug();
                list.Add(sb.ToString());
                sb.Clear();
            }

            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("owo"));
            e.WithTitle("Modules");

            List<EmbedBuilder> embeds = GenerateList(list, e);
            page = page.InRange(1, embeds.Count) - 1;

            EmbedFooterBuilder f = GeneratePagedFooter(page, embeds.Count);
            embeds[page].WithFooter(f);
            await ReplyAsync(embed: embeds[page].Build());
        }

        public EmbedFooterBuilder GeneratePagedFooter(int index, int max, string message = null)
        {
            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText($"{message ?? ""}{((message.Exists() & max > 1) ? " | " : "")}{(max > 1 ? $"Page {index + 1} of {max}" : "")}");
            return f;
        }

        public EmbedBuilder GetDefaultEmbed()
        {
            EmbedBuilder e = new EmbedBuilder();
            e.WithColor(EmbedData.GetColor("origreen"));
            return e;
        }

        public List<EmbedBuilder> GenerateList(List<string> list, EmbedBuilder e = null)
        {
            List<EmbedBuilder> embeds = new List<EmbedBuilder>();
            const int MAX_DESCRIPTION = 1024;
            e = e ?? GetDefaultEmbed();

            StringBuilder sb = new StringBuilder();
            foreach (string s in list)
            {
                if (sb.Length + s.Length > MAX_DESCRIPTION)
                {
                    e.WithDescription(sb.ToString());
                    embeds.Add(e);
                    sb.Clear();
                }
                sb.AppendLine(s);
            }
            if (sb.Length > 0)
            {
                e.WithDescription(sb.ToString());
                embeds.Add(e);
                sb.Clear();
            }

            return embeds;
        }

        public async Task UnloadModuleAsync(ModuleInfo module)
        {


            IEnumerable<ModuleInfo> current = _service.Modules;
            EmbedBuilder eb = EmbedData.DefaultEmbed;

            if (!module.IsActive(current))
            {
                eb.WithColor(EmbedData.GetColor("error"));
                eb.WithDescription($"{module.GetName().MarkdownBold()} is already inactive.");
                await ReplyAsync(embed: eb.Build());
                return;
            }

            await _service.RemoveModuleAsync(module);

            
            eb.WithColor(EmbedData.GetColor("owo"));
            eb.WithDescription($"{module.GetName().MarkdownBold()} has been unloaded.");

            await ReplyAsync(embed: eb.Build());
        }

        public async Task LoadModuleAsync(ModuleInfo module)
        {
            module.GetType().Debug("the type of the module.");

            IEnumerable<ModuleInfo> current = _service.Modules;
            List<ModuleInfo> modules = Context.Data.Modules;

            if (module.IsActive(current))
            {
                await ReplyAsync("This module is already active.");
                return;
            }

            //await _service.AddModuleAsync(type, _provider);
            await ReplyAsync($"I have built {module.GetName()}, except I can't reload modules yet. Figure it out, numnut.");
        }

        public async Task GetModuleAsync(ModuleInfo module)
        {
            List<ModuleInfo> submodules = null;
            if (module.Submodules.Count > 0)
            {
                submodules = module.Submodules.OrderByDescending(x => x.GetName()).ToList();
            }

            EmbedBuilder e = GenerateModuleBox(module, submodules);
            await ReplyAsync(embed: e.Build());
        }

        public EmbedBuilder GenerateModuleBox(ModuleInfo main, List<ModuleInfo> sub = null)
        {
            int totalCommands = main.Commands.Count;

            EmbedBuilder e = new EmbedBuilder();
            e.WithTitle(main.GetName());
            e.WithDescription(main.Summary ?? "Empty.");
            e.WithColor(EmbedData.GetColor("owo"));
            if (!main.IsActive(_service.Modules))
            {
                e.WithColor(EmbedData.GetColor("yield"));
            }
            if (sub.Exists())
            {
                List<string> submods = new List<string>();
                StringBuilder sb = new StringBuilder();

                if (sub.Count > 0)
                {
                    foreach (ModuleInfo s in sub)
                    {
                        totalCommands += s.Commands.Count;
                        if (s.IsActive(_service.Modules))
                        {
                            sb.Append(EmojiIndex.Active);
                        }
                        else
                        {
                            sb.Append(EmojiIndex.Inactive);
                        }
                        sb.Append($" {s.GetName().MarkdownBold()}");
                        submods.Add(sb.ToString());
                        sb.Clear();
                    }
                }

                EmbedFieldBuilder field = new EmbedFieldBuilder();
                field.WithName("Submodules");
                field.WithValue(string.Join('\n', submods));
                field.WithIsInline(false);

                e.AddField(field);
            }

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText($"{totalCommands} command{(totalCommands > 1 ? "s": "")} | {(main.IsActive(_service.Modules) ? "Active" : "Inactive")}");

            e.WithFooter(f);

            return e;
        }

        public async Task GetAllCommandHelpersAsync()
        {

        }
        
        public async Task GetMainPanelHelperAsync()
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            string parameters = GetSyntaxParameterBox();
        }

        public string GetSyntaxParameterBox()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString().DiscordBlock("");
        }

        public List<string> GetAllCommands(ModuleInfo m)
        {
            Utility.Debugger.Write("Getting all commands for a module...");
            List<string> l = new List<string>();
            Dictionary<string, int> cmds = new Dictionary<string, int>();
            foreach (CommandInfo c in m.Commands)
            {
                string cmd = c.GetName();
                if (cmds.ContainsKey(cmd))
                {
                    cmd.Debug("duped command, adding 1");
                    cmds[cmd].Debug();
                    cmds[cmd] += 1;
                    continue;
                }
                else
                {
                    cmd.Debug("new command");
                    cmds[cmd] = 0;
                }
                
            }

            //.MarkdownLine()
            foreach (KeyValuePair<string, int> pair in cmds)
            {
                if (pair.Value > 0)
                {
                    l.Add($"{pair.Key}+{pair.Value}".DiscordLine());
                }
                else
                {
                    l.Add(pair.Key.DiscordLine());
                }
            }

            return l;
        }

        public List<string> GetAllGroups(ModuleInfo m)
        {
            List<string> l = new List<string>();
            foreach (ModuleInfo s in m.Submodules.Where(x => x.Group.Exists()))
            {
                l.Add($"{s.Group}*".DiscordLine());
            }
            return l;
        }

        public List<string> GetAllSubmodules(ModuleInfo m)
        {
            List<string> l = new List<string>();
            foreach (ModuleInfo s in m.Submodules.Where(x => !x.Group.Exists()))
            {
                l.Add(GetSubmoduleInfo(s));
            }

            return l;
        }

        public string GetSubmoduleInfo(ModuleInfo s)
        {
            return $"{EmojiIndex.Experience.Pack(Context.Account)}{s.GetName().MarkdownBold()} - {s.GetSummary()}";
        }

        public int GetSubmoduleCommandCount(ModuleInfo m)
        {
            int commands = 0;
            foreach (ModuleInfo s in m.Submodules)
            {
                commands += s.Commands.Count();
            }
            return commands;
        }

        public async Task GetInnerModulePanelAsync(ModuleInfo m)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(m.GetName().MarkdownBold());


            int count = m.Commands.Count + GetSubmoduleCommandCount(m);
            string f = $"{count} command{(count > 1 ? "s" : "")}";
            if (count > 0)
            {
                e.WithFooter(f);
            }
            StringBuilder str = new StringBuilder();
            List<string> cmds = GetAllCommands(m);
            List<string> groups = GetAllGroups(m);
            cmds = cmds.Merge(groups).OrderBy(x => x).ToList();
            List<string> sms = GetAllSubmodules(m).OrderBy(x => x).ToList();

            if (m.Summary.Exists())
            {
                str.AppendLine(m.Summary);
                str.AppendLine();
            }
            if (cmds.Funct())
            {
                str.AppendLine("Commands".MarkdownBold());
                str.AppendLine(cmds.Conjoin(" "));
            }
            if (sms.Funct())
            {
                if (m.Commands.Funct())
                {
                    str.AppendLine();
                }
                str.AppendLine("Submodules".MarkdownBold());
                str.Append(sms.Conjoin("\n"));
            }

            e.WithDescription(str.ToString());
            await ReplyAsync(embed: e.Build());
        }

        public async Task GetModulePanelAsync(int page = 1)
        {
            EmbedBuilder e = GetDefaultEmbed();
            e.WithDescription("orikivo : modules".DiscordBlock());
            e.WithFooter($"{Context.Server.Config.GetPrefix(Context)}help <module>");

            IEnumerable<ModuleInfo> modules = GetActiveModules().Parents();
            List<string> summaries = BuildModuleSummaries(modules);
            Embed r = EmbedData.GenerateEmbedList(summaries, page, e);
            await ReplyAsync(embed: r);
        }

        public List<string> GetAliasList(ModuleInfo m)
        {
            List<string> l = new List<string>();
            foreach (string a in m.Aliases)
            {
                if (a.ToLower() == m.Name.ToLower())
                {
                    continue;
                }

                string b = a;

                if (m.Parent.Exists())
                {
                    if (m.Parent.Group.Exists())
                    {
                        b = a.ReplaceMany("", m.Parent.Aliases.ToArray());
                    }
                }
                if (string.IsNullOrWhiteSpace(b))
                {
                    continue;
                }
                if (l.Contains(b))
                {
                    continue;
                }
                if (b.ToLower() == m.Name.ToLower())
                {
                    continue;
                }

                l.Add(b);
            }
            return l.OrderByDescending(x => x.Length).ToList();
        }

        public List<string> GetAliasList(CommandInfo cmd)
        {
            List<string> l = new List<string>();
            foreach (string a in cmd.Aliases)
            {
                cmd.Name.Debug();

                if (a == cmd.Name)
                {
                    continue;
                }

                string b = a;
                b.Debug();
                
                if (cmd.Module.Group.Exists())
                {
                    cmd.Module.Aliases.ForEach(x => x.Debug());
                    if (cmd.Module.Aliases.Any(x => a == $"{x} {cmd.Name}"))
                        continue;
                    //b = a.Replace(cmd.Module.Group, "").Trim();
                    b = a.ReplaceMany("", cmd.Module.Aliases.ToArray());
                }
                if (string.IsNullOrWhiteSpace(b))
                {
                    continue;
                }
                if (l.Contains(b) || l.Any(x => x.Contains(b)))
                {
                    continue;
                }
                if (b == cmd.Name)
                {
                    continue;
                }

                l.Add(b);
            }
            return l.OrderByDescending(x => x.Length).ToList();
        }

        public EmbedBuilder BuildCommandSummary(CommandInfo cmd)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            //e.WithTitle(GetCommandNameArgs(cmd));
            StringBuilder str = new StringBuilder();
            str.AppendLine(GetCommandNameArgs(cmd));
            if (cmd.Aliases.Funct())
            {
                List<string> alias = GetAliasList(cmd);
                if (alias.Funct())
                {
                    str.AppendLine($"-- " + alias.Conjoin(", "));
                }
            }
            if (cmd.Summary.Exists())
            {
                str.AppendLine(cmd.Summary);
            }
            
                
            StringBuilder footer = new StringBuilder();
            footer.Append($"{cmd.Module.GetName()}");
            if (cmd.Parameters.Funct())
            {
                if (cmd.Parameters.Any(x => x.IsOptional))
                {
                    footer.Append($" | arg? (Optional)");
                }
            }
            e.WithFooter(footer.ToString());

            e.WithDescription(str.ToString());
            
            return e;
        }

        public string GetCommandNameArgs(CommandInfo cmd)
        {
            CommandSyntax syn = new CommandSyntax(cmd);
            return $"{(cmd.Module.Group.Exists() ? cmd.Module.Group + (string.IsNullOrWhiteSpace(cmd.GetName()) ? "" : " ") : "")}{( string.IsNullOrWhiteSpace(cmd.GetName()) ? "" : $"{cmd.GetName().MarkdownBold()}")}({syn.GetArgs()})";
        }

        public EmbedFieldBuilder GetInnerCommandField(CommandInfo cmd)
        {
            EmbedFieldBuilder f = new EmbedFieldBuilder();
            f.WithName(GetCommandNameArgs(cmd));
            
            StringBuilder str = new StringBuilder();
            if (cmd.Aliases.Funct())
            {
                List<string> alias = GetAliasList(cmd);
                if (alias.Funct())
                {
                    str.AppendLine($"-- " + alias.Conjoin(", "));
                }
            }
            if (cmd.Summary.Exists())
            {
                str.AppendLine(cmd.Summary);
            }
            f.WithValue(str.ToString());
            return f;
        }

        public EmbedBuilder GetManyInnerCommandPanelAsync(IEnumerable<CommandInfo> cmds)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            foreach (CommandInfo cmd in cmds)
            {
                e.AddField(GetInnerCommandField(cmd));
            }
            if (cmds.Funct())
            {
                if (cmds.Any(x => x.Parameters.Any(y => y.IsOptional)))
                {
                    e.WithFooter($"arg? (Optional)");
                }
            }
            e.Fields = e.Fields.OrderBy(x => x.Name.Length).ToList();

            return e;
        }

        public EmbedFieldBuilder GetInnerCommandGroupField(CommandInfo c)
        {
            EmbedFieldBuilder f = new EmbedFieldBuilder();
            f.WithName(GetCommandNameArgs(c));

            StringBuilder str = new StringBuilder();
            if (c.Aliases.Funct())
            {
                List<string> alias = GetAliasList(c);
                if (alias.Funct())
                {
                    str.AppendLine($"-- " + alias.Conjoin(", "));
                }
            }
            if (c.Summary.Exists())
            {
                str.AppendLine(c.Summary);
            }
            string r = str.ToString();
            if (string.IsNullOrWhiteSpace(r))
            {
                r = "Unspecified value.";
            }
            f.WithValue(r);
            return f;
        }

        public async Task GetInnerGroupPanelAsync(ModuleInfo g)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            StringBuilder str = new StringBuilder();
            string title = g.GetName().MarkdownBold();
            if (g.Aliases.Funct())
            {
                List<string> alias = GetAliasList(g);
                if (alias.Funct())
                {
                    title += $"\n --- {alias.Enumerate(x => Format.Code(x)).Conjoin(" ")} *";
                }
            }
            e.WithTitle(title);

            if (g.Summary.Exists())
            {
                str.AppendLine(g.Summary);
                str.AppendLine();
            }
            foreach (CommandInfo c in g.Commands)
            {
                e.AddField(GetInnerCommandGroupField(c));
            }
            if (g.Commands.Any(x => x.Parameters.Any(y => y.IsOptional)))
            {
                e.WithFooter($"arg? (Optional)");
            }
            if (g.Submodules.Funct())
            {
                str.AppendLine("Subgroups".MarkdownBold());
                foreach(ModuleInfo sm in g.Submodules)
                {
                    str.AppendLine($"{EmojiIndex.Experience.Pack(Context.Account)}{sm.GetName().MarkdownBold()}");
                    List<string> subalias = GetAliasList(sm);
                    if (subalias.Funct())
                    {
                        str.AppendLine($"-- {subalias.Conjoin(", ")}");
                    }
                }
            }

            e.WithDescription(str.ToString());
            e.Fields = e.Fields.OrderBy(x => x.Name.Length).ToList();
            await ReplyAsync(embed: e.Build());
        }

        public EmbedBuilder GetCommandArgPanelAsync(ParameterInfo p)
        {
            EmbedBuilder eb = Embedder.DefaultEmbed;
            StringBuilder sb = new StringBuilder();
            if (p.Command.Module.Exists())
            {
                if (p.Command.Module.Group.Exists())
                {
                    sb.Append($"{p.Command.Module.Group} ");
                }
            }
            sb.AppendLine($"{p.Command.Name}(**{p.Name}**{(p.IsOptional ? "?": "")}");
            sb.AppendLine(p.Type.Name);
            if (p.Summary.Exists())
                sb.AppendLine(p.Summary);
            eb.WithDescription(sb.ToString());
            return eb;
        }

        [Command("oldhelp"), Alias("oh"), Priority(1)]
        [Summary("Search for a command or module.")]
        public async Task GetHelp21Async([Remainder]string context)
        {
            context = context.ToLower();
            IEnumerable<ModuleInfo> modules = GetActiveModules();
            IEnumerable<CommandInfo> commands = _service.Commands.Where(x => x.Module?.Name != "Hidden");

            string[] splitContext = context.Split('.');
            if (splitContext.Length == 2)
            {
                string ctxModule = splitContext[0];
                string[] ctxCommandArgs = splitContext[1].Split('(');
                string ctxCommand = "";
                string ctxArg = "";
                if (ctxCommandArgs.Length == 2)
                {
                    ctxCommand = ctxCommandArgs[0];
                    ctxArg = ctxCommandArgs[1];
                }
                else
                {
                    ctxCommand = ctxCommandArgs[0];
                }
                
                if (modules.Any(x => x.HasMatchingAliases(ctxModule)))
                {
                    ModuleInfo m = modules.Where(x => x.HasMatchingAliases(ctxModule)).First();
                    if (!string.IsNullOrWhiteSpace(ctxCommand))
                    {
                        if (m.Commands.Funct())
                        {
                            if (m.Commands.Any(x => x.HasMatchingAliases(ctxCommand)))
                            {
                                IEnumerable<CommandInfo> cmds = m.Commands.Where(x => x.HasMatchingAliases(ctxCommand));
                                if (cmds.Count() > 1)
                                {
                                    List<string> cmdsName = new List<string>();
                                    if (!string.IsNullOrWhiteSpace(ctxArg))
                                    {
                                        if (cmds.All(x => x.Name == cmds.First().Name))
                                        {
                                            if (cmds.Any(x => x.Parameters.Funct()))
                                            {
                                                if (cmds.Where(x => x.Parameters.Funct()).Any(x => x.Parameters.Any(y => y.HasMatchingName(ctxArg))))
                                                {
                                                    ParameterInfo p = cmds.Where(x => x.Parameters.Funct()).Where(x => x.Parameters.Any(y => y.HasMatchingName(ctxArg))).OrderByDescending(x => x.Parameters.Count).First().Parameters.Where(x => x.HasMatchingName(ctxArg)).First();
                                                    await ReplyAsync(embed: GetCommandArgPanelAsync(p).Build());
                                                    return;
                                                }

                                                await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`UNKNOWN_ARG`", "The argument you specified for this command batch led to no results."));
                                                return;
                                            }

                                            await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`EMPTY_ARG`", "There are no arguments for any of the specified overloaded command."));
                                            return;

                                        }

                                        await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`MULTIPLE_COMMANDS`", "The alias used to specify the command returned multiple variants."));
                                        return;

                                    }

                                    await ReplyAsync(embed: GetManyInnerCommandPanelAsync(cmds).Build());
                                    return;
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(ctxArg))
                                    {
                                        if (cmds.First().Parameters.Funct())
                                        {
                                            if (cmds.First().Parameters.Any(x => x.HasMatchingName(ctxArg)))
                                            {
                                                ParameterInfo p = cmds.First().Parameters.Where(x => x.HasMatchingName(ctxArg)).First();
                                                await ReplyAsync(embed: GetCommandArgPanelAsync(p).Build());
                                                return;
                                            }
                                            else
                                            {
                                                await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`UNKNOWN_ARG`", "The argument you specified for this command led to no results."));
                                                return;
                                            }
                                        }
                                        await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`EMPTY_ARG`", "There are no arguments for this command."));
                                        return;
                                    }

                                    await ReplyAsync(embed: BuildCommandSummary(cmds.First()).Build());
                                    return;
                                }
                            }
                            await ReplyAsync(embed: EmbedData.Throw(Context, $"{m.Name.ToLower()}.`UNKNOWN_COMMAND`", "The command you specified for this module led to no results."));
                            return;
                        }
                        await ReplyAsync(embed: EmbedData.Throw(Context, $"{m.Name.ToLower()}.`EMPTY_COMMANDS`", "There are no commands for this module."));
                        return;
                    }
                    if (m.Group.Exists())
                    {
                        await GetInnerGroupPanelAsync(m);
                        return;
                    }
                    await GetInnerModulePanelAsync(m);
                    return;
                }
                await ReplyAsync(embed: EmbedData.Throw(Context, "`UNKNOWN_VALUE`", "The value you specified led to no results."));
                return;
            }
            context.Replace(".", "");
            string[] splitCtxCommand = context.Split('(');
            if (splitCtxCommand.Length == 2)
            {
                string ctxCommand = splitCtxCommand[0];
                string ctxArg = splitCtxCommand[1];
                if (commands.Any(x => x.HasMatchingAliases(ctxCommand)))
                {
                    IEnumerable<CommandInfo> cmds = commands.Where(x => x.HasMatchingAliases(ctxCommand));
                    if (cmds.Count() > 1)
                    {
                        List<string> cmdsName = new List<string>();
                        if (!string.IsNullOrWhiteSpace(ctxArg))
                        {
                            if (cmds.All(x => x.Name == cmds.First().Name))
                            {
                                if (cmds.Any(x => x.Parameters.Funct()))
                                {
                                    if (cmds.Where(x => x.Parameters.Funct()).Any(x => x.Parameters.Any(y => y.HasMatchingName(ctxArg))))
                                    {
                                        ParameterInfo p = cmds.Where(x => x.Parameters.Funct()).Where(x => x.Parameters.Any(y => y.HasMatchingName(ctxArg))).OrderByDescending(x => x.Parameters.Count).First().Parameters.Where(x => x.HasMatchingName(ctxArg)).First();
                                        await ReplyAsync(embed: GetCommandArgPanelAsync(p).Build());
                                        return;
                                    }

                                    await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`UNKNOWN_ARG`", "The argument you specified for this command batch led to no results."));
                                    return;
                                }

                                await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`EMPTY_ARG`", "There are no arguments for any of the specified overloaded command."));
                                return;

                            }

                            await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`MULTIPLE_COMMANDS`", "The alias used to specify the command returned multiple variants."));
                            return;

                        }

                        await ReplyAsync(embed: GetManyInnerCommandPanelAsync(cmds).Build());
                        return;
                    }
                    else
                    {
                        CommandInfo c = cmds.First();
                        if (c.Parameters.Funct())
                        {
                            if (c.Parameters.Any(x => x.HasMatchingName(ctxArg)))
                            {
                                ParameterInfo p = c.Parameters.Where(x => x.HasMatchingName(ctxArg)).First();
                                await ReplyAsync(embed: GetCommandArgPanelAsync(p).Build());
                                return;
                            }
                            else
                            {
                                await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`UNKNOWN_ARG`", "The argument you specified for this command led to no results."));
                                return;
                            }
                        }
                        await ReplyAsync(embed: EmbedData.Throw(Context, $"{cmds.First().Name}(`EMPTY_ARG`", "There are no arguments for this command."));
                        return;
                    }

                    await ReplyAsync(embed: EmbedData.Throw(Context, "`UNKNOWN_VALUE`", "The value you specified led to no results."));
                    return;
                }
            }
            context.Replace("(", "");
            if (modules.Any(x => x.HasMatchingAliases(context)))
            {
                ModuleInfo m = modules.Where(x => x.HasMatchingAliases(context)).First();
                if (m.Group.Exists())
                {
                    await GetInnerGroupPanelAsync(m);
                    return;
                }
                await GetInnerModulePanelAsync(m);
                return;
            }
            
            if (commands.Any(x => x.HasMatchingAliases(context)))
            {
                IEnumerable<CommandInfo> cmds = commands.Where(x => x.HasMatchingAliases(context));
                if (cmds.Count() > 1)
                {
                    await ReplyAsync(embed: GetManyInnerCommandPanelAsync(cmds).Build());
                }
                else
                {
                    await ReplyAsync(embed: BuildCommandSummary(cmds.First()).Build());
                }
                return;
            }
            await ReplyAsync(embed: EmbedData.Throw(Context, "`UNKNOWN_VALUE`", "The value you specified led to no results."));
        }

        [Command("oldhelp"), Alias("oh"), Priority(0)]
        [Summary("Returns a collection of all root modules.")]
        public async Task GetHelp2Async()
        {
            await GetModulePanelAsync();
        }

        [Command("help"), Alias("h"), Priority(1)]
        [Summary("Search Orikivo's index to learn more about a module, command, or argument.")]
        public async Task GetHelpingAsync([Remainder]string context)
        {
            CommandHelper ch = new CommandHelper(Context, _service);
            await ch.SearchFunctionAsync(context);
        }

        [Command("help"), Alias("h"), Priority(0)]
        [Summary("View the complete collection of all existing parent modules.")]
        public async Task GetHelping2Async(int page = 1)
        {
            CommandHelper ch = new CommandHelper(Context, _service);
            await Context.Channel.SendEmbedAsync(ch.ShowMainAsync(page));
        }

        public List<string> BuildModuleSummaries(IEnumerable<ModuleInfo> modules)
        {
            List<string> l = new List<string>();
            foreach (ModuleInfo m in modules)
            {
                l.Add($"{EmojiIndex.Experience.Pack(Context.Account)}{m.GetName().MarkdownBold()} - {m.GetSummary()}");
            }
            return l;
        }

        public List<EmbedBuilder> GenerateHelperBook(List<EmbedFieldBuilder> fields, EmbedBuilder e = null)
        {
            const int MAX_FIELDS = 25;
            const int MAX_CHARACTERS = 6000;

            int chars = e.Length;

            e = e ?? GetDefaultEmbed();
            EmbedBuilder b = e;

            List<EmbedBuilder> embeds = new List<EmbedBuilder>();
            for(int i = 0; i < fields.Count; i++)
            {
                if (chars > MAX_CHARACTERS - 1 || e.Fields.Count > MAX_FIELDS - 1)
                {
                    embeds.Add(e);
                    e = b;
                }

                e.AddField(fields[i]);
                chars += fields[i].GetLength();
            }
            if (e.Fields.Count > 0)
            {
                embeds.Add(e);
                e = b;
            }

            return embeds;
        }

        public List<EmbedFieldBuilder> GenerateModuleSummaries(IEnumerable<ModuleInfo> modules)
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            foreach (ModuleInfo module in modules)
            {
                fields.Add(GenerateModuleSummary(module));
            }

            return fields;
        }

        public EmbedFieldBuilder GenerateModuleSummary(ModuleInfo module)
        {
            return new EmbedFieldBuilder().WithName(module.GetName()).WithValue(module.GetSummary()).WithIsInline(false);
        }

        public IEnumerable<ModuleInfo> GetActiveModules()
        {
            return Context.Data.Modules.Where(x => x.Name != "Hidden" && x.IsActive(_service.Modules)).OrderBy(x => x.GetName());
        }

        public async Task GetCommandInfoPanelAsync()
        {

        }

        public async Task GetModuleInfoPanelAsync()
        {

        }

        [Command("module")]
        [Summary("View information on a specific module.")]
        public async Task GetModuleResponseAsync(string module)
        {
            if (!Context.Data.Modules.TryGetModule(module, out ModuleInfo m))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Invalid context.", "The module you specified does not exist."));
                return;
            }

            await GetModuleAsync(m);
        }

        [Command("botstatus"), Alias("bst")]
        public async Task GetModulesResponseAsync(int page = 1)
        {
            await GetModulesAsync(page);
        }

        //[Command("types")]
        public async Task ViewTypesResponseAsync()
        {
            // view more info about a parameter type.
        }

        //[Command("type")]
        public async Task ViewTypeResponseAsync()
        {
            // view more info about a specific type
            // and what it handles
        }

        //[Command("exceptions")]
        public async Task ViewExceptionsResponseAsync()
        {
            // learn about exceptions
            // and why they might occur.
        }

        [Command("unload")]
        [Summary("Unload a module.")]
        public async Task UnloadModuleResponseAsync(string module)
        {
            if (module.ToLower() == "core")
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Module conflict.", "Core modules cannot be unloaded."));
                return;
            }

            if (!Context.User.IsCreator())
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Missing permissions.", "This command is built solely for the owner of the bot."));
                return;
            }

            if (!_service.Modules.TryGetModule(module, out ModuleInfo m))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Invalid context.", "The module you specified does not exist."));
                return;
            }

            await UnloadModuleAsync(m);
        }

        // definercategory
        // help - general basic usage
        // about - information about what the command is
        // aliases - all shortcuts to the command
        // syntax - descriptive box about what the command takes in
        // args - a list of all arguments in a command
        // arg - a detailed box about a specified argument
        // status - shows the current context status, with a counter on any completed and pending reports // declined are hidden
        //                                  reports are color-coded based on priority
        // reports - a list of reports, with a list of all reports on a specified context.
        // organizer
        // make all summaries of the bot in a seperate file
        // make all aliases of the bot in a seperate file
        // module // Core
        // submodule // Core.Utility
        // command // Core.Help()
        // command w/ multiple definers // Core.Help() +3
        // command batch // Core.Help[]
        // arg // Help(string context)
        // [] = command w/ overloads, <> = command w/ subcommands, () command.

        #region Core.About[] +2
        //[Command("about"), Priority(2)]
        public async Task AboutResponseAsync(string command, string context)
        {
            // try to find a subcommand in a command
            // get general info
            // place in embed
            // send
        }

        //[Command("about"), Priority(1)]
        // ctx is a module or command name.
        public async Task AboutResponseAsync(string context)
        {
            // try to find a command/module
            // get general info
            // place in embed
            // send
        }

        public string CompareDates(DateTime past, DateTime present)
        {
            TimeSpan ts = present - past;
            StringBuilder sb = new StringBuilder();

            if (ts.TotalDays > 1)
            {
                sb.Append($"{Math.Round(ts.TotalDays)} days");
            }
            else
            {
                if (ts.TotalHours > 1)
                {
                    sb.Append($"{Math.Round(ts.TotalHours)} hours");
                }
                else
                {
                    if (ts.TotalMinutes > 1)
                    {
                        sb.Append($"{Math.Round(ts.TotalMinutes)} minutes");
                    }
                    else
                    {
                        if (ts.TotalSeconds > 1)
                        {
                            sb.Append($"{Math.Round(ts.TotalSeconds)} seconds");
                        }
                        else
                        {
                                sb.Append($"{Math.Round(ts.TotalMilliseconds)} milliseconds");
                        }
                    }
                }
            }
            sb.Append(" since launch.");

            return sb.ToString();
            // example : 189 days since launch. :)
            //return ts.ToString(formatter);
           
        }

        [Command("about"), Priority(0)]
        public async Task AboutResponseAsync()
        {
            OldAccount a = Context.Account;
            DateTime past = Context.Client.CurrentUser.CreatedAt.DateTime;
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle("What is an \"Orikivo\"?");

            EmbedFooterBuilder f = new EmbedFooterBuilder();
            f.WithText($"Version {OldGlobal.ClientVersion} | {CompareDates(past, DateTime.Now)}");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{EmojiIndex.Topic.Pack(a)} Orikivo is a personal project that originally started a joke. However, due to my lack of self-control, it has expanded to a collection of commands, free for anyone to use.");
            sb.AppendLine($"\n**I apologize for incompletion. I still have much to clean!**");
            sb.AppendLine($"{_service.Commands.Count()} whopping commands! :o");
            sb.AppendLine($"The structure of Orikivo is using **Discord.Net** ({Discord.DiscordConfig.Version}).");
            sb.AppendLine($"All other systems and API wrappers have been built by hand, with the assistance of helpful users, Stack Overflow, and careful documentation reading.");
            e.WithFooter(f);

            e.WithDescription(sb.ToString());
            await ReplyAsync(embed: e.Build());
        }
        #endregion

        #region Core.Aliases[] +1
        [Command("alias"), Priority(1)]
        public async Task AliasesResponseAsync(string context)
        {
            if (!_service.TryGetCommand(context, out CommandInfo cmd))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "This command could not be found.", "As of now, alias cannot be used to search with.", false));
                return;
            }

            CommandSyntax syntax = cmd.GetSyntax();

            Embed e = GenerateAliasBox(syntax);

            await ReplyAsync(embed: e);
            // try to find a command/module/subcommand
            // get all aliases
            // place in embed
            // send
        }

        //[Command("aliases"), Priority(0)]
        public async Task AliasesResponseAsync(string command, string arg)
        {
            // try to find an argument in a command



            // get all aliases
            // place in embed
            // send
        }
        #endregion

        #region Core.Arguments()
        //[Command("arguments")]
        public async Task ArgumentsResponseAsync(string command)
        {
            // try to find a command
            // get info
            // place in embed
            // send
        }
        #endregion

        #region Core.Argument()
        //[Command("argument")]
        public async Task ArgumentResponseAsync(string command, string argument)
        {
            if (!_service.TryGetCommand(command, out CommandInfo cmd))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "This command could not be found."));
                return;
            }

            // cmd.TryGetArg(argument, out ArgumentInfo arg);
            // Embed e = GenerateArgBox(arg);

            //await ReplyAsync(embed: e);

            // try to find an argument in a command
            // get info
            // place in embed
            // send
        }
        #endregion

        #region Core.Commands[] +1
        //[Command("commands"), Priority(1)]
        public async Task CommandsResponseAsync(string module) { }

        //[Command("commands"), Priority(0)]
        public async Task CommandsResponseAsync() { }
        #endregion

        #region Core.Help[] +2
        public async Task BuildHelperPanel(string context)
        {
            /*
             Goals:

            Return the default helper panel if no context was provided.

            Allow search contexts to return more than one match, and display a list of a collisions.

            Hide duplicate commands from showing if they have the exact same name, with different methods.
            This way, if that one command name was searched for, all methods appear as one.

            Treat Command batches as a singular command,
            with their subcommand counterparts appearing inside the main help box.

            Give all helper panels a footer to navigate the different helping variants of information.

            Treat Submodules as a inner group, from which they have their own inner commands,
                just like a module.
             */
        }

        /// <summary>
        /// Returns a collection of modules that are known as a root module.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModuleInfo> GetRootModulesAsync()
        {
            return _service.Modules.Where(x => !x.IsSubmodule && !x.Group.Exists());
        }

        /// <summary>
        /// Returns a collection of modules that are wrapped inside another module.
        /// </summary>
        public IEnumerable<ModuleInfo> GetSubmodulesAsync()
        {
            return _service.Modules.Where(x => x.IsSubmodule && !x.Group.Exists());
        }

        /// <summary>
        /// Returns a collection of modules that are a base for inner commands.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModuleInfo> GetCommandGroupsAsync()
        {
            return _service.Modules.Where(x => x.Group.Exists());
        }

        //[Command("help"), Priority(2)]
        public async Task HelpResponseAsync(string command, string subcommand)
        {
            // try to find a subcommand from a command
            // get info
            // place in embed
            // send
        }
        #endregion

        #region Core.Latency()
        [Command("latency"), Alias("ping")]
        public async Task LatencyResponseAsync() { await ModuleManager.TryExecute(Context.Channel, GetLatencyAsync()); }
        #endregion

        #region Core.Module()
        //[Command("module")]
        public async Task ModuleResponseAsync(string name) { }
        #endregion

        #region Core.Modules()
        //[Command("modules")]
        public async Task ModuleResponseAsync() { }
        #endregion

        #region Core.Prefix()
        //[Command("prefix")]
        public async Task PrefixResponseAsync()
        {
           // view the server prefix
        }
        #endregion

        #region Core.Reports[] +1
        //[Command("reports"), Priority(1)]
        public async Task ReportsResponseAsync(string context)
        {
            // try to search for reports on a module, command, or from a user.
        }

        //[Command("reports"), Priority(0)]
        public async Task ReportsResponseAsync()
        {
            // open a list of reports
        }
        #endregion

        #region Core.Report[] +1
        //[Command("report"), Priority(1)]
        public async Task ReportResponseAsync(ulong id)
        {
            // try to find a report by id
            // place in embed
            // send
        }

        //[Command("report"), Priority(0)]
        public async Task ReportResponseAsync(string name)
        {
            // try to find a report with the subject of a this name
            // place in embed
            // send
        }
        #endregion

        #region Core.Status[] +1
        //[Command("status"), Priority(1)]
        public async Task StatusResponseAsync(string context)
        {
            // search for a command or module
            // place in embed
            // send
        }

        //[Command("stats"), Priority(0)]
        public async Task StatusResponseAsync()
        {
            int gc = Context.Client.Guilds.Count();
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle("Status (incomplete)");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Guilds: {gc}");


            await ReplyAsync(embed: e.Build());


            // Get orikivo's main status
            // place in embed
            // send
        }
        #endregion

        #region Core.Syntax()
        [Command("syntax")]
        public async Task GetSyntaxResponseAsync([Remainder]string command)
        {
            if (!_service.TryGetCommand(command, out CommandInfo cmd))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "The context provided yielded no results."));
                return;
            }

            CommandSyntax syntax = cmd.GetSyntax();

            Embed e = GenerateSyntaxBox(syntax);

            await ReplyAsync(embed: e);

            // search for command
            // get syntax collection, with main name and type.
            // place in embed.
            // send
        }
        #endregion

        #region Core.Version()
        [Command("version")]
        public async Task GetVersionResponseAsync()
        {
            await ModuleManager.TryExecute(Context.Channel, GetVersionAsync());
            
        }

        public async Task GetVersionAsync()
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(OldGlobal.ClientVersion); //Context.Global.Version.ToString()
            await ReplyAsync(embed: e.Build());
        }


        #endregion

        public Embed GenerateAliasBox(CommandSyntax syntax)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(syntax.FullName);
            if (syntax.Alias.Exists()) e.WithDescription(string.Join(", ", syntax.Alias));

            return e.Build();
        }

        public Embed GenerateSyntaxBox(CommandSyntax syntax)
        {
            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle(GetSyntaxBoxTitle(syntax));
            List<EmbedFieldBuilder> argFields = GetArgFields(syntax.Arguments);
            if (argFields.Funct())
            {
                e.Fields = argFields;
            }
            // arg summaries

            return e.Build();
        }

        public List<EmbedFieldBuilder> GetArgFields(IEnumerable<ArgumentInfo> args)
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            if (!args.Funct())
            {
                return null;
            }
            foreach (ArgumentInfo arg in args)
            {
                fields.Add(GetArgField(arg));
            }
            return fields;
        }

        public EmbedFieldBuilder GetArgField(ArgumentInfo arg)
        {
            EmbedFieldBuilder f = new EmbedFieldBuilder();
            f.WithName(arg.Name);
            f.WithValue(GetArgSummary(arg));
            f.WithIsInline(false);
            return f;
        }

        public string GetArgSummary(ArgumentInfo arg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(arg.Type.FullName);
            sb.Append(arg.Summary.Default);
            return sb.ToString();
        }

        public string GetSyntaxBoxTitle(CommandSyntax syntax)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(Context.Server.Config.GetPrefix(Context));
            sb.Append($"{syntax.Name}({syntax.TryStateArgs()})");
            return sb.ToString();
        }

        //hardware
        [Name("Hardware")]
        [Summary("Provides information on computer analytics.")]
        public class HardwareModule : ModuleBase<OrikivoCommandContext>
        {
            private readonly CommandService _service;
            private readonly DiscordSocketClient _socket;
            private readonly IConfigurationRoot _config;

            public HardwareModule
            (
                CommandService service,
                DiscordSocketClient socket,
                IConfigurationRoot config
            )
            {
                _socket = socket;
                _service = service;
                _config = config;
            }

            // get info about the hosting computer.
            public async Task HostResponseAsync()
            {
            }

            [Command("execute"), Alias("exec")]
            [Summary("Execute a process from the Command Prompt.")]
            [RequireOwner]
            public async Task ProcessAsync(string file, [Remainder]string processInfo)
            {
                var process = new Process();
                var info = new ProcessStartInfo();
                info.FileName = file;
                info.Arguments = processInfo;
                info.RedirectStandardOutput = true;
                process.StartInfo = info;
                process.Start();

                var e = EmbedData.SetEmbed($"Executing process...", null,
                    null, null, false,
                    "213,16,93", null, EmbedData.SetFooter(null, $"{info.FileName}: {processInfo}"));
                var emb = await Context.Channel.SendMessageAsync(null, false, e.Build());

                if (!processInfo.StartsWith("shutdown"))
                {
                    var result = "";
                    while (!process.StandardOutput.EndOfStream)
                    {
                        result += process.StandardOutput.ReadLine() + "\n";
                    }
                    Console.WriteLine(result + "\n");
                    e.WithTitle("Executed process.");
                    e.WithDescription($"```{(result.Length > 1018 ? result.Substring(0, 1018) : result)}```");
                    //e.WithFooter(EmbedData.SetFooter(null, $"{processInfo}"));
                    e.WithColor(129, 243, 193);
                    await emb.ModifyAsync(x => { x.Embed = e.Build(); });
                }
            }

            [Command("commandprompt"), Alias("cli")]
            [Summary("Use the command prompt on the current computer.")]
            [RequireOwner]
            public async Task UseCommandPromptAsync([Remainder]string processInfo)
            {
                var cmd = new Process();
                var cmdRef = new ProcessStartInfo();
                cmdRef.WindowStyle = ProcessWindowStyle.Hidden;
                cmdRef.FileName = "cmd.exe";
                cmdRef.Arguments = $"/C {processInfo}";
                cmdRef.RedirectStandardOutput = true;
                cmd.StartInfo = cmdRef;
                cmd.Start();

                var e = EmbedData.SetEmbed($"Executing process...", null,
                    null, null, false,
                    "213,16,93", null, EmbedData.SetFooter(null, $"{cmdRef.FileName}: {processInfo}"));
                var emb = await Context.Channel.SendMessageAsync(null, false, e.Build());


                if (!processInfo.StartsWith("shutdown"))
                {
                    var result = "";
                    while (!cmd.StandardOutput.EndOfStream)
                    {
                        result += cmd.StandardOutput.ReadLine() + "\n";
                    }
                    Console.WriteLine(result + "\n");
                    e.WithTitle("Executed process.");
                    e.WithDescription($"```{(result.Length > 1018 ? result.Substring(0, 1018) : result)}```");
                    //e.WithFooter(EmbedData.SetFooter(null, $"{processInfo}"));
                    e.WithColor(129, 243, 193);
                    await emb.ModifyAsync(x => { x.Embed = e.Build(); });
                }
            }
            // readdirectory - get a list of files in a directory
            [Command("sendfile"), Alias("sf")]
            [Summary("Send an existing file from the host to Discord.")]
            [RequireOwner]
            public async Task SendFileAsync([Remainder]string url)
            {
                if (url.StartsWith("[desktop]"))
                {
                    url = url.Replace("[desktop]", @"C:\Users\AbnerSquared\Desktop\");
                }
                else if (url.StartsWith("[main]"))
                {
                    url = url.Replace("[main]", @"C:\Users\AbnerSquared\Desktop\base\");
                }

                var e = EmbedData.SetEmbed($"Searching for file...", null,
                    null, null, false,
                    "255, 238, 129", null, EmbedData.SetFooter(null, $"{Environment.MachineName}: {url}"));
                var emb = await Context.Channel.SendMessageAsync(null, false, e.Build());
                var fileSizeB = new FileInfo(url).Length;
                var fileSizeKB = fileSizeB / 1024;
                var fileSizeMB = fileSizeKB / 1024;
                Console.WriteLine($"{fileSizeB}\n{fileSizeKB}\n{fileSizeMB}");
                if (!File.Exists(url))
                {
                    e.WithTitle($"File not found.");
                    e.WithColor(213, 16, 93);
                    await emb.ModifyAsync(x => { x.Embed = e.Build(); });
                }
                else if (fileSizeMB > 8)
                {
                    e.WithTitle($"File size too large. ({fileSizeMB}MB)");
                    e.WithColor(213, 16, 93);
                    await emb.ModifyAsync(x => { x.Embed = e.Build(); });
                }
                else
                {
                    e.WithTitle($"File found.");
                    e.WithColor(129, 243, 193);
                    await emb.ModifyAsync(x => { x.Embed = e.Build(); });
                    await Context.Channel.SendFileAsync(url);
                }
            }

            [Command("taskmanager"), Alias("tasks")]
            [Summary("Gathers all current processes running.")]
            public async Task CollectProcessesAsync(string sortType = null, string direction = null)
            {
                if (direction == null)
                {
                    direction = "";
                }

                var processList = Process.GetProcesses();
                var processCollection = new List<Tuple<string, string, long>>();

                foreach (var process in processList)
                {
                    processCollection.Add(new Tuple<string, string, long>($"{process.ProcessName}", $"{process.BasePriority}", process.WorkingSet64));
                }

                var summary = "";
                if (sortType == null)
                {
                    processCollection = processCollection.OrderByDescending(x => x.Item3).ToList();
                }
                else
                {
                    sortType = sortType.ToLower();
                    direction = direction.ToLower();
                    if (sortType.Equals("name"))
                    {
                        if (direction == null || (!direction.Equals("descending") && !direction.Equals("desc") && !direction.Equals("ascending") && !direction.Equals("asc")))
                        {
                            processCollection = processCollection.OrderBy(x => x.Item1).ToList();
                        }
                        else
                        {
                            if (direction.Equals("ascending") || direction.Equals("asc"))
                            {
                                processCollection = processCollection.OrderBy(x => x.Item1).ToList();
                            }
                            else if (direction.Equals("descending") || direction.Equals("desc"))
                            {
                                processCollection = processCollection.OrderByDescending(x => x.Item1).ToList();
                            }
                        }
                    }
                    else if (sortType.Equals("priority"))
                    {
                        if (direction == null || (!direction.Equals("descending") && !direction.Equals("desc") && !direction.Equals("ascending") && !direction.Equals("asc")))
                        {
                            processCollection = processCollection.OrderByDescending(x => int.Parse(x.Item2)).ToList();
                        }
                        else
                        {
                            if (direction.Equals("ascending") || direction.Equals("asc"))
                            {
                                processCollection = processCollection.OrderBy(x => int.Parse(x.Item2)).ToList();
                            }
                            else if (direction.Equals("descending") || direction.Equals("desc"))
                            {
                                processCollection = processCollection.OrderByDescending(x => int.Parse(x.Item2)).ToList();
                            }
                        }
                    }
                    else if (sortType.Equals("usage"))
                    {

                        if (direction == null || (!direction.Equals("descending") && !direction.Equals("desc") && !direction.Equals("ascending") && !direction.Equals("asc")))
                        {
                            processCollection = processCollection.OrderByDescending(x => x.Item3).ToList();
                        }
                        else
                        {
                            if (direction.Equals("ascending") || direction.Equals("asc"))
                            {
                                processCollection = processCollection.OrderBy(x => x.Item3).ToList();
                            }
                            else if (direction.Equals("descending") || direction.Equals("desc"))
                            {
                                processCollection = processCollection.OrderByDescending(x => x.Item3).ToList();
                            }
                        }
                    }
                }



                foreach (var prc in processCollection)
                {
                    var newLine = $"{prc.Item1} [{prc.Item2}]: {FormatSize(prc.Item3)}\n";
                    if (summary.Length + newLine.Length < 1024)
                    {
                        summary += newLine;
                    }
                    else
                    {
                        break;
                    }
                }

                var e = EmbedData.SetEmbed
                    (
                        $"{Environment.MachineName} : processes",
                        summary,
                        null, null, false, "213,16,93"
                    );

                await ReplyAsync(null, false, e.Build());
            }

            static string FormatSize(long bytes)
            {
                var sizeRef = new List<string> { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
                int index = 0;
                do { bytes /= 1024; index++; }
                while (bytes >= 1024);
                return $"{bytes.ToString("0.#")} {sizeRef[index]}";
            }

            static string FormatRawSize(long bytes)
            {
                do { bytes /= 1024; }
                while (bytes >= 1024);
                return $"{bytes.ToString("0.#")}";
            }


            [Command("hardware"), Alias("hw")]
            [Summary("Collect base computer hardware information.")]
            public async Task GetHardwareInformationAsync()
            {
                // For Logged information, use Environment.
                var cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                var ramUsage = new PerformanceCounter("Memory", "Available MBytes");



                float cpuUsageData = 0.0f;
                float ramUsageData = 0.0f;


                var f = EmbedData.SetEmbed
                    (
                        $"Calculating usage...",
                        null,
                        null, null, false, "213,16,93"
                    );


                var checker = await Context.Channel.SendMessageAsync(null, false, f.Build());

                cpuUsage.NextValue();
                ramUsage.NextValue();
                await Task.Delay(1000);
                cpuUsageData = cpuUsage.NextValue();
                ramUsageData = ramUsage.NextValue();

                cpuUsageData = (float)Math.Round(cpuUsageData, 2);

                var cpuUsageDisp = $"{cpuUsageData}%";
                var ramUsageDisp = $"{ramUsageData} MB";

                var e = EmbedData.SetEmbed
                    (
                        $"{Environment.MachineName} : hardware-statistics",
                        $"cpu.usage : {cpuUsageDisp}\n" +
                        $"ram.available : {ramUsageDisp}",
                        null, null, false, "129,243,193"
                    );

                await checker.ModifyAsync(x => { x.Embed = e.Build(); });
            }

            //[Command("hardwareconst"), Alias("hwc")]
            //[Summary("Collect base computer hardware information.")]
            public async Task GetConstantHardwareInformationAsync()
            {
                // For Logged information, use Environment.
                var cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                var ramUsage = new PerformanceCounter("Memory", "Available MBytes");
                float cpuUsageData = 0.0f;
                float ramUsageData = 0.0f;


                var f = EmbedData.SetEmbed
                    (
                        $"Calculating usage...",
                        null,
                        null, null, false, "213,16,93"
                    );


                var checker = await Context.Channel.SendMessageAsync(null, false, f.Build());
                var newMessage = false;

                var recentMsgBuild = Context.Channel.GetMessagesAsync(1);
                var recentMsgList = recentMsgBuild.ToList();

                IMessage previousMsg;
                IMessage currentMsg;




                while (!newMessage)
                {
                    cpuUsage.NextValue();
                    ramUsage.NextValue();
                    await Task.Delay(1000);
                    previousMsg = recentMsgList.Result[0].First();
                    cpuUsageData = cpuUsage.NextValue();
                    cpuUsageData = (float)Math.Round(cpuUsageData, 2);
                    ramUsageData = ramUsage.NextValue();

                    var cpuUsageDisp = $"{cpuUsageData}%";
                    var ramUsageDisp = $"{ramUsageData} MB";

                    var e = EmbedData.SetEmbed
                        (
                            $"{Environment.MachineName} : hardware-statistics",
                            $"cpu.usage : {cpuUsageDisp}\n" +
                            $"ram.available : {ramUsageDisp}",
                            null, null, false, "129,243,193"
                        );

                    recentMsgBuild = Context.Channel.GetMessagesAsync(1);
                    recentMsgList = recentMsgBuild.ToList();

                    await Task.Delay(1000);

                    currentMsg = recentMsgList.Result[0].First();

                    if (currentMsg != previousMsg)
                    {
                        e.WithColor(255, 238, 129);
                        await checker.ModifyAsync(x => { x.Embed = e.Build(); });
                        return;
                    }
                    else
                    {
                        await checker.ModifyAsync(x => { x.Embed = e.Build(); });
                    }
                }
            }

            [Command("system"), Alias("sys")]
            [Summary("Collect base computer system information.")]
            public async Task GetSystemInformationAsync()
            {
                var is64Os = Environment.Is64BitOperatingSystem;
                var is64Process = Environment.Is64BitProcess;
                var machineName = Environment.MachineName;
                var osVersion = Environment.OSVersion;
                var processors = Environment.ProcessorCount;
                var pageSize = Environment.SystemPageSize;
                var user = Environment.UserName;

                var e = EmbedData.SetEmbed(
                    $"{machineName} : system-information",
                    $"os.version: {osVersion}\n" +
                    $"processor.count: {processors}\n" +
                    $"page.size: {pageSize}\n" +
                    $"current.user: {user}",
                    null, null, false, "129,243,193");

                await ReplyAsync(null, false, e.Build());
            }

            [Command("shutdown"), Alias("exit")]
            [RequireOwner]
            [Summary("Shut down Orikivo.")]
            public async Task CloseApplicationAsync()
            {
                // Use a confirmation sequence.
            }

            [Command("uptime"), Alias("upt")]
            [Summary("Retrieve how long this computer has been on.")]
            public async Task GetUptimeAsync()
            {
                int rawUptime = Environment.TickCount;

                // Convert to a timer.
                int seconds = rawUptime / 1000;
                int minutes = 0;
                int hours = 0;
                int days = 0;

                for (int min = 0; seconds >= 60; min++)
                {
                    seconds -= 60;
                    minutes = min;
                }
                for (int hr = 0; minutes >= 60; hr++)
                {
                    minutes -= 60;
                    hours = hr;
                }
                for (int d = 0; hours >= 24; d++)
                {
                    hours -= 24;
                    days = d;
                }

                Console.WriteLine($"s:{seconds}\nm:{minutes}\nh:{hours}\nd:{days}");
                string displayComputerName = Environment.MachineName;
                string displayUptime =
                    $"{displayComputerName} has been on for " +
                    (days > 0 ? $"{days} day{(days > 1 ? "s" : "")}{(hours > 0 || minutes > 0 || seconds > 0 ? ", " : ".")}" : "") +
                    ((hours > 0 & minutes == 0 & seconds == 0) ? "and " : "") +
                    (hours > 0 ? $"{hours} hour{(hours > 1 ? "s" : "")}{(minutes > 0 || seconds > 0 ? ", " : ".")}" : "") +
                    ((minutes > 0 & seconds == 0) ? "and " : "") +
                    (minutes > 0 ? $"{minutes} minute{(minutes > 1 ? "s" : "")}{(seconds > 0 ? ", " : ".")}" : "") +
                    ((seconds > 0) ? "and " : "") +
                    (seconds > 0 ? $"{seconds} second{(seconds > 1 ? "s" : "")}." : "");

                EmbedBuilder e = EmbedData.SetEmbed(null, $"{displayUptime}", null, null, false, "129,243,193");
                e.WithFooter($"Raw Uptime | {rawUptime}ms");
                await ReplyAsync(null, false, e.Build());
            }


        }
    }

    public class CommandSyntax
    {
        public CommandSyntax(CommandInfo command)
        {
            FullName = command.GetFullName();
            if (command.Module.Group.Exists())
            {
                if (command.GetName() == "")
                {
                    Name = command.Module.Group;
                }
            }
            else
            {
                Name = command.GetName();
            }
            Arguments = command.TryGetArgs();
            //Alias = command.Aliases.ToList();
            //Alias.Remove(Name);
        }

        public CommandSyntax(string fullName, string name, IEnumerable<ArgumentInfo> args)
        {
            FullName = fullName;
            Name = name;
            Arguments = args;
        }

        public string FullName { get; set; } // complete full name.
        public string Name { get; set; } // default name of command
        public List<string> Alias { get; set; } // the sub callings of the command.
        public IEnumerable<ArgumentInfo> Arguments { get; set; }

        public string GetArgs()
        {
            if (Arguments.Funct())
            {
                List<string> args = new List<string>();
                foreach (ArgumentInfo arg in Arguments)
                {
                    args.Add(arg.SimpleString());
                    //args.Add(arg.Name);
                }

                return string.Join(", ", args);
            }

            return string.Empty;
        }
        public string TryStateArgs()
        {
            if (Arguments.Funct())
            {
                List<string> args = new List<string>();
                foreach (ArgumentInfo arg in Arguments)
                {
                    args.Add(arg.ToString());
                    //args.Add(arg.Name);
                }

                return string.Join(", ", args);
            }

            return string.Empty;
        }
        // all params in a command
    }

    public class ArgumentInfo
    {
        public ArgumentInfo(string name, Type type, bool optional, ArgumentSummary summary)
        {
            Name = name;
            Type = type;
            Optional = optional;
            Summary = summary;
        }

        public string Name { get; set; } // the name of the argument
        public Type Type { get; set; } // the type of argument
        public bool Optional { get; set; } // if the command is required
        public ArgumentSummary Summary { get; set; }
        // collects and holds all types of summaries
        public string SimpleString()
        {
            return $"{Name}{(Optional?"?":"")}";
        }
        public override string ToString()
        {
            return $"{Type.Name}{(Optional?"?":"")} {Name}";
        }
    }

    public class ArgumentSummary
    {
        public ArgumentSummary(string def = null, string description = null, string summary = null)
        {
            Default = def;
            Description = description;
            Summary = summary;
        }

        public string Default { get; set; } // default parameter summary.
        public string Description { get; set; } // custom short summary.
        public string Summary { get; set; } // custom full summary.
    }
}