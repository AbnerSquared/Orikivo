using Orikivo;
namespace Arcadia.Casino
{
    public interface ICasinoResult
    {
        Message ApplyAndDisplay(ArcadeUser user);

        long Reward { get; }

        bool IsSuccess { get; }
    }
}
