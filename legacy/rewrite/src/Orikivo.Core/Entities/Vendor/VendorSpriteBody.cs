using System.Drawing;

namespace Orikivo
{
    public class VendorSpriteBody
    {
        private readonly Bitmap leftWrist;

        public Bitmap GetLeftWrist()
        {
            return leftWrist;
        }

        public Bitmap LeftBicep {get;} // left bicep sprite.
        public Bitmap LeftHand {get;} // left hand sprite.
        public Bitmap RightWrist {get;} // right wrist sprite.
        public Bitmap RightBicep {get;} // right bicep sprite.
        public Bitmap RightHand {get;} // right hand sprite.
        public Bitmap Torso {get;} // torso sprite.
    }
}