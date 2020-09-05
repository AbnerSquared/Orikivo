namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a collection of modifiers that a <see cref="FontFace"/> might have.
    /// </summary>
    [System.Flags]
    public enum FontTag
    {
        /// <summary>
        /// Marks a <see cref="FontFace"/> as monospace, which ensures that all characters are equal in width.
        /// </summary>
        Monospace = 1,

        /// <summary>
        /// Marks a <see cref="FontFace"/> as Unicode supported.
        /// </summary>
        UnicodeSupported = 2
    }
}
