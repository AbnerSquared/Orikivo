using Orikivo;
namespace Arcadia.Casino
{
    public interface ICasinoResult
    {
        Message ApplyAndDisplay(ArcadeUser user);
    }
}
