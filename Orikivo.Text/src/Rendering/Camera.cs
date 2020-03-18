namespace Orikivo.Text.Rendering
{
    public class Camera
    {
        string Shader; // a string containing brightness values from low to high
        int Width; // the viewport width of the camera
        int Height; // the viewport height of the camera
        char Void; // the character used if there is nothing to draw
        char[,] Screen; // this is always applied over the camera's lens.
    }
}
