using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Defines the cost and criteria for the tier of an <see cref="Upgrade"/>.
    /// </summary>
    public class UpgradeTier
    {
        public ulong Cost { get; set; }

        public Func<User, bool> Criteria { get; set; }
    }
}
