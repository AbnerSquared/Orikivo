namespace Orikivo
{
    public interface IPurchasable
    {
        bool TryBuy(OriUser user);
    }
}