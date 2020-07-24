namespace Arcadia
{
    public enum ItemAttributeType
    {
        // If static, the attribute is only referenced within Item, and doesn't change.
        Static = 1,

        // If dynamic, the attribute is transferred over to the UniqueItemData class, where it is then handled.
        Dynamic = 2
    }
}