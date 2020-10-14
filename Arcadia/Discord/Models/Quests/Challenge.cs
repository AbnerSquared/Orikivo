using System.Collections.Generic;

namespace Arcadia
{
    public class Challenge
    {
        public string Name { get; set; }

        public int Difficulty { get; set; } // The higher the number, the harder it is

        public List<VarCriterion> Criteria { get; internal set; }
    }
}
