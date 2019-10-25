﻿using Discord;
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
            // TODO: Create as a method in ContextUtils
            if (command.Attributes.Any(x => x.GetType() == typeof(CooldownAttribute)))
                CooldownLength = (command.Attributes.First(x => x.GetType() == typeof(CooldownAttribute)) as CooldownAttribute).Seconds;

            if (command.Preconditions.Any(x => x.GetType() == typeof(RequirePermissionsAttribute)))
                Permissions = (command.Preconditions.First(x => x.GetType() == typeof(RequirePermissionsAttribute)) as RequirePermissionsAttribute).Permissions;

            if (command.Preconditions.Any(x => x.GetType() == typeof(BindToAttribute)))
                TrustLevel = (command.Preconditions.First(x => x.GetType() == typeof(BindToAttribute)) as BindToAttribute).Level;

            Name = command.Name;
            Aliases = ContextUtils.GetAliases(command);
            Priority = command.Priority;
            Summary = command.Summary;

            if (Checks.NotNull(command.Module.Group))
            {
                GroupName = command.Module.Group;

                if (!Checks.NotNull(Name))
                    Name = "";
            }

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
    }
}
