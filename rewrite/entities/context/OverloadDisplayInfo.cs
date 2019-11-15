using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents a specific command function with a set priority.
    /// </summary>
    public class OverloadDisplayInfo : IDisplayInfo
    {
        internal OverloadDisplayInfo(CommandInfo command, List<ContextValue> family = null)
        {
            CooldownLength = command.Attributes.GetAttribute<CooldownAttribute>()?.Seconds;
            Permissions = command.Preconditions.GetAttribute<RequirePermissionsAttribute>()?.Permissions;
            TrustLevel = command.Preconditions.GetAttribute<BindToAttribute>()?.Level;

            Name = command.Name ?? string.Empty;
            Aliases = ContextUtils.GetAliases(command);
            Priority = command.Priority;
            Summary = command.Summary;
            GroupName = command.Module.Group;

            if (Checks.NotNull(family))
                family.Add(new ContextValue(command));

            Family = family ?? ContextUtils.GetFamilyTree(command);
            Parameters = command.Parameters.Select(x => new ParameterDisplayInfo(x, Family)).ToList();
        }

        public ContextInfoType Type => ContextInfoType.Overload;

        /// <summary>
        /// The entire family branch leading up to this overload.
        /// </summary>
        public List<ContextValue> Family { get; }

        public List<IReport> Reports { get; }

        public string Name { get; }

        public string GroupName { get; }

        public bool IsInGroup => Checks.NotNull(GroupName);

        public List<string> Aliases { get; }

        public int Priority { get; }

        public string Summary { get; }

        public double? CooldownLength { get; }

        public TrustLevel? TrustLevel { get; }

        // TODO: Use the [Flag] variant to store this, as opposed to each flag value.
        public List<GuildPermission> Permissions { get; }

        public List<ParameterDisplayInfo> Parameters { get; }

        public string Id => $"{ContextUtils.ConcatFamilyTree(Family)}+{Priority}";

        public string Content => ContextUtils.WriteDisplayContent(this);

        public override string ToString()
            => Content;
    }
}
