using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class ObjectiveData
    {
        public int CompletedObjectives { get; private set; }
        public DateTime LastAssigned { get; private set; }
        public List<Objective> CurrentObjectives { get; private set; }

        public DateTime LastSkipped { get; private set; }
    }
}
