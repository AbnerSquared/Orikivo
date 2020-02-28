namespace Orikivo.Desync
{
    public enum ItemTag
    {
        Callable,
        Design,
        Physical, // when this tag is specified, the items can only refer to real-world items.
        Digital // when this tag is specified, the items can only refer to digital items.
    }
}
