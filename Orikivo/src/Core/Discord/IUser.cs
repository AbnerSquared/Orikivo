namespace Orikivo.Models.Discord
{
    public interface IUser : IModel<ulong>
    {
        string Name { get; }
    }
}