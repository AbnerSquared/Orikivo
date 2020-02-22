using System;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a unique cooldown for an identifiable property.
    /// </summary>
    public class CooldownData
    {
        public CooldownData(string id, DateTime expiresOn)
        {
            Id = id;
            ExpiresOn = expiresOn;
        }

        /// <summary>
        /// Gets the unique identifier for the cooldown that it is applied to.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the cooldown expires on.
        /// </summary>
        public DateTime ExpiresOn { get; }
    }
}
