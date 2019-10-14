using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class CommandDisplayInfo : IDisplayInfo
    {
        public CommandDisplayInfo(CommandInfo command)
        {
            Name = command.Name;
            ModuleName = command.Module.Name;
            Aliases = command.Aliases.Where(x => x != Name).ToList();
            IsInGroup = false;
            if (command.Module.Group != null)
            {
                IsInGroup = true;
                GroupName = command.Module.Group;
                if (string.IsNullOrWhiteSpace(Name))
                    Name = "";

                Aliases = Aliases.Where(x => x.StartsWith($"{command.Module.Group} ")).Select(x => x.Replace($"{GroupName} ", "").Trim()).Where(x => x != Name).ToList();
                if (string.IsNullOrWhiteSpace(command.Name))
                    Aliases.Clear();
            }
            Summary = command.Summary;
            Overloads = new List<OverloadDisplayInfo>();
            Overloads.Add(new OverloadDisplayInfo(command));
        }
        public CommandDisplayInfo(List<CommandInfo> commands)
        {
            CommandInfo command = commands.OrderBy(x => x.Priority).First();
            Name = command.Name;
            Aliases = command.Aliases.Where(x => x != Name).ToList();
            Console.WriteLine(string.Join('\n', Aliases));
            IsInGroup = false;
            if (command.Module.Group != null)
            {
                IsInGroup = true;
                GroupName = command.Module.Group;
                if (string.IsNullOrWhiteSpace(Name))
                    Name = "";

                Aliases = Aliases.Where(x => x.StartsWith($"{command.Module.Group} ")).Select(x => x.Replace($"{GroupName} ", "").Trim()).Where(x => x != Name).ToList();
                if (string.IsNullOrWhiteSpace(command.Name))
                    Aliases.Clear();
            }
            Summary = command.Summary;
            Overloads = new List<OverloadDisplayInfo>();
            foreach (CommandInfo commandInfo in commands)
            {
                Overloads.Add(new OverloadDisplayInfo(commandInfo));
            }
        }
        
        public string Name { get; }
        public string ModuleName { get; }
        public string GroupName { get; }
        public bool IsInGroup { get; }
        public string BlockName { get { return $"`{Name}{(HasMultiple ? $"+{Overloads.Count - 1}" : "")}`"; } }
        public List<string> Aliases { get; }
        public string Summary { get; }
        public List<OverloadDisplayInfo> Overloads { get; } // all variants of a command
        public OverloadDisplayInfo Default => HasMultiple ? Overloads.OrderBy(x => x.Priority).First() : Overloads.First();
        public bool HasMultiple { get { return Overloads.Count > 1; } }
        public string GetOverloads()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join("\n", Overloads.Select(x =>
            {
                StringBuilder ob = new StringBuilder();
                // List<string> name = x.Name.Split(' ').ToList();
                // string.Join(' ', name.Select(y => name.IndexOf(y) == (name.Count - 1) ? $"**{y}**" : y)
                string fullName = x.Name;
                if (x.IsInGroup)
                {
                    fullName = GroupName;
                    if (!string.IsNullOrWhiteSpace(x.Name))
                        fullName += $" {x.Name}";
                }
                List<string> name = fullName.Split(' ').ToList();
                ob.Append($"> {string.Join(' ', name.Select(y => name.IndexOf(y) == (name.Count - 1) ? $"**{y}**" : y))}");
                ob.Append($"{(HasMultiple ? OriFormat.Subscript($"+{x.Priority}") : "")}({string.Join(", ", x.Parameters.Select(y => y.SyntaxName))})");
                return ob.ToString();
            })));
            return sb.ToString();

        }
        public string GetDisplay()
        {
            StringBuilder sb = new StringBuilder();
            if (HasMultiple)
            {
                sb.AppendLine($"{(IsInGroup ? $"{GroupName} " : "")}{(Aliases.Count > 0 ? $"[**{Name}**, {string.Join(", ", Aliases)}]" : $"**{Name}**")}");
                sb.AppendLine($"• **Overloads** {OriFormat.Subscript($"({Overloads.Count})")}:");
                sb.Append(GetOverloads());
            }
            else
            {
                sb.AppendLine($"> {(IsInGroup ? $"{GroupName} " : "")}{(Aliases.Count > 0 ? $"[**{Name}**, {string.Join(", ", Aliases.OrderByDescending(x => x.Length))}]" : $"**{Name}**")}({string.Join(", ", Default.Parameters.Select(x => x.SyntaxName))})");
                if (Default.Permissions != null)
                    sb.AppendLine($"> **Permissions Required**: {string.Join(", ", Default.Permissions.Select(x => $"`{x.ToString()}`"))}");
                if (Default.TrustLevel.HasValue)
                    sb.AppendLine($"> **Elevation**: {Default.TrustLevel.Value.ToString()}");
                if (Default.CooldownLength.HasValue)
                    sb.AppendLine($"> **Cooldown**: {OriFormat.GetShortTime(Default.CooldownLength.Value)}");
                if (Summary != null)
                    sb.AppendLine($"> ⇛ {Default.Summary}");
            }

            return sb.ToString();
        }
    }
}
