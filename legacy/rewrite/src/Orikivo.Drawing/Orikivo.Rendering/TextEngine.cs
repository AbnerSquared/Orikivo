using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Orikivo.Systems.Services;
using System;

namespace Orikivo
{
    public static partial class PixelEngine
    {
        public static List<CardComponent> GetCardComponents(CompactUser user)
        {
            List<CardComponent> components = new List<CardComponent>();

            /*
                The Rendering Limit (SIZE) of a card
                
                Width: 200 pixels
                Height: 150 pixels

                By default, cards render on a 2.0 scale, which enforces the limit to 300 pixels by 400px
                    
                    300px, 400px - The discord embed limit, before pixels start becoming blurry.

                The structure of a Card:
                    [CALLING_CONFIG]
                    - IsUser - A check that determines if the user itself is checking for the card,
                        or if another user is. If another user is checking, the user that is being
                        checked on has their name plastered in the embed of the card.
                    
                    [CARD_CONFIG]

                    [DELAYED] - Font : The font style they wish to use.

                    - Scheme : The collection of colors used for rendering the card.
                    - Type : The type of card that is being rendered
                        (Is it an exp card.... level card...?)

                    [BITMAP_COMPONENTS]
                    - Border : the outline of the card they use to display.
                    - Backdrop : the backdrop used behind the card.

                    [INFO_COMPONENTS] - At least one component has to exist.
                    - Avatar : the icon of a user, or guild.
                    - Username/Nickname : the name of a user/guild.
                    - Activity/State : the current activity being played, or online status.
                    - Status : the status of the user from their status, or online prescence.
                    - Balance : How much the user currently has.
                    - Level : What level the user is currently at.
                    - Experience Bar : the percentage of the completion that the user is on that level.
                    - Merit : the merit a user probably has chosen to display.
            
            
            
            
            
            
            
             */
             return components;
        }

        public static List<RenderedComponent> RenderComponents(List<CardComponent> components)
        {
            List<RenderedComponent> renders = new List<RenderedComponent>();
            foreach(CardComponent component in components)
                renders.Add(RenderComponent(component));

            return renders;
        }

        public static RenderedComponent RenderComponent(CardComponent component)
        {
            RenderedComponent render = new RenderedComponent();
            return render;
        }

        public static Bitmap RenderCard(Card card)
        {
            Bitmap tmp = new Bitmap(1, 1);
            return tmp;
        }







        // In terms of drawing text onto a bitmap.
        public static Bitmap DrawText(Bitmap bmp, string s, Point p, PixelRenderingOptions options)
        {
            // Make catches for when the point is larger than the bmp,
            // whenever the text is too large to fit in the box.

            Bitmap txt = s.Render(options);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle clip = new Rectangle(p, txt.Size);
                g.SetClip(clip);
                g.DrawImage(txt, clip);
                g.ResetClip();
            }
            txt.Dispose();
            return bmp;
        }

        public static Bitmap RenderChar(char c, PixelRenderingOptions options)
        {
            FontFace fallback = FontManager.FontMap.GetFont(0);

            if (c == ' ')
            {
                throw new Exception("A space cannot be rendered in the TextEngine.");
            }

            FontSheet sheet = options.Font.Search(c, options.FontType, out (int i, int x, int y) index) ??
                              fallback.Search(c, options.FontType, out index);

            Rectangle crop = new Rectangle(
                options.Font.Ppu.Width * index.x,
                options.Font.Ppu.Height * index.y,
                options.Font.Ppu.Width,
                options.Font.Ppu.Height);

            using (Bitmap b = new Bitmap(sheet.Path))
            {
                using (Bitmap tmp = b.Clone(crop, b.PixelFormat))
                {
                    if (!options.Font.AutoCrop.HasValue)
                        crop.Width = TextConfiguration.GetWidth(tmp, tmp.Width);
                    else
                    {
                        if (options.Font.AutoCrop.Value)
                            crop.Width = TextConfiguration.GetWidth(tmp, tmp.Width);
                    }
                    crop.Debug();

                    return b.Clone(crop, b.PixelFormat);
                }
            }
        }

        public static Bitmap[] RenderChars(char[] map, PixelRenderingOptions options)
        {
            char[] chars = map.GetRenderMap();
            if (options.Font.HasFallback.HasValue)
                if (!options.Font.HasFallback.Value)
                    chars = chars.Where(x => options.Font.Search(x, options.FontType, out (int i, int x, int y) index).Exists()).ToArray();
            Bitmap[] renders = new Bitmap[chars.Length];
            for (int i = 0; i < renders.Length; i++)
            {
                renders[i] = chars[i].Render(options);
            }

            return renders;
        }

        public static Bitmap RenderString(string str, PixelRenderingOptions options, ulong? fontId = null, int padding = 0)
        {
            char[] map = str.ToCharArray();
            FontFace oldFont = options.Font;
            options.Font = fontId.HasValue ? FontManager.FontMap.GetFont(fontId.Value) : options.Font;
            FontFace font = options.Font;

            Point pointer = new Point(padding, padding); // This is where the top left letter bitmap will start from.
            Size s = str.GetRenderBox(font); // Correct render box built.
            s.Width += padding * 2;
            s.Height += padding * 2;
            s.Debug();
            Rectangle area = new Rectangle(pointer, s);

            Bitmap render = new Bitmap(s.Width, s.Height);
            using (Graphics g = Graphics.FromImage(render))
            {
                Bitmap[] renders = RenderChars(map, options);

                bool overhung = false;

                int x = 0;
                for (int i = 0; i < map.Length; i++)
                {
                    if (map[i] == '\n')
                    {
                        if (overhung)
                        {
                            pointer.Y += font.Overhang;
                            overhung = false;
                        }
                        pointer.Y += font.Ppu.Height + font.Padding;
                        pointer.X = padding;
                        continue;
                    }
                    if (map[i] == ' ')
                    {
                        pointer.X += font.Spacing;
                        continue;
                    }
                    if (map[i] == '⠀') // Invisible Braille Block
                    {
                        pointer.X += 4; // The width of all braille.
                        continue;
                    }
                    if (font.HasFallback.HasValue)
                        if (!font.HasFallback.Value)
                            if (!font.Search(map[i], options.FontType, out (int i, int x, int y) index).Exists())
                                continue;
                    if (map[i].Overhangs())
                    {
                        overhung = true;
                        pointer.Y += font.Overhang;
                    }
                    // if there is no fallback, if the letter doesnt exist, skip it.

                    using (Bitmap b = renders[x])
                    {
                        Rectangle rct = new Rectangle(pointer, b.Size);

                        g.SetClip(rct);
                        g.DrawImage(b, rct);
                        g.ResetClip();

                        pointer.X += rct.Width + font.Padding;
                    }

                    if (map[i].Overhangs())
                        pointer.Y -= font.Overhang;

                    x++;
                }
            }

            options.Font = oldFont;
            return render.Color(options.Palette);
        }
    }
}