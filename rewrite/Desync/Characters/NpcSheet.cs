using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Unstable
{
    // Represents all of the images that make up this NPC.
    public class NpcSheet
    {
        public Dictionary<DialogueTone, Sprite> Reactions { get; set; }
        
        public Sprite BodyFrame { get; set; }

        public Sprite HeadFrame { get; set; }

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

        public Bitmap GetDisplayImage(DialogueTone tone, GammaPalette palette)
        {
            Bitmap result = new Bitmap(72, 64);

            using (Graphics g = Graphics.FromImage(result))
            {
                using (Bitmap bg = new Bitmap("../assets/npcs/npc_frame.png"))
                    GraphicsUtils.ClipAndDrawImage(g, bg, new Point(0, 0));

                using (Bitmap body = BodyFrame.GetImage())
                    GraphicsUtils.ClipAndDrawImage(g, body, new Point(20, 16));

                using (Bitmap head = HeadFrame.GetImage())
                    GraphicsUtils.ClipAndDrawImage(g, head, new Point(28, 4));

                using (Bitmap face = GetReactionOrDefault(tone).GetImage())
                    GraphicsUtils.ClipAndDrawImage(g, face, new Point(28, 4));
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
