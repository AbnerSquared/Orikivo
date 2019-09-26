using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // used alongside StringMap; might be scrapped
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MapProperty : Attribute
    {
        public string Name { get; }

        public MapProperty(string name = null)
        {
            Name = name;
        }
    }
}
