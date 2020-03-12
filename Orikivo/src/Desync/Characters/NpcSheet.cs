using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Desync
{

    // Represents all of the images that make up this NPC.
    /// <summary>
    /// Represents a collection of image references for drawing an <see cref="Npc"/>.
    /// </summary>
    public class NpcApparel
    {
        /// <summary>
        /// A list of facial expressions that this <see cref="Npc"/> might use when speaking.
        /// </summary>
        public Dictionary<DialogueTone, Sprite> Reactions { get; set; }
        
        public Sprite Body { get; set; }
        public Point BodyOffset { get; set; }

        public Sprite Head { get; set; }
        public Point HeadOffset { get; set; }

        /// <summary>
        /// Determines where a reaction is drawn when building this <see cref="Npc"/>'s image.
        /// </summary>
        public Point FaceOffset { get; set; }

        public List<NpcOutfit> Outfits { get; set; }
        
        // Facial reactions
        // Overall head design
        // overlay head
        // body frame

        public Sprite GetReactionOrDefault(DialogueTone tone)
        {
            if (Reactions.ContainsKey(tone))
                return Reactions[tone];

            return Reactions[DialogueTone.Neutral];
        }

        // TODO: Move to GraphicsHandler
        public Bitmap GetDisplayImage(DialogueTone tone, GammaPalette palette)
        {
            Bitmap result = new Bitmap(72, 64);

            using (Graphics g = Graphics.FromImage(result))
            {
                using (Bitmap bg = new Bitmap("../assets/npcs/npc_frame.png"))
                    GraphicsUtils.ClipAndDrawImage(g, bg, new Point(0, 0));

                using (Bitmap body = Body.GetImage())
                    GraphicsUtils.ClipAndDrawImage(g, body, BodyOffset);

                using (Bitmap head = Head.GetImage())
                    GraphicsUtils.ClipAndDrawImage(g, head, HeadOffset);

                using (Bitmap face = GetReactionOrDefault(tone).GetImage())
                    GraphicsUtils.ClipAndDrawImage(g, face, FaceOffset);
            }

            result = BitmapHandler.ReplacePalette(result, GammaPalette.Default, palette);
            result = GraphicsUtils.Scale(result, 2, 2);
            return result;
        }
    }

    public class NpcOutfit
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Sprite Torso { get; set; }
    }
}
