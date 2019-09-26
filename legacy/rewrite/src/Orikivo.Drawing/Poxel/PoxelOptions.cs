using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.rewrite.src.Orikivo.Drawing.Poxel
{
    public class PoxelOptions
    {
        public PoxelUnitScale OutputScale { get; } // lock to x1, x2, x4
        public uint PacketId { get; } // color packet used.
        public uint FontId { get; } // default font face used.
    }
}
