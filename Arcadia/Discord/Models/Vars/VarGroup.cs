using System;

namespace Arcadia
{
    public class VarGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public VarType Type { get; set; } = VarType.Stat;

        public Func<long, string> ValueWriter { get; set; }

        public Func<ArcadeUser, string> Writer { get; set; }
    }
}