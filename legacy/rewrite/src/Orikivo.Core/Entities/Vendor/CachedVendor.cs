using System.Collections.Generic;

namespace Orikivo
{
    public class CachedVendor // a cache block defining all plates of information.
    {
        public string Data {get; private set;} // the string defining their overall personality.
        public string SpriteCache {get; private set;} // the string defining their appearance.
        public List<CachedVendorInteraction> Interactions { get; private set; } // a collection of interactions between each user.
    }
}