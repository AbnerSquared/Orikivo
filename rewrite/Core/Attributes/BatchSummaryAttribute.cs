using System;

namespace Orikivo
{
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
