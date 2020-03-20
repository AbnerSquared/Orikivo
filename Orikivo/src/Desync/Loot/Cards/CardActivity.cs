namespace Orikivo.Desync
{
    public class CardActivity
    {
        public string DesignId { get; set; }
        public int? LocalColorId { get; set; }
        public int? Gamma { get; set; } // leave empty for design default (if a design has color layers, limit the gamma that can be set
        public bool Active { get; set; }
    }
}
