using System;

namespace Orikivo
{
    // TODO: figure out how to handle a primary summary for a command batch
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BatchSummaryAttribute : Attribute
    {
        public string Summary { get; }

        public BatchSummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
