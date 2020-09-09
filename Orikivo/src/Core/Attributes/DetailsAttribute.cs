using System;
using System.Collections.Generic;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DetailsAttribute : Attribute
    {
        public DetailsAttribute(params string[] details)
        {
            Details = details;
        }

        public IEnumerable<string> Details { get; }
    }
}
