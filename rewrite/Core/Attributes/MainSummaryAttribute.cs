using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MainSummaryAttribute : Attribute
    {
        public string Summary { get; }

        public MainSummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
