namespace Orikivo
{
    /// <summary>
    /// Defines the method used when updating an element.
    /// </summary>
    public enum ElementUpdateMethod
    {
        ///<summary>
        /// Sets an element at a tab to a specified value.
        ///</summary>
        Set = 1,

        ///<summary>
        /// Appends an element into the end of a tab.
        ///</summary>
        Add = 2,

        ///<summary>
        /// Inserts an element into a tab at a specified position.
        ///</summary>
        Insert = 3,

        ///<summary>
        /// Removes an element from a tab.
        ///</summary>
        Remove = 4,
        
        ///<summary>
        /// Sets an element within a group to a specified value.
        ///</summary>
        SetAtGroup = 5,

        ///<summary>
        /// Appends an element to the end of the group.
        ///</summary>
        AddToGroup = 6,

        ///<summary>
        /// Inserts an element into a group at a specified position.
        ///</summary>
        InsertAtGroup = 7,

        ///<summary>
        /// Removes an element from a group.
        ///</summary>
        RemoveFromGroup = 8,

        ///<summary>
        /// Deletes all elements in a group.
        ///</summary>
        ClearGroup = 9
    }
}
