using Orikivo.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace Orikivo.Desync
{

    // Represents all of the images that make up this NPC.
    /// <summary>
    /// Represents a collection of image references for drawing a <see cref="Character"/>.
    /// </summary>
    public class CharacterModel
    {
        /// <summary>
        /// Stores a list of facial expressions a character might use.
        /// </summary>
        public Dictionary<DialogTone, AppearanceNode> Expressions { get; set; }

        public AppearanceNode Head { get; set; }

        public AppearanceNode Body { get; set; }

        public Point DefaultFaceOffset { get; set; }

        public List<CharacterOutfit> Outfits { get; set; }
        
        public AppearanceNode GetReactionOrDefault(DialogTone tone)
        {
            if (Expressions.ContainsKey(tone))
                return Expressions[tone];

            return Expressions[DialogTone.Neutral];
        }

        // TODO: Move to GraphicsHandler
        public Bitmap Render(DialogTone tone, GammaPalette palette)
        {
            Bitmap result = new Bitmap(72, 64);

            using (Graphics g = Graphics.FromImage(result))
            {
                // replace this with the interior images provided from a Location.
                using (Bitmap bg = new Bitmap("../assets/npcs/npc_frame.png"))
                    ImageHelper.ClipAndDrawImage(g, bg, new Point(0, 0));

                using (Bitmap body = Body.Value.Load())
                    ImageHelper.ClipAndDrawImage(g, body, Body.GetOffset());

                using (Bitmap head = Head.Value.Load())
                    ImageHelper.ClipAndDrawImage(g, head, Head.GetOffset());

                using (Bitmap face = GetReactionOrDefault(tone).Value.Load())
                    ImageHelper.ClipAndDrawImage(g, face, DefaultFaceOffset);
            }

            result = ImageHelper.SetColorMap(result, GammaPalette.Default, palette);
            result = ImageHelper.Scale(result, 2, 2);
            return result;
        }
    }
}
