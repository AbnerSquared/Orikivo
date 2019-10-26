﻿namespace Orikivo
{
    // TODO: Implement StringResource
    /// <summary>
    /// Utilized with MessagePacket, this handles creating custom embed formats.
    /// </summary>
    public class Embedder
    {
        public static Embedder Default
            => new Embedder { Color = OriColor.OriGreen };

        public StringLocale Locale { get; set; } = StringLocale.English;
        public OriColor Color { get; set; }
        public string Footer { get; set; }
        public string Header { get; set; }
    }

    // TODO: Create derivable types that can support custom information.
    // Most likely will focus on ReportMessagePacket instead.
    /*
    public class ReportEmbedder : Embedder
    {
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string CloseReason { get; set; }
        public int Id { get; set; }
        public bool IsClosed { get; set; }
        public new string Content => "";
    }
    */
}