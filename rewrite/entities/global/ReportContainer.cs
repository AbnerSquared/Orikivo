using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // TODO: Synchronize with GitHub.
    /// <summary>
    /// A container that keeps track of all known reports submitted through Orikivo.
    /// </summary>
    public class ReportContainer : IReportContainer<Report>
    {
        [JsonConstructor]
        internal ReportContainer(int caseCount, List<Report> reports)
        {
            CaseCount = caseCount;
            Reports = reports ?? new List<Report>();
        }

        internal ReportContainer()
        {
            CaseCount = 0;
            Reports = new List<Report>();
        }

        /// <summary>
        /// Opens a new report.
        /// </summary>
        public int Open(OriUser user, OverloadDisplayInfo overload, ReportBody info, params ReportTag[] tags)
        {
            Reports.Add(new Report(CaseCount, overload, user, info, tags));
            int id = CaseCount;
            CaseCount++;
            return id;
        }

        /// <summary>
        /// Closes the specified report with an optional reason.
        /// </summary>
        public void Close(int id, string reason = null)
            => this[id].Close(reason);

        /// <summary>
        /// Returns the next available case ID.
        /// </summary>
        [JsonProperty("cases")]
        public int CaseCount { get; private set; }

        /// <summary>
        /// The list of all reports.
        /// </summary>
        [JsonProperty("reports")]
        public List<Report> Reports { get; }

        /// <summary>
        /// The count of all existing reports.
        /// </summary>
        [JsonIgnore]
        public int Count => Reports.Count;

        /// <summary>
        /// Checks to see if there is a report at the specified ID.
        /// </summary>
        public bool Contains(int id)
            => Reports.Any(x => x.Id == id);

        public Report this[int id]
            => Reports.FirstOrDefault(x => x.Id == id);
    }
}
