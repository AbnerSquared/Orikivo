using System.Collections.Generic;

namespace Orikivo.Desync
{
    // generalizes the person that can receive this Dialog. If this criteria is not met, the dialog cannot be used.
    public class DialogRecipient
    {
        // this is the relationship level this user has to be with this NPC.
        public RelationshipLevel Level { get; set; }


        // the person receiving this has to have these specified flags
        public List<string> Flags { get; set; }
    }

}
