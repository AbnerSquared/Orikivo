using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ParseExampleAttribute :Attribute
    {
        public List<string> Examples { get; }

        public ParseExampleAttribute(params string[] examples)
        {
            if (examples.Length == 0)
                throw new ArgumentException("You must at least specify one parsing example.");

            Examples = examples.ToList();
        }
    }
}
