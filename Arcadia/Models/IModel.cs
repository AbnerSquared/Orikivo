namespace Arcadia.Models
{
    public interface IModel<out TId>
    {
        TId Id { get; }

        string Name { get; }
    }
}