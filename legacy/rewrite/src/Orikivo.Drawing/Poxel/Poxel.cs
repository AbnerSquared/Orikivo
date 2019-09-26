using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.rewrite.src.Orikivo.Drawing.Poxel
{
    // pixel rendering and formatter engine
    public class Poxel
    {
        public Poxel(PoxelOptions options)
        {

        }

        public PoxelUnitScale OutputScale { get; } // the multipling size of Bitmap generations.
        public PoxelFontFace Font { get; } // the default rendering font to be used.
        public PoxelColorPacket Packet { get; } // coloring packet id used to color.

    }
}
