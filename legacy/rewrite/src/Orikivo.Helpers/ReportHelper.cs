using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Helpers
{
    public class ReportHelper
    {
    }

    public class Report
    {
        public ulong Id { get; } // the report case id.
        public Author Author { get; } // the user that posted this report.
        public CompactCommandInfo Command { get; } // the command that was used to report.
        public OriReportPriorityType Priority { get; } // the priority of the report.
        public string Subject { get; } // the subject of the report.
        public string Summary { get; } // the summary of the report.
        public DateTime CreatedAt { get; } // the time the report was created.
        public DateTime? EditedAt { get; } // the time the report was edited, if any.
        public ReportStatusType Status { get; } // the status of the project.
    }



    public enum ReportStatusType
    {
        Pending = 1,
        Accepted = 2,
        OnHold = 3,
        Completed = 4
    }

    public class CompactCommandInfo
    {
        public CompactCommandInfo(CommandInfo command)
        {
            RootModuleName = command.GetRootModule();
            ModuleName = command.Module.Name;
            if (command.Module.Group.Exists())
                GroupName = command.Module.Group;
            CommandName = command.Name;
            OverloadIndex = command.Priority;
        }

        [JsonProperty("root")]
        public string RootModuleName { get; } // name of the root module.
        [JsonProperty("module")]
        public string ModuleName { get; } // name of the module.
        [JsonProperty("group")]
        public string GroupName { get; } // the group name, if any.
        [JsonProperty("command")]
        public string CommandName { get; } // the main command name.

        [JsonProperty("overload_pos")]
        public int OverloadIndex { get; } // the priority number.
    }
}
