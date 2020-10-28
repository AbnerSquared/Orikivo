using System;
using System.Collections.Generic;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class TooltipAttribute : Attribute
    {
        public TooltipAttribute(params string[] tips)
        {
            Tips = tips;
        }

        public IEnumerable<string> Tips { get; }
    }
}
