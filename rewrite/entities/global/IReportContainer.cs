using System.Collections.Generic;

namespace Orikivo
{
    public interface IReportContainer<TReport> where TReport : IReport
    {
        List<TReport> Reports { get; }
        int CaseCount { get; }
    }
}
