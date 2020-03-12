using Discord.Commands;
using Orikivo.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // GuideNode : For the list of Guides that can be read (help beginner)
    public abstract class ContextNode : ContentNode
    {
        protected ContextNode(ModuleInfo module)
        {
            Id = GetId(module);
            Name = module.Name;
            Aliases = module.Aliases.ToList();
            Summary = module.Summary;
            // Reports = ...
            // Family = ...
        }

        protected ContextNode(CommandInfo command, bool useIndexing = false)
        {
            Id = GetId(command, useIndexing);
            Name = command.Name;
            Aliases = command.Aliases.ToList();
            Summary = command.Summary;
            // Reports = ...
            // Family = ...
        }

        protected ContextNode(ParameterInfo parameter)
        {
            Id = GetId(parameter);
            Name = parameter.Name;
            Aliases = null;
            Summary = parameter.Summary;
            // Reports = ...
            // Family = ...
        }

        public string Id { get; } // derives from Family

        public string Name { get; }
        public List<string> Aliases { get; }
        public string Summary { get; }
        public List<IReport> Reports { get; } = new List<IReport>();
        public abstract InfoType Type { get; }

        public static string GetId(ModuleInfo module)
        {
            StringBuilder id = new StringBuilder();

            ModuleInfo parent = module.Parent;

            // If the module is in a group.

            // For modules
            while(parent != null)
            {
                id.Insert(0, Check.NotNull(parent.Group) ? parent.Group + " " : parent.Name + ".");
                parent = parent.Parent;
            }

            id.Append(Check.NotNull(module.Group) ? module.Group + "*" : module.Name + ".");

            return id.ToString().ToLower();
        }

        public static string GetId(CommandInfo command, bool useOverloadIndex = false)
        {
            StringBuilder id = new StringBuilder();
            id.Append(GetId(command.Module));

            if (Check.NotNull(command.Name))
            {
                if (Check.NotNull(command.Module.Group))
                    id.Replace('*', ' ');

                id.Append(command.Name);
            }

            // if there is more than one instance of this command in existance, get the specific priority for the command IF it was wanted.
            if (useOverloadIndex)
                if (command.Module.Commands.Where(x => x.Name == command.Name).Count() > 1)
                    id.Append($"+{command.Priority}");

            if (Check.NotNull(command.Module.Group) && !Check.NotNull(command.Name))
                id.Replace('*', '(');
            else
                id.Append('(');

            return id.ToString().ToLower();
        }

        public static string GetId(ParameterInfo parameter)
        {
            StringBuilder id = new StringBuilder();
            id.Append(GetId(parameter.Command, true));
            id.Append(parameter.Name);
            return id.ToString().ToLower();
        }

        protected override string Formatting => "";
    }
}
