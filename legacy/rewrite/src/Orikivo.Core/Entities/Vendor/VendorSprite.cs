using System.Collections.Generic;
using System.Drawing;

namespace Orikivo
{
    public class VendorSprite
    {
        public VendorSprite()
        {

        }

        public VendorSpritePose Pose {get; private set;} // the position of how the model is laid out.
        public Bitmap Head {get; private set;} // the head model
        public List<VendorSpriteFace> Faces {get; private set;} // the faces based from each interaction format.
        public VendorSpriteBody Body {get; private set;} // the sprites of the body.
    }
}