namespace Orikivo.Text
{
    /// <summary>
    /// A ruleset to determine how collision is handled when an <see cref="AsciiCollider"/> collides with the boundaries set by an <see cref="AsciiGrid"/>.
    /// </summary>
    public enum GridCollideMethod
    {
        /// <summary>
        /// Tells the <see cref="AsciiCollider"/> to ignore the boundaries of the <see cref="AsciiGrid"/>.
        /// </summary>
		Ignore = 0,

        /// <summary>
        /// Tells the <see cref="AsciiCollider"/> to wrap the <see cref="AsciiObject"/> around onto the opposite end of where the collision occurred.
        /// </summary>
		Scroll = 1,

        /// <summary>
        /// Tells the <see cref="AsciiCollider"/> to reflect the impact velocity of where the collision occurred.
        /// </summary>
		Reflect = 2,

        /// <summary>
        /// Tells the <see cref="AsciiCollider"/> to stop all velocity in the direction of where the collision occurred.
        /// </summary>
		Stop = 3,
    }
}
