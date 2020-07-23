using Discord.Commands;
using Orikivo.Desync;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class DesyncInfoFormatter : InfoFormatter
    {
        public override List<GuideNode> OnLoadGuides()
        {
            return new List<GuideNode>
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
        }

        public override string OnWriteMenu(InfoService service, BaseUser user = null)
        {
            bool showTooltips = user?.Config?.Tooltips ?? true;

            var menu = new StringBuilder();

            menu.AppendLine(GetMenuHeader(showTooltips));
            menu.Append(GetMenuGuides());
            menu.Append(GetMenuCategories(service));

            return menu.ToString();
        }

        private string GetMenuCategories(InfoService service)
        {
            if (service.Modules.Any())
                return "";

            var segment = new StringBuilder();
            var modules = service.GetBaseModules().Select(x => new ModuleNode(x));

            foreach (ModuleNode module in modules)
                segment.Append(GetCategory(module));

            return segment.ToString();
        }

        private static readonly int _innerNameLimit;

        private string GetCategory(ModuleNode module)
        {
            var segment = new StringBuilder();

            segment.Append("> ");

            // if (showReportStatus) panel.Append(GetSeverityIcon(module));
            segment.Append($"**{module.Name}**");

            bool hasSubtitle = !string.IsNullOrWhiteSpace(module.Subtitle);

            if (hasSubtitle || module.InnerCount > 0)
                segment.Append(": ");

            if (hasSubtitle)
                segment.AppendLine(module.Subtitle);

            if (module.InnerCount > 0)
            {
                if (hasSubtitle)
                    segment.Append("> ");

                var innerNames = GetModuleInnerNames(module);
                int inserted = 0;

                foreach(string innerName in GetModuleInnerNames(module))
                {
                    if (inserted >= _innerNameLimit)
                        break;

                    if (inserted > 0)
                        segment.Append(" ");

                    segment.Append(innerName);
                    inserted++;
                }

                if (module.InnerCount - inserted > 0)
                    segment.Append($" (+**{module.InnerCount - inserted}** more)");

                segment.AppendLine();
            }
            else
            {
                segment.AppendLine("...");
            }

            return segment.ToString();
        }

        private static IEnumerable<string> GetModuleInnerNames(ModuleNode module)
        {
            var names = new List<string>();

            names.AddRange(module.Commands.Select(x => $"`{x.Name}`"));
            names.AddRange(module.Submodules.Where(x => x.IsGroup).Select(x => $"`{x.Name}`**\\***"));

            return names.OrderBy(x => x.Substring(1));
        }

        private string GetMenuGuides()
        {
            var guides = OnLoadGuides();

            if (guides.Count == 0)
                return "";

            var segment = new StringBuilder();

            segment.AppendLine("\n**Guides**");

            foreach (GuideNode guide in guides)
                segment.AppendLine($"> {guide.Tooltip}");

            return segment.ToString();
        }

        private string GetMenuHeader(bool showTooltips = true)
        {
            var header = new StringBuilder();

            header.AppendLine("> **Help Menu**");

            if (showTooltips)
                header.AppendLine("> Use `help <name>` to learn more about a command, module, or action.");

            return header.ToString();
        }

        public string GetActions(InfoService service, User user)
        {
            var segment = new StringBuilder();
            bool showTooltips = user?.Config?.Tooltips ?? true;
            
            segment.AppendLine("**Actions**");

            if (user?.Husk == null)
            {
                if (user.Brain.HasFlag(DesyncFlags.Initialized))
                    return "*\"I couldn't seem to establish a connection to you.\"*";

                return "";
            }

            if (showTooltips)
                segment.AppendLine("> *This is everything that you are able to execute.*");

            segment.Append("• ");

            Locator location = user.Husk.Location;

            bool hasDestination = user.Husk.Destination != null;

            if (hasDestination)
                location = user.Husk.Destination;

            segment.AppendLine(Engine.WriteLocationInfo(location.Id, hasDestination));

            ModuleInfo core = service.InternalModules.FirstOrDefault(x => x.Name == "Actions");
            var actions = GetAvailableActions(service, core, user);

            // This is now writing each action
            if (actions.Count() > 0)
                segment.AppendJoin(" • ", actions.Select(x => WriteAction(x)));
            else
                segment.Append("No available actions to execute.");

            return segment.ToString();
        }

        private string WriteAction(CommandNode command)
        {
            string action = $"`{command.Name}`";

            if (command.Overloads.Count > 1)
                action += $"**+{command.Overloads.Count - 1}**";

            return action;
        }

        private IEnumerable<CommandNode> GetAvailableActions(InfoService service, ModuleInfo core, User user)
        {
            var actions = new List<CommandNode>();
            bool canMove = Engine.CanMove(user, user.Husk);
            bool canAct = Engine.CanAct(user);

            // This is compiling all of the available actions together
            foreach (CommandInfo action in core?.Commands)
            {
                if (actions.Any(x => x.Name == action.Name))
                    continue;

                var precondition = action.Preconditions.FirstOrDefault<OnlyWhenAttribute>();

                if (precondition != null)
                {
                    if (!precondition.Judge(user))
                        continue;
                }

                var bind = action.Attributes.FirstOrDefault<BindToRegionAttribute>();

                if (bind != null)
                {
                    if (!canMove || !canAct)
                        continue;

                    if (!bind.Judge(user.Husk))
                        continue;
                }

                actions.Add(new CommandNode(service.GetCommands(action.Name, core)));
            }

            return actions;
        }
    }
}
