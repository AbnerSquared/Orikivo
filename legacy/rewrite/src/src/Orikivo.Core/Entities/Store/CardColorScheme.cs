using System.Drawing;

namespace Orikivo
{
    // derive from item object
    public class ColorBox
    {
        public ColorBox()
        {

        }

        public ColorBox(string name, Color highlight, Color shadow, HueShift shift)
        {
            Name = name;
            Highlight = highlight;
            Shadow = shadow;
            Shift = shift;
        }

        public string Name { get; set; }
        /// <summary>
        /// The brightest color for the set.
        /// </summary>
        public Color Highlight { get; set; }

        /// <summary>
        /// The darkest color of the set.
        /// </summary>
        public Color Shadow { get; set; }

        /// <summary>
        /// The shift value for each direct color shade.
        /// </summary>
        public HueShift Shift { get; set; }

        /// <summary>
        /// The amount of colors that are in the palette.
        /// </summary>
        public int Size { get; set; }
    }

    public class HueShift
    {
        public HueShift(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }

    public class OldCardColorScheme
    {
        public OldCardColorScheme() { }
        public OldCardColorScheme(string name, ulong cost, Color[] palette)
        {
            Name = name ?? "new-scheme";
            Cost = cost;
            Palette = palette;
        }

        // this is a base class that supports a color scheme card for profile building.
        //public string Icon { get; set; } // the icon used to give off a display.
        public string Name { get; set; } // the name of the scheme module.
        public ulong Cost { get; set; } // the cost of the scheme module.
        public Color[] Palette { get; set; } // the palette the service uses.
                                             //public SchemeRenderingMode Type { get; set; } // the type of color scheme this works.

        /*public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as ItemObject);
        }

        public bool Equals(ItemObject item)
            => Name == item.Name;

        public override int GetHashCode()
            => unchecked(Name.GetHashCode());*/
    }
}