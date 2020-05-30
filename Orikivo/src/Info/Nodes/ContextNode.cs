using Discord.Commands;
using Orikivo.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public abstract class ContextNode : ContentNode
    {
        private const char GROUP_MARKER = '*';
        private const char MODULE_MARKER = '.';
        private const char COMMAND_MARKER = '(';

        protected ContextNode(ModuleInfo module)
        {
            Id = GetId(module);
            Name = module.Name;
            Aliases = module.Aliases.ToList();
            Summary = module.Summary;
            // Reports = ...
        }

        protected ContextNode(CommandInfo command, bool useIndexing = false)
        {
            Id = GetId(command, useIndexing);
            Name = command.Name;
            Aliases = command.Aliases.ToList();
            Summary = command.Summary;
            // Reports = ...
        }

        protected ContextNode(ParameterInfo parameter)
        {
            Id = GetId(parameter);
            Name = parameter.Name;
            Aliases = null;
            Summary = parameter.Summary;
            // Reports = ...
        }

        public string Id { get; } // derives from Family

        public string Name { get; }
        public List<string> Aliases { get; }
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
                id.Insert(0, Check.NotNull(parent.Group) ? parent.Group + ' ' : parent.Name + MODULE_MARKER);
                parent = parent.Parent;
            }

            id.Append(Check.NotNull(module.Group) ? module.Group + GROUP_MARKER : module.Name + MODULE_MARKER);

            return id.ToString().ToLower();
        }

        public static string GetId(CommandInfo command, bool useOverloadIndex = false)
        {
            var id = new StringBuilder();
            id.Append(GetId(command.Module));

            if (Check.NotNull(command.Name))
            {
                if (Check.NotNull(command.Module.Group))
                    id.Replace(GROUP_MARKER, ' ');

                id.Append(command.Name);
            }

            // if there is more than one instance of this command in existance, get the specific priority for the command IF it was wanted.
            if (useOverloadIndex)
                if (command.Module.Commands.Where(x => x.Name == command.Name).Count() > 1)
                    id.Append($"+{command.Priority}");

            if (Check.NotNull(command.Module.Group) && !Check.NotNull(command.Name))
                id.Replace(GROUP_MARKER, COMMAND_MARKER);
            else
                id.Append(COMMAND_MARKER);

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
