namespace Arcadia.Old
{
    /// <summary>
    /// Defines the type of argument specified.
    /// </summary>
    public enum GameObjectType
    {
        /// <summary>
        /// The argument requires that an identifier or name of a user be passed.
        /// </summary>
        User = 1,
        /// <summary>
        /// The argument requires that a string be passed.
        /// </summary>
        String = 2,

        /// <summary>
        /// The argument requires that any of the argument fields are passed.
        /// </summary>
        Custom = 3,

        // the arguments requires that a number be passed
        Number = 4
    }
}
