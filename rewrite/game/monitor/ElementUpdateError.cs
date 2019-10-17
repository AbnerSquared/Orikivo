namespace Orikivo
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
