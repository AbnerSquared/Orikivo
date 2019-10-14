using System;

namespace Orikivo
{
    // used to determine the result of an update.
    public class ElementUpdateResult
    {
        public ElementUpdateMethod Method { get; }
        public ElementMetadata Metadata { get; }
        public bool IsSuccess { get; }

        // this can be used to create a result from an error that occured.
        public static ElementUpdateResult FromError() { throw new NotImplementedException(); }
    }
}
