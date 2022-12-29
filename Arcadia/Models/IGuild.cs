namespace Arcadia.Models
{
    public interface IGuild : IModel<ulong>
    {
        System.DateTime CreatedAt { get; }
    }
}
