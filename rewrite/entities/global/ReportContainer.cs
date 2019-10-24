using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // TODO: Synchronize with GitHub.
    /// <summary>
    /// A container that keeps track of all known reports submitted through Orikivo.
    /// </summary>
    public class ReportContainer
    {
        [JsonConstructor]
        internal ReportContainer(int reportIndex, List<ReportInfo> reports)
        {
            CaseCount = reportIndex;
            Reports = reports ?? new List<ReportInfo>();
        }

        internal ReportContainer()
        {
            CaseCount = 0;
            Reports = new List<ReportInfo>();
        }

        /// <summary>
        /// Opens a new report.
        /// </summary>
        public int Open(OriUser user, OverloadDisplayInfo overload, ReportBodyInfo info, params ReportTag[] tags)
        {
            Reports.Add(new ReportInfo(CaseCount, overload, user, info, tags));
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
        public List<ReportInfo> Reports { get; private set; }

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

        public ReportInfo this[int id]
            => Reports.FirstOrDefault(x => x.Id == id);
    }
}
