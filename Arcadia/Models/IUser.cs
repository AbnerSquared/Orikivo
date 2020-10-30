namespace Arcadia.Models
{
    public interface IUser : IModel<ulong>
    {
        System.DateTime CreatedAt { get; }
    }
}
