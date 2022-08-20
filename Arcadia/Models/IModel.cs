namespace Arcadia.Models
{
    public interface IModel
    {
        object Id { get; }

        string Name { get; }
    }

    public interface IModel<out TId> : IModel
    {
        new TId Id { get; }

        object IModel.Id => Id;
    }
}
