using System;

namespace Arcadia
{
    /// <summary>
    /// Represents a basic requirement.
    /// </summary>
    public class Criterion
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public Func<CriterionContext, bool> Judge { get; set; }

        public CriterionTriggers Triggers { get; set; }
    }
}
