namespace Arcadia.Old
{
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
        ElementNotFound = 4,

        /// <summary>
        /// The update packet failed to specify an element.
        /// </summary>
        ElementUnspecified = 5,

        ///<summary>
        /// The group specified did not return any results.
        ///</summary>
        GroupNotFound = 6,

        /// <summary>
        /// The update packet failed to specify a group.
        /// </summary>
        GroupUnspecified = 7,

        /// <summary>
        /// The content used to create a new element is unspecified.
        /// </summary>
        ContentUnspecified = 8,

        /// <summary>
        /// The update packet provided was empty.
        /// </summary>
        PacketNullReference = 9,

        /// <summary>
        /// The update packet failed to specify an index.
        /// </summary>
        IndexUnspecified = 10,

        /// <summary>
        /// The index specified was out of range.
        /// </summary>
        IndexOutOfRange = 11,

        /// <summary>
        /// The method specified does not exist.
        /// </summary>
        UnknownMethod = 12,

        ///<summary>
        /// An exception was thrown that failed to meet any of the error criteria.
        ///</summary>
        Exception = 13
    }
}
