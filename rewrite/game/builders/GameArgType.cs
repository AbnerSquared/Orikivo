namespace Orikivo
{
    /// <summary>
    /// Defines the type of argument specified.
    /// </summary>
    public enum GameArgType
    {
        /// <summary>
        /// The argument requires that an identifier or name of a user be passed.
        /// </summary>
        User = 1,
        /// <summary>
        /// The argument requires that a string be passed.
        /// </summary>
        Message = 2,

        /// <summary>
        /// The argument requires that any of the argument fields are passed.
        /// </summary>
        Custom = 3
    }
}
