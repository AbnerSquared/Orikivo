﻿namespace Orikivo.Desync
{
    // for Arcadia
    public class CardLevel
    {
        public string DesignId { get; set; } // leave empty for default
        public int? LocalColorId { get; set; }
        public int? FontId { get; set; }
        public int? Gamma { get; set; }
        public bool Active { get; set; }
    }
}
