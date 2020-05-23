using System;

namespace Orikivo
{
    // TODO: figure out how to handle a primary summary for a command batch
    // this will be the summary that appears during
    // a command with multiple overloads
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BaseSummaryAttribute : Attribute
    {
        public string Summary { get; }

        public BaseSummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
