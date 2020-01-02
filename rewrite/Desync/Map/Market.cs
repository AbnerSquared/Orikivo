using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Market
    {
        string Id;
        string Name;
        string ExteriorImagePath;
        string InteriorImagePath;
        
        List<WorldItemTag> TagTable; // the tags of the groups of items that it can sell.
        List<Vendor> Vendors;

        // Get schedule from Vendor shifts.
    }
}
