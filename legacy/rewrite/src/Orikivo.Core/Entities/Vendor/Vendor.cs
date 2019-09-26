namespace Orikivo
{
    public class Vendor // a person who can talk, buy, sell, and act in certain ways
    {
        public Vendor() // make a new vendor at random.
        {

        }

        public Vendor(CachedVendor cache) // reload a vendor from a cache.
        {

        }

        public VendorCharacterBlock Character {get; private set;} // defines everything about the vendor.
        public VendorDesignBlock Design {get; private set;} // defines how the vendor will look.
        public VendorPersonalityType Personality {get; private set;} // defines how the vendor acts.
        public VendorMoodType Mood {get; private set;} // determine mood based on interactions and whatknot.
        // private List<VendorInteraction> Interactions {get; private set;} // keep track of this on the user.
        public VendorSprite Sprite {get; private set;} // the sprite of the vendor.
    }

    public class VendorDesignBlock { }
}