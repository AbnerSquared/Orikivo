using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // this class is used to help signify and identify information about a command with overridable variants
    /// <summary>
    /// Represents a specific command function with a set priority.
    /// </summary>
    public class OverloadDisplayInfo : IDisplayInfo
    {
        public OverloadDisplayInfo(CommandInfo command)
        {
            //Console.WriteLine($">>attr\n{string.Join("\n", command.Attributes.Select(x => x.GetType().Name))}");
            //Console.WriteLine($">>precond\n{string.Join("\n", command.Preconditions.Select(x => x.GetType().Name))}");
            if (command.Attributes.Any(x => x.GetType() == typeof(CooldownAttribute)))
                CooldownLength = (command.Attributes.First(x => x.GetType() == typeof(CooldownAttribute)) as CooldownAttribute).Seconds;

            if (command.Preconditions.Any(x => x.GetType() == typeof(RequirePermissionsAttribute)))
                Permissions = (command.Preconditions.First(x => x.GetType() == typeof(RequirePermissionsAttribute)) as RequirePermissionsAttribute).Permissions;

            if (command.Preconditions.Any(x => x.GetType() == typeof(BindToAttribute)))
                TrustLevel = (command.Preconditions.First(x => x.GetType() == typeof(BindToAttribute)) as BindToAttribute).Level;

            ModuleName = command.Module.Name;
            StringBuilder sb = new StringBuilder();
            sb.Append(command.Module.Name);
            if (command.Module.Parent != null)
            {
                ModuleInfo parent = command.Module.Parent;
                while (parent != null)
                {
                    sb.Insert(0, $"{parent.Name}.");
                    parent = parent.Parent;
                }
            }
            _rootName = sb.ToString();

            Name = command.Name;
            Aliases = command.Aliases.Where(x => x != Name).ToList();
            Priority = command.Priority;
            Summary = command.Summary;
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
            Parameters = new List<ParameterDisplayInfo>();
            foreach (ParameterInfo parameter in command.Parameters)
            {
                Parameters.Add(new ParameterDisplayInfo(parameter));
            }
        }

        private string _rootName { get; }
        public string Name { get; }
        public string ModuleName { get; }
        public string GroupName { get; }
        public bool IsInGroup { get; }
        public List<string> Aliases { get; }
        public int Priority { get; }
        public string Summary { get; }
        public double? CooldownLength { get; }
        public TrustLevel? TrustLevel { get; } //if the command is limited to the developer
        public List<GuildPermission> Permissions { get; }
        public List<ParameterDisplayInfo> Parameters { get; }
        public string Id { get { return $"{_rootName}.{Name}+{Priority}"; } }
        public string GetDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"> {(IsInGroup ? $"{GroupName} " : "")}**{Name}**{OriFormat.Subscript($"+{Priority}")}({string.Join(", ", Parameters.Select(x => x.SyntaxName))})");
            if (Permissions != null)
                sb.AppendLine($"> **Permissions Required**: {string.Join(", ", Permissions.Select(x => $"`{x.ToString()}`"))}");
            if (TrustLevel.HasValue)
                sb.AppendLine($"> **Elevation**: {TrustLevel.Value.ToString()}");
            if (CooldownLength.HasValue)
                sb.AppendLine($"> **Cooldown**: {OriFormat.GetShortTime(CooldownLength.Value)}");
            if (Summary != null)
                sb.Append($"> ⇛ {Summary}");
            return sb.ToString();
        }
    }
}
