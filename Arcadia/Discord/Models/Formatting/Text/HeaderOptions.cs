namespace Arcadia
{
    // This is used to handle how headers are drawn on text bodies
    public enum HeaderOptions
    {
        Relative = 1, // The header is only shown on the first page (the page bar is isolated as its own title)
        Static = 2 // The header is shown across all pages (the page bar is moved to the header's extra content)
    }
}
