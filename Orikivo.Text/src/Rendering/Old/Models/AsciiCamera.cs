namespace Orikivo.Text
{
    // TODO: Make camera the actual viewport used when rendering.
    /// <summary>
    /// The viewport used to draw an <see cref="AsciiGrid"/> to a frame.
    /// </summary>
    public class AsciiCamera
    {
		public AsciiCamera(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height { get; } // determines how much of a grid is seen at once.
        public int Width { get; } // determines how much of a grid is seen at once.

        /*
        int LeftMoveWidth; // how large is the trigger to move left. ( only when grid is larger than camera. if grid is smaller than camera use a VoidChar.
        int RightMoveWidth; // size of trigger to move camera right.
        int TopMoveHeight; // size of trigger to move camera up.
        int BottomMoveHeight; // size of trigger to move camera down.
        */
    }
}
