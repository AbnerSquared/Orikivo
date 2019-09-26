using System;

namespace Orikivo
{
    /// <summary>
    /// Represents the humane side of an Account.
    /// </summary>
    public class PersonalityChart
    {
        /// <summary>
        /// The actual name of a user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The birthday date of a user.
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// A brief description describing what the user wishes.
        /// </summary>
        public string Summary { get; set; }
    }
}
