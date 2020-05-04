using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class UnlockCriteria
    {
        /// <summary>
        /// The ID used to ensure this criteria has already been met.
        /// </summary>
        public string UnlockId { get; set; }

        /// <summary>
        /// Represents the list of flags a Husk must have in order to complete this criteria.
        /// </summary>
        public List<string> RequiredFlags { get; set; }

        public bool Judge(Husk husk, HuskBrain brain)
        {
            if (brain.HasFlag(UnlockId))
                return true;

            foreach (string flag in RequiredFlags)
                if (!brain.HasFlag(flag))
                    return false;

            brain.SetFlag(UnlockId);
            return true;
        }
    }
}
