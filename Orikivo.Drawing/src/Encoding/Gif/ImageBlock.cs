namespace Orikivo.Drawing.Encoding.Gif
{
    // Exactly one Image Descriptor must be present per image in the Data Stream.
    // An unlimited number of images may be present per Data Stream.

    // This represents a single image to be added into a GIF.
    public class ImageBlock
    {
        public ImageDescriptor LocalImageDescriptor;

        // This can be ignored
        public ColorTriplet[] LocalColorTable;

        // This byte determines the initial number
        // of bits used for LZW codes in the image data
        public byte LZWMinCodeSize;

        // This stores all of the indexes for each pixel
        // in the image data.
        public DataBlock[] ImageData;
    }
}
