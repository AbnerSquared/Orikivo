using Discord.Commands;
using Orikivo.Text;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // GuideNode : For the list of Guides that can be read (help beginner)
    // ActionNode : For the available actions you can do as your Husk
    public abstract class ContextNode : ContentNode
    {
        protected ContextNode(ModuleInfo module)
        {
            Name = module.Name;
            Aliases = module.Aliases.ToList();
            Summary = module.Summary;
            // Reports = ...
            // Family = ...
        }

        protected ContextNode(CommandInfo command)
        {
            Name = command.Name;
            Aliases = command.Aliases.ToList();
            Summary = command.Summary;
            // Reports = ...
            // Family = ...
        }

        protected ContextNode(ParameterInfo parameter)
        {
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
        public List<IReport> Reports { get; }
        public ContextFamily Family { get; }

        public abstract InfoType Type { get; }

        protected override string Formatting => "";
    }
}
