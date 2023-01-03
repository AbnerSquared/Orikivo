namespace Orikivo.Models.Discord
{
    public interface IRole : IModel<ulong>
    {
        string Name { get; }
    }
}