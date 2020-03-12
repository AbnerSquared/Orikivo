using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class CardName
    {
        public string DesignId { get; set; }

        public int? FontId { get; set; } // Leave empty for default
        public List<int> FontModifierIds { get; set; } // leave empty for nothing

        public int? Gamma { get; set; }
        public int? LocalColorId { get; set; }
        public bool Active { get; set; }
    }
}
