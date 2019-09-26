using System;
using System.Drawing;

namespace Orikivo.Rendering
{
    // used to render animations.
    // for help, refer to
    // https://github.com/DataDink/Bumpkit/blob/master/BumpKit/BumpKit/GifEncoder.cs
    // in short, it should define
    // the color map
    // the size and height
    // and then the collection of images.
    public class AnimatedImageEncoder //: IDisposable
    {
        public ImageStruct Format;
        public RepeatType RepeatType;
        public int? RepeatAmount;

    }

    public enum RepeatType : int
    {
        Infinite = -1,
        Once = 0,
        Specified = 1
    }

    public class ImageStruct
    {
        public Size Size;
    }
}