﻿using System;
using Discord.Commands;
using Orikivo.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static System.Reflection.CustomAttributeExtensions;
namespace Orikivo
{
    public abstract class ContextNode : ContentNode
    {
        public const char GroupMarker = '*';
        private const char ModuleMarker = '.'; // Use delimiter name instead?
        private const char CommandMarker = '(';

        protected ContextNode(ModuleInfo module)
        {
            Id = GetId(module);
            Name = module.Name;
            Aliases = module.Aliases.ToList();

            List<string> tooltips = module.Attributes.FirstOrDefault<TooltipAttribute>()?.Tips.ToList() ?? new List<string>();
            Tooltips = tooltips;

            Summary = module.Summary;
        }

        protected ContextNode(CommandInfo command, bool useIndexing = false)
        {
            Id = GetId(command, useIndexing);
            Name = command.Name;
            Aliases = command.Aliases.ToList();
            Summary = command.Summary;
            List<string> tooltips = command.Attributes.FirstOrDefault<TooltipAttribute>()?.Tips.ToList() ?? new List<string>();
            Tooltips = tooltips;
            // Reports = ...
        }

        protected ContextNode(ParameterInfo parameter)
        {
            Id = GetId(parameter);
            Name = parameter.Name;
            Aliases = null;
            Summary = parameter.Summary;
            List<string> tooltips = parameter.Attributes.FirstOrDefault<TooltipAttribute>()?.Tips.ToList() ?? new List<string>();

            if (parameter.Type.IsEnum)
            {
                if (parameter.Type.GetCustomAttribute<FlagsAttribute>() != null)
                    tooltips.Add("This parameter type supports combined flag input.");
                else
                    tooltips.Add("This parameter can be specified by an individual name or number.");
            }

            Tooltips = tooltips;
        }

        protected override bool ReadAttributes => false;

        public string Id { get; } // derives from Family

        public string Name { get; }
        public List<string> Aliases { get; }
        public List<string> Tooltips { get; }
        public string Summary { get; }
        public List<IReport> Reports { get; } = new List<IReport>();
        public abstract InfoType Type { get; }

        public static string GetId(ModuleInfo module)
        {
            var id = new StringBuilder();

            ModuleInfo parent = module.Parent;

            // If the module is in a group.

            // For modules
            while (parent != null)
            {
                id.Insert(0, Check.NotNull(parent.Group) ? parent.Group + ' ' : parent.Name + ModuleMarker);
                parent = parent.Parent;
            }

            id.Append(Check.NotNull(module.Group) ? module.Group + GroupMarker : module.Name + ModuleMarker);

            return id.ToString().ToLower();
        }

        public static string GetId(CommandInfo command, bool useOverloadIndex = false)
        {
            var id = new StringBuilder();
            id.Append(GetId(command.Module));

            if (Check.NotNull(command.Name))
            {
                if (Check.NotNull(command.Module.Group))
                    id.Replace(GroupMarker, ' ');

                id.Append(command.Name);
            }

            // if there is more than one instance of this command in existence, get the specific priority for the command IF it was requested.
            if (useOverloadIndex)
                if (command.Module.Commands.Count(x => x.Name == command.Name) > 1)
                    id.Append($"+{command.Priority}");

            if (Check.NotNull(command.Module.Group) && !Check.NotNull(command.Name))
                id.Replace(GroupMarker, CommandMarker);
            else
                id.Append(CommandMarker);

            return id.ToString().ToLower();
        }

        public static string GetId(ParameterInfo parameter)
        {
            var id = new StringBuilder();
            id.Append(GetId(parameter.Command, true));
            id.Append(parameter.Name);
            return id.ToString().ToLower();
        }

        protected override string Formatting => "";
    }
}
