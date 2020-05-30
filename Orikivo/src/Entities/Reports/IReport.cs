using System;
using System.Collections.Generic;

namespace Orikivo
{
    // Moderation class
    // The report system might either be scrapped,
    // or extracted for alternate uses
    /// <summary>
    /// Represents a generic report.
    /// </summary>
    public interface IReport
    {
        int Id { get; }

        string CommandId { get; }

        Author Author { get; }

        DateTime CreatedAt { get; }

        ReportTag Tag { get; }

        ReportState State { get; }

        string CloseReason { get; }

        List<VoteInfo> Votes { get; }
    }
}
