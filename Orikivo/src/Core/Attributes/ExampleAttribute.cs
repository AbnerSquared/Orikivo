using System;
using System.Collections.Generic;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ExampleAttribute :Attribute
    {
        public IEnumerable<string> Examples { get; }

        public ExampleAttribute(params string[] examples)
        {
            if (examples.Length == 0)
                throw new ArgumentException("You must at least specify one parsing example.");

            Examples = examples;
        }
    }
}
