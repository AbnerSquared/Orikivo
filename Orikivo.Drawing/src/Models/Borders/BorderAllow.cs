namespace Orikivo.Drawing
{
    [System.Flags]
    public enum BorderAllow
    {
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        All = Left | Top | Right | Bottom
    }
}
