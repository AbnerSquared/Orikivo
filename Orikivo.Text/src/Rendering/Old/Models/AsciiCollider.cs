namespace Orikivo.Text
{
    /// <summary>
    /// A collider used to determine collision for an <see cref="AsciiObject"/>.
    /// </summary>
    public class AsciiCollider
    {
        /// <summary>
        /// Creates a new <see cref="AsciiCollider"/> with the specified width, height, and method.
        /// </summary>
        public AsciiCollider(int width, int height, GridCollideMethod method = GridCollideMethod.Ignore)
        {
            Width = width;
            Height = height;
            GridCollider = method;
        }

        // this is used to detect when something is in range of colliding.
        // private const int COLLISION_DETECTION_RANGE = 2;

        /// <summary>
        /// The width of the collider (in characters).
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the collider (in characters).
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The method that is used to determine how to handle velocity when the <see cref="AsciiCollider"/> collides with the boundaries of an <see cref="AsciiGrid"/>.
        /// </summary>
        public GridCollideMethod GridCollider { get; }

        /*
        /// <summary>
        /// A cushion around the collider that helps detect when it is about to collide with another <see cref="AsciiCollider"/>.
        /// </summary>
        public Padding Detector { get; }
        */
	}
}
