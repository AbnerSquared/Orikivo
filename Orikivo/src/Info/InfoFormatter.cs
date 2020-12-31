﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Orikivo.Text;

namespace Orikivo
{
    // TODO: Find a better way to handle localization
    public class InfoFormatter
    {
        private const int MAX_COMMAND_DISPLAY = 3;

        public InfoFormatter(LocaleProvider locale)
        {
            Locale = locale;
        }

        protected LocaleProvider Locale { get; }

        public virtual string ViewMenu(InfoService service, BaseUser user = null)
        {
            bool showTooltips = user?.Config.Tooltips ?? true;
            Language language = user?.Config.Language ?? Language.English;

            var panel = new StringBuilder();

            if (showTooltips)
            {
                var tooltips = new List<string>();

                if (user == null)
                {
                    tooltips.Add(Locale.GetValue("help_tooltip_newbie", language));
                }

                tooltips.Add(Locale.GetValue("help_tooltip_default", language));

                if (tooltips.Count > 1)
                {
                    panel.AppendLine($"{Format.Tooltip(tooltips)}\n");
                    panel.AppendLine($"> **{Locale.GetValue("help_header", language)}**");
                }
                else
                {
                    panel.AppendLine($"> **{Locale.GetValue("help_header", language)}**");
                    panel.AppendLine($"{Format.Tooltip(tooltips)}");
                }
            }
            else
            {
                panel.AppendLine($"> **{Locale.GetValue("help_header", language)}**");
            }

            if (service.Modules.Any())
            {
                panel.AppendLine();
                panel.AppendLine($"**{Locale.GetValue("help_category_plural", language)}**");

                foreach (ModuleNode module in service.GetBaseModules().OrderBy(x => x.Name).Select(x => new ModuleNode(x)))
                {
                    panel.Append("> ");

                    if (Check.NotNull(module.Icon))
                        panel.Append($"{module.Icon} ");

                    panel.Append($"**{module.Name}**");

                    if (module.Commands.Count > 0)
                        panel.Append(": ");

                    if (module.Commands.Count > 0)
                    {
                        int inserted = 0;

                        foreach (CommandNode command in Randomizer.Shuffle(module.Commands))
                        {
                            if (inserted >= MAX_COMMAND_DISPLAY)
                                break;

                            if (inserted > 0)
                                panel.Append(" ");

                            panel.Append($"`{command.Name}`");

                            inserted++;
                        }

                        if (module.Commands.Count - inserted > 0)
                            panel.Append($" (+**{module.Commands.Count - inserted}** {Locale.GetValue("help_more", language)})");

                        panel.AppendLine();
                    }
                    else
                    {
                        panel.Append("...");
                        panel.AppendLine();
                    }
                }
            }

            return panel.ToString();
        }

        public virtual string ViewModule(ModuleNode module, BaseUser user = null)
        {
            bool showTooltips = user?.Config.Tooltips ?? true;
            Language language = user?.Config.Language ?? Language.English;

            var result = new StringBuilder();

            if (showTooltips)
            {
                var tooltips = new List<string>(module.Tooltips)
                {
                    Locale.GetValue("help_tooltip_module", language)
                };

                result.AppendLine($"{Format.Tooltip(tooltips)}\n");
            }

            string summary = Locale.GetValueOrDefault($"{module.Id}_summary", language, module.Summary);

            result.Append(Check.NotNull(module.Icon) ? module.Icon : "•");
            result.Append($" **{module.Name}**");

            if (Check.NotNull(summary))
            {
                result.Append($"```{summary}```");
            }

            result.AppendLine();

            // var commands = module.Commands.Where(x => !x.Overloads.All(y => y.CanExecute());

            if (module.Commands.Count > 0)
            {
                result.AppendLine($"**{Locale.GetValue("help_command_plural", language)}** (**{module.TotalCommandCount:##,0}**)");

                result.Append($"> ");

                var values = new List<string>();

                foreach (ModuleNode group in module.Submodules.Where(x => x.IsGroup))
                    values.Add($"`{group.Group}`**\\***");

                foreach (CommandNode command in module.Commands.OrderBy(x => x.Name))
                {
                    string term = $"`{command.Name}`";

                    if (command.Overloads.Count > 1)
                        term += $"**+{command.Overloads.Count - 1}**";

                    values.Add(term);
                }

                if (values.Count > 0)
                    result.AppendJoin(" ", values.OrderBy(x => x.Substring(1)));
            }

            if (module.Submodules.Any(x => !x.IsGroup))
            {
                result.AppendLine($"**{Locale.GetValue("help_submodule_header", language)}**");

                foreach (ModuleNode submodule in module.Submodules.Where(x => !x.IsGroup).OrderBy(x => x.Name))
                {
                    result.Append($"• **{submodule.Name}**");

                    if (submodule.Commands.Count > 0)
                        result.Append(": ");

                    if (submodule.Commands.Count > 0)
                    {
                        int inserted = 0;
                        foreach (CommandNode command in submodule.Commands.OrderBy(x => x.Name))
                        {
                            if (inserted >= 3)
                                break;

                            if (inserted > 0)
                                result.Append(" ");

                            result.Append($"`{command.Name}`");

                            inserted++;
                        }

                        if (submodule.Commands.Count - inserted > 0)
                            result.Append($" (+**{submodule.Commands.Count - inserted}** {Locale.GetValue("help_more", language)})");

                        result.AppendLine();
                    }
                }
            }

            return result.ToString();
        }

        public virtual string ViewGroup(ModuleNode group, BaseUser user = null)
        {
            bool showTooltips = user?.Config.Tooltips ?? true;
            Language language = user?.Config.Language ?? Language.English;

            string summary = Locale.GetValueOrDefault($"{group.Id}_summary", language);

            var result = new StringBuilder();

            if (showTooltips)
            {
                var tooltips = new List<string>(group.Tooltips);

                tooltips.Add(Locale.GetValue("help_tooltip_group", language, group.Name));
                result.AppendLine($"{Format.Tooltip(tooltips)}\n");
            }

            result.AppendLine($"**{group.Group}** •()");

            if (Check.NotNull(summary))
            {
                result.AppendLine($"⇛ {summary}");
            }

            if (group.Commands.Count > 0)
            {
                result.AppendLine();
                result.Append($"**{Locale.GetValue("help_methods", language)}**");

                foreach (CommandNode command in group.Commands.OrderBy(x => x.Name))
                {
                    foreach (OverloadNode overload in command.Overloads.OrderBy(x => x.Index))
                    {
                        result.AppendLine();

                        string name = Check.NotNull(overload.Name)
                            ? $"{group.Group} **{overload.Name}**"
                            : $"**{group.Group}**";

                        result.Append($"> {name}(");

                        if (overload.Parameters.Count > 0)
                        {
                            result.AppendJoin(", ", overload.Parameters.Select(x => x.Syntax));
                        }

                        result.Append(")");

                        if (command.Overloads.Count > 1)
                        {
                            result.Append($" {Format.Subscript($"+{overload.Index}")}");
                        }
                    }
                }
            }

            return result.ToString();
        }

        public virtual string ViewCommand(CommandNode command, BaseUser user = null)
        {
            bool showTooltips = user?.Config.Tooltips ?? true;
            Language language = user?.Config.Language ?? Language.English;

            if (command.Overloads.Count == 1)
                return ViewOverload(command.Default, user);

            var result = new StringBuilder();

            if (showTooltips)
            {
                var tooltips = new List<string>(command.Tooltips)
                {
                    Locale.GetValue("help_tooltip_overload", language, command.Name)
                };

                result.AppendLine($"{Format.Tooltip(tooltips)}\n");
            }

            // Group
            if (Check.NotNull(command.Group))
            {
                result.Append(!Check.NotNull(command.Name) ? Format.Bold(command.Group) : $"{command.Group} ");
            }

            if (Check.NotNull(command.Name))
            {
                result.Append(Format.Bold(command.Name));
            }

            result.AppendLine();

            // Main summary
            if (Check.NotNull(command.MainSummary))
            {
                result.AppendLine($"⇛ {command.MainSummary}");
            }

            // Write overloads
            if (command.Overloads.Count > 1)
            {
                result
                    .AppendJoin("\n", command.Overloads.OrderBy(o => o.Index).Select(o => $"> {o.Syntax}"))
                    .AppendLine();
            }

            return result.ToString();
        }

        public virtual string ViewOverload(OverloadNode overload, BaseUser user = null)
        {
            bool showTooltips = user?.Config.Tooltips ?? true;
            Language language = user?.Config.Language ?? Language.English;

            AddExample(overload, language);

            string summary = Locale.GetValueOrDefault($"{overload.Id}_summary", language);

            var result = new StringBuilder();

            if (showTooltips)
            {
                var tooltips = new List<string>(overload.Tooltips);

                if (overload.Parameters.Count > 0 && overload.Parameters.Any(x => Check.NotNull(x.Summary)))
                {
                    string commandName = $"{overload.Name}{(overload.Count > 1 ? $"+{overload.Index}" : "")}";
                    tooltips.Add(Locale.GetValue("help_tooltip_parameter", language, commandName));
                }

                result.AppendLine($"{Format.Tooltip(tooltips)}\n");
            }

            // page bar, if more than 1
            if (overload.Count > 1)
            {
                result.Append('▰', overload.Index + 1);
                result.Append('▱', overload.Count - (overload.Index + 1));
                result.AppendLine();
            }

            // grouping
            if (Check.NotNull(overload.Group))
            {
                if (!Check.NotNull(overload.Name))
                {
                    result.Append(Format.Bold(overload.Group));
                }
                else
                {
                    result.Append(overload.Group);
                    result.Append(' ');
                }
            }

            if (Check.NotNull(overload.Name))
                result.Append(Format.Bold(overload.Name));

            // parameters
            result.Append('(');

            if (overload.Parameters.Count > 0)
            {
                // implement Parameters
                result.AppendJoin(", ", overload.Parameters.Select(x => x.Syntax));
            }

            result.Append(')');

            // index marker if more than 1
            if (overload.Count > 1)
            {
                result.Append(' ');
                result.Append(Format.Subscript($"+{overload.Index}"));
            }

            // end of line 2
            result.AppendLine();

            // summary

            if (Check.NotNull(summary))
                result.AppendLine($"⇛ {summary}");

            result.AppendLine();

            if (overload.Aliases.Count > 1)
            {
                result.Append($"> **{Locale.GetValue("help_aliases", language)}**: ");
                result.AppendJoin(", ",
                    (Check.NotNull(overload.Group)
                        ? overload.Aliases.Select(x => x.Substring(overload.Group.Length + 1))
                        : overload.Aliases)
                    .Where(x => x != overload.Name)
                    .OrderByDescending(x => x.Length)
                    .Select(x => $"`{x}`"));

                result.AppendLine();
            }

            if (overload.Access.HasValue)
                result.AppendLine($"> **{Locale.GetValue("help_access", language)}**: **{overload.Access.Value.ToString()}**");

            if (overload.Cooldown.HasValue)
                result.AppendLine($"> **{Locale.GetValue("help_cooldown", language)}**: {Format.LongCounter(overload.Cooldown.Value)}");

            if (Check.NotNull(overload.Example))
                result.Append($"> **{Locale.GetValue("help_example", language)}**: `{overload.Example}`");

            return result.ToString();
        }

        public virtual string ViewParameter(ParameterNode parameter, BaseUser user = null)
        {
            bool showTooltips = user?.Config.Tooltips ?? true;
            Language language = user?.Config.Language ?? Language.English;

            string summary = Locale.GetValueOrDefault($"{parameter.Id}_summary", language, parameter.Summary);

            var result = new StringBuilder();

            result.Append($"{Format.Bold(Format.HumanizeType(parameter.ValueType))} {parameter.Name}");

            if (parameter.DefaultValue != null)
            {
                result.Append($" = {parameter.DefaultValue}");
            }

            result.AppendLine();

            // summary
            if (Check.NotNull(summary))
                result.AppendLine($"⇛ {summary}");

            result.AppendLine();

            // info
            if (parameter.ValueType != null)
            {
                if (parameter.ValueType.IsEnum)
                {
                    result.AppendLine($"> **{Locale.GetValue("help_values", language)}**{(parameter.ValueType.GetCustomAttribute<FlagsAttribute>() != null ? $" ({Locale.GetValue("help_flags", language)})" : "")}\n```cs");

                    string[] names = parameter.ValueType.GetEnumNames();
                    Array values = parameter.ValueType.GetEnumValues();

                    for (int i = 0; i < names.Length; i++)
                    {
                        object enumValue = values.GetValue(i);
                        result.AppendLine($"{names[i]} = {Convert.ToInt16(enumValue)}");
                    }

                    result.AppendLine("```");
                }
            }

            result.AppendLine($"> **{Locale.GetValue("help_command", language)}**: `{parameter.CommandId}`");

            // tags
            if (parameter.Tags != 0)
            {
                result.Append($"> **{Locale.GetValue("help_tags", language)}**: ");
                result.AppendJoin(", ", EnumUtils.GetValues<ParameterTag>()
                    .Where(t => parameter.Tags.HasFlag(t))
                    .Select(t => $"`{t.ToString()}`"));
                result.AppendLine();
            }

            // parsing examples
            if (Check.NotNullOrEmpty(parameter.ParseExamples))
            {
                result
                    .Append($"> **{(parameter.ParseExamples.Count() != 1 ? Locale.GetValue("help_example_plural", language) : Locale.GetValue("help_example", language))}**: ")
                    .AppendJoin(", ", parameter.ParseExamples.Select(x => $"`{x}`"))
                    .AppendLine();
            }

            return result.ToString();
        }

        private void AddExample(OverloadNode overload, Language language)
        {
            var result = new StringBuilder();

            // result.Append(prefix); See if no prefix works better or not
            result.Append(overload.Name);

            if (overload.Parameters.Count > 0)
            {
                result.Append(' ');
                result.AppendJoin(" ", overload.Parameters.Select(x => GetExampleParam(x, language)));
            }

            overload.Example = result.ToString();
        }

        private string GetExampleParam(ParameterNode parameter, Language language)
        {
            if (parameter.Tags.HasFlag(ParameterTag.Mentionable))
                return Locale.GetValue("help_mention_example", language);

            if (parameter.Tags.HasFlag(ParameterTag.Trailing))
                return Locale.GetValue("help_trail_example", language);

            return Locale.GetValue("help_default_example", language);
        }

        public string ViewContext(ContextNode ctx, BaseUser user = null)
        {
            return ctx switch
            {
                ModuleNode m when !string.IsNullOrWhiteSpace(m.Group) => ViewGroup(m, user),
                ModuleNode m => ViewModule(m, user),
                CommandNode c => ViewCommand(c, user),
                OverloadNode o => ViewOverload(o, user),
                ParameterNode p => ViewParameter(p, user),
                _ => throw new ArgumentException("An invalid context node was specified")
            };
        }
    }
}
