using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class OptionNode
    {
        public string Name { get; protected set; }

        public List<string> Aliases { get; protected set; }

        public Type ValueType { get; protected set; }
    }
}
