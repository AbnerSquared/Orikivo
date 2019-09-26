namespace Orikivo
{
    public class VendorVisualBlock // defines how the vendor is shown visually.
    {
        public ushort WeightId {get;} // the weight used.
        public ushort HeightId {get;} // the height stored.
        public ushort HeadId {get;} // the head map used.
        public ushort FaceId {get;} // the face map used.
        public ushort HairId {get;} // the hair style used.
        public ushort FacialHairId {get;} // the style of facial hair, like beards, goatees, etc.
        public ushort ClothingGroupId {get;} // the style of clothings they may wear, defines the average collection.
        public ushort DefaultUndergarmentId {get;} // stuff like shirts, tank tops, etc.
        public ushort DefaultCoatId {get;} // stuff like sweaters, coats, etc.
        public ushort DefaultNeckwearId {get;} // stuff like ties, necklaces, etc.
        public ushort DefaultMaskId {get;} // stuff like gas masks, doctor's masks, etc.
        public ushort DefaultHeadwearId {get;} // stuff like baseball caps, fedoras, etc.
        public ushort DefaultEyewearId {get;} // stuff like glasses, visors, goggles, etc.
        public ushort DefaultHelmetId {get;} // stuff like bike helmets, astronaut helmets, etc.
        public ushort DefaultLeggingsId {get;} // stuff like jeans, shorts, etc.
        public ushort DefaultFootwearId {get;} // stuff like sandals, boots, sneakers, barefoot.
        public ushort DefaultMarkingId {get;} // markings, like scars, freckles, moles, etc.
        public ushort DefaultTattooId {get;} // tattoos they may have
    }
}