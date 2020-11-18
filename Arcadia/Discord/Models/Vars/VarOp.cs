namespace Arcadia
{
    public enum VarOp
    {
        /// <summary>
        /// Compares if a <see cref="Var"/> is greater than the specified value.
        /// </summary>
        GTR = 1,

        /// <summary>
        /// Compares if a <see cref="Var"/> is greater than or equal to the specified value.
        /// </summary>
        GEQ = 2,

        /// <summary>
        /// Compares if a <see cref="Var"/> is equal to the specified value.
        /// </summary>
        EQU = 4,

        /// <summary>
        /// Compares if a <see cref="Var"/> is less than or equal to the specified value.
        /// </summary>
        LEQ = 8,

        /// <summary>
        /// Compares if a <see cref="Var"/> is less than the specified value.
        /// </summary>
        LSS = 16,

        /// <summary>
        /// Compares if a <see cref="Var"/> is not equal to the specified value.
        /// </summary>
        NEQ = 32
    }
}
