using System;

namespace Orikivo
{
    ///<summary>
    /// An object defining the results of an element update packet.
    ///</summary>
    public class ElementUpdateResult
    {
        private ElementUpdateResult() { }

        ///<summary>
        /// The method that was specified when updating an element.
        ///</summary>
        public ElementUpdateMethod Method { get; private set; }

        ///<summary>
        /// The metadata defining the element that was created within a group or tab.
        ///</summary>
        public ElementMetadata Metadata { get; private set; }

        public bool IsSuccess => Error.HasValue;

        ///<summary>
        /// The type of error that occured when updating an element. Can be left empty.
        ///</summary>
        public ElementUpdateError? Error { get; private set; } = null;

        ///<summary>
        /// The reason an error occured when updating an element. Can be left empty.
        ///</summary>
        public string ErrorReason { get; private set; }

        // this can be used to create a result from an error that occured.

        ///<summary>
        /// Creates a successful update result with its element metadata and update method.
        ///</summary>
        public static ElementUpdateResult FromSuccess(ElementUpdateMethod method, ElementMetadata metadata)
            => new ElementUpdateResult() { Method = method, Metadata = metadata };

        ///<summary>
        /// Creates a failed update result specified by an error type and optional message.
        ///</summary>
        public static ElementUpdateResult FromError(ElementUpdateError error, string message = null)
            => new ElementUpdateResult() { Error = error, ErrorReason = message };

        ///<summary>
        /// Creates a failed update result specified by an Exception.
        ///</summary>
        public static ElementUpdateResult FromError(Exeception exception)
            => new ElementUpdateResult() { Error = ElementUpdateError.Exception, ErrorReason = exception.Message };
    }

    // TODO: Separate ElementUpdateError

    ///<summary>
    /// Defines the type of error that occurred when updating an element.
    ///</summary>
    public enum ElementUpdateError
    {
        ///<summary>
        /// The element specified is immutable and cannot be deleted.
        ///</summary>
        ImmutableElement = 1,

        ///<summary>
        /// The tab can no longer insert any new elements.
        ///</summary>
        TabCapacityReached = 2,

        ///<summary>
        /// The group can no longer insert any new elements.
        ///</summary>
        GroupCapacityReached = 3,

        ///<summary>
        /// The element specified did not return any results.
        ///</summary>
        ElementNullReference = 4,

        ///<summary>
        /// The group specified did not return any results.
        ///</summary>
        GroupNullReference = 5,

        ///<summary>
        /// An exception was thrown that failed to meet any of the error criteria.
        ///</summary>
        Exception = 6
    }
}
