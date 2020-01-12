using System;

namespace Orikivo
{
    /// <summary>
    /// Defines the cost and criteria for each tier for an <see cref="Upgrade"/>.
    /// </summary>
    public class UpgradeTier
    {
        public ulong Cost { get; set; }

        public Func<Unstable.User, bool> Criteria { get; set; }
    }
}
