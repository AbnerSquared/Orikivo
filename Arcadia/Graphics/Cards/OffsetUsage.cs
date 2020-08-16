namespace Arcadia.Graphics
{
    public enum OffsetUsage
    {
        // The cursor offset specified will be reverted after it is drawn
        Temporary = 1,

        // The cursor offset specified will not be reverted
        Include = 2
    }
}