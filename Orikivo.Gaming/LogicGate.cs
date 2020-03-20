namespace Orikivo.Gaming
{
    /// <summary>
    /// Defines a logic gate for comparable values.
    /// </summary>
    public enum LogicGate
    {
        /// <summary>
        /// Defines an argument to return true as long as all specified values are true.
        /// </summary>
        AND = 1,

        /// <summary>
        /// Defines an argument to return true as long as one of the specified values is true.
        /// </summary>
        OR = 2,

        /// <summary>
        /// Defines an argument to return true as long as all of the specified values are false.
        /// </summary>
        NOR = 4,

        /// <summary>
        /// Defines an argument to return true as long as one of the specified values are false.
        /// </summary>
        NAND = 8
    }
}