namespace Arcadia.Models
{
    public interface IShop : IModel<string>
    {
        string Quote { get; }

        long SellTag { get; }
    }
}
