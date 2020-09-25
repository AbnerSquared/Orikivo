namespace Arcadia.Models
{
    public interface IMerit : IModel<string>
    {
        int Rank { get; }
    }
}
