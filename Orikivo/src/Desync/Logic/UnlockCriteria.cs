using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class UnlockCriteria
    {
        // the id that ensures that you unlocked this
        public string UnlockId { get; set; }
        // represents the list of flags that a husk must have in order to enter.
        public List<string> RequiredFlags { get; set; }
    }
}
