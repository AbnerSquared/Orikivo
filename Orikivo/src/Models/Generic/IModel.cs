namespace Orikivo.Models
{
    public interface IModel
    {
        object Id { get; }
    }

    public interface IModel<out TId> : IModel
    {
        new TId Id { get; }

        object IModel.Id => Id;
    }
}