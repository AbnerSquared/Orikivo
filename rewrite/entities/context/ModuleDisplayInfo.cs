using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // this class is used to help signify and identify information about a module
    public class ModuleDisplayInfo
    {
        public ModuleDisplayInfo(ModuleInfo module)
        {
            Name = module.Name;
            if (module.Parent != null)
            {
                StringBuilder sb = new StringBuilder();
                
                ModuleInfo parent = module.Parent;
                while (parent != null)
                {
                    sb.Insert(0, $"{parent.Name}.");
                    parent = parent.Parent;
                }

                Parent = sb.ToString();
            }
            else
            {
                Parent = null;
            }

            Summary = module.Summary;
            IsGroup = !string.IsNullOrWhiteSpace(module.Group);
            Group = module.Group;
            Aliases = module.Aliases.Where(x => x != Name).ToList();
            Submodules = new List<ModuleDisplayInfo>();
            Groups = new List<ModuleDisplayInfo>();

            foreach (ModuleInfo submodule in module.Submodules)
            {
                if (submodule.Group != null)
                    Groups.Add(new ModuleDisplayInfo(submodule));
                else
                    Submodules.Add(new ModuleDisplayInfo(submodule));
            }

            Commands = new List<CommandDisplayInfo>();
            List<string> commands = new List<string>();
            List<List<CommandInfo>> commandGroups = new List<List<CommandInfo>>();
            foreach (CommandInfo command in module.Commands)
            {
                if (commands.Contains(command.Name))
                {
                    commandGroups.First(x => x.Any(y => y.Name == command.Name)).Add(command);
                    continue;
                }

                List<CommandInfo> commandGroup = new List<CommandInfo>();
                commandGroup.Add(command);
                commands.Add(command.Name);
                commandGroups.Add(commandGroup);
                continue;
            }

            foreach (List<CommandInfo> commandGroup in commandGroups)
            {
                Commands.Add(new CommandDisplayInfo(commandGroup));
            }
        }

        public string Parent { get; }
        public string Group { get; }
        public string Name { get; }
        public List<string> Aliases { get; }
        public string BlockName { get { return $"`{Name}`{(IsGroup ? "**\\***":"")}"; } }
        public string FlavorText { get; }
        public string Summary { get; }
        public bool IsGroup { get; }
        public List<CommandDisplayInfo> Commands { get; }
        public List<ModuleDisplayInfo> Submodules { get; }
        public List<ModuleDisplayInfo> Groups { get; }
        public int TotalCommands
        {
            get
            {
                return Submodules.Concat(Groups).Sum(x => x.TotalCommands) + Commands.Count;
            }
        }

        public string GetDisplay()
        {
            StringBuilder sb = new StringBuilder();
            if (!IsGroup)
            {
                sb.AppendLine($"{Parent}**{Name}**");
                if (Summary != null)
                {
                    sb.AppendLine($"```\n{Summary}```");
                }

                if (Commands.Count > 0)
                {
                    sb.AppendLine($"**Commands** {OriFormat.Subscript($"({Commands.Count})")}:");
                    sb.AppendLine($"{string.Join(" ", Commands.Select(x => x.BlockName).Concat(Groups.Select(x => x.BlockName)).OrderBy(x => x[1]).ToList())}");
                }
                if (Submodules.Count > 0)
                {
                    sb.AppendLine($"**Submodules** {OriFormat.Subscript($"({Submodules.Count})")}:");
                    sb.AppendLine($"{string.Join("\n", Submodules.Select(x => $"🔹**{x.Name}** {OriFormat.Subscript($"(+{x.TotalCommands})")}"))}");
                }
            }
            else
            {
                sb.AppendLine($"{(Aliases.Count > 0 ? $"[**{Name}**, {string.Join(", ", Aliases.OrderByDescending(x => x.Length))}]" : $"**{Name}**")}");
                sb.AppendLine($"{string.Join("\n", Commands.Select(x => x.GetOverloads()))}");
            }
            return sb.ToString();
        }
    }
}
