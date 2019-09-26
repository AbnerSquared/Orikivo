using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Orikivo
{
    // char[][][] CharMap ; used to label and identify all valid chars that can be used on orikivo's poxel engine.
    public class PoxelFont
    {
        // identifying a font can be external

        // the size of each sprite box
        public Size Size { get; }

        // if the font face supports unicode chars
        public bool IsUnicodeSupported { get; }

        // if the font face is cropped based on the sprite size.
        public bool IsMonospace { get; }

        // all sheets applied to the font
        public List<PoxelFontSheet> SheetRef { get; }

        // gets the char sprite based on the character written.
        public Bitmap GetChar(char c, out (int i, int x, int y) index)
        {
            index = (0, 0, 0);
            // if(!IsMonospace)
            // Poxel.CropChar(charBmp) // returns a bitmap in which the whitespace in a char bitmap is trimmed.
            return null;
        }
    }

    public enum FontTag
    {
        Monospace = 1, // is a monospace font.
        UnicodeSupported = 2 // supports unicode
    }
    public class PoxelFontSheet
    {
        // the directory path of the sheet
        public string Path { get; }
        // the index in comparison to the fontarraymap
        public int Index { get; }
    }
}
