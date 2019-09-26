namespace Orikivo
{
    /// <summary>
    /// Represents a collection of output format types.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// When available, all output formats are rendered with pixels. Otherwise, it falls back to Embed.
        /// </summary>
        Pixel = 1,

        /// <summary>
        /// All output formats are rendered in embeds.
        /// </summary>
        Embed = 2,

        /// <summary>
        /// All output formats are rendered inside a code block.
        /// </summary>
        Markdown = 4,

        /// <summary>
        /// All output formats are rendered as C# code segments.
        /// </summary>
        Program = 8,

        /// <summary>
        /// All output formats are rendered in extreme detail.
        /// </summary>
        Debug = 16
    }
}