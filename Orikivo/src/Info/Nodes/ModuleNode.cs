using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // EmbedNode ?? : This would allow you to set up the framework for an embed.
    // .ToEmbed();
    public class ModuleNode : ContextNode
    {
        public ModuleNode(ModuleInfo module) : base(module)
        {
            Group = module.Group;
            Subtitle = module.Attributes.OfType<DescriptionAttribute>().FirstOrDefault()?.Content;

            Submodules = module.Submodules.Select(s => new ModuleNode(s)).ToList();

            // GETTING COMMAND CLUSTERS
            Dictionary<string, List<CommandInfo>> commandGroups = new Dictionary<string, List<CommandInfo>>();

            foreach (CommandInfo command in module.Commands)
            {
                if (!commandGroups.TryAdd(command.Name, command.AsList()))
                    commandGroups[command.Name].Add(command);
            }

            Commands = commandGroups.Select(x => new CommandNode(x.Value)).ToList();
        }

        public string Group { get; protected set; }
        public string Subtitle { get; protected set; }
        public int CommandCount => Submodules.Sum(x => x.CommandCount) + Commands.Count;
        public int InnerCount => CommandCount + Submodules.Where(x => x.IsGroup).Count();
        public bool IsGroup => Check.NotNull(Group);
        public List<ModuleNode> Submodules { get; protected set; }
        public List<CommandNode> Commands { get; protected set; }

        public override InfoType Type => InfoType.Module;
        protected override string Formatting
        {
            get
            {
                StringBuilder format = new StringBuilder();

                if (IsGroup) // GROUP FORMATTING
                {
                    format.Append($"**{Group}**");
                    format.Append(" •()");
                    format.AppendLine();

                    if (Check.NotNull(Summary))
                    {
                        format.Append("⇛ ");
                        format.Append(Summary);
                        format.AppendLine();          
                    }

                    if (Commands.Count > 0)
                    {
                        format.AppendLine();
                        format.Append($"**Methods**");

                        foreach(CommandNode command in Commands.OrderBy(x => x.Name))
                        {
                            foreach (OverloadNode overload in command.Overloads.OrderBy(x => x.Index))
                            {
                                format.AppendLine();

                                string name = Check.NotNull(overload.Name) ? $"{Group} **{overload.Name}**" :
                                    $"**{Group}**";

                                format.Append("> ");
                                format.Append(name);
                                format.Append("(");
                                if (overload.Parameters.Count > 0)
                                {
                                    // implement Parameters
                                    format.AppendJoin(", ", overload.Parameters.Select(x => x.Syntax));
                                }
                                format.Append(")");

                                if (command.Overloads.Count > 1)
                                {
                                    format.Append(' ');
                                    format.Append(Format.Subscript($"+{overload.Index}"));
                                }

                                
                            }
                        }
                    }
                }
                else // MODULE FORMATTING
                {
                    format.Append($"**{Name}**");

                    if (Check.NotNull(Summary))
                    {
                        format.Append($"```{Summary}```");
                    }

                    format.AppendLine();

                    if (Commands.Count > 0)
                    {
                        format.AppendLine("**Commands**");

                        format.Append($"> ");

                        List<string> values = new List<string>();

                        foreach (ModuleNode group in Submodules.Where(x => x.IsGroup))
                            values.Add($"`{group.Group}`**\\***");

                        foreach (CommandNode command in Commands.OrderBy(x => x.Name))
                        {
                            string term = $"`{command.Name}`";

                            if (command.Overloads.Count > 1)
                                term += $"**+{command.Overloads.Count - 1}**";

                            values.Add(term);
                        }

                        if (values.Count > 0)
                            format.AppendJoin(" ", values.OrderBy(x => x.Substring(1)));
                    }

                    if (Submodules.Any(x => !x.IsGroup))
                    {
                        format.AppendLine($"**Directory**");

                        foreach (ModuleNode submodule in Submodules.Where(x => !x.IsGroup).OrderBy(x => x.Name))
                        {
                            format.Append("• ");

                            format.Append($"**{submodule.Name}**");

                            if (Check.NotNull(submodule.Summary) || submodule.Commands.Count > 0)
                                format.Append(": ");

                            if (Check.NotNull(submodule.Subtitle))
                            {
                                format.AppendLine(Subtitle);
                            }

                            if (submodule.Commands.Count > 0)
                            {
                                int inserted = 0;
                                foreach (CommandNode command in submodule.Commands.OrderBy(x => x.Name))
                                {
                                    if (inserted >= 3)
                                        break;

                                    if (inserted > 0)
                                        format.Append(" ");

                                    format.Append($"`{command.Name}`");

                                    inserted++;
                                }

                                if (submodule.Commands.Count() - inserted > 0)
                                    format.Append($" (+**{submodule.Commands.Count() - inserted}** more)");

                                format.AppendLine();
                            }
                        }
                    }
                }



                // id
                if (Check.NotNull(Id))
                {
                    format.AppendLine();
                    format.Append($"**ID**: `{Id}`");
                }


                return format.ToString();
            }
        } // it has to be separated into a group layout and a module layout
    }
}
