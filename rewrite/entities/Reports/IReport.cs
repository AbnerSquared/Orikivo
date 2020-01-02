using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic report.
    /// </summary>
    public interface IReport
    {
        int Id { get; }

        string CommandId { get; }

        Author Author { get; }

        DateTime CreatedAt { get; }

        List<ReportTag> Tags { get; }

        ReportState State { get; }

        string CloseReason { get; }

        List<VoteInfo> Votes { get; }
    }
}
