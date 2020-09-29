using Orikivo;

namespace Arcadia.Casino
{
    public interface ICasinoResult
    {
        CasinoMode Mode { get; }

        Message ApplyAndDisplay(ArcadeUser user);

        long Reward { get; }

        bool IsSuccess { get; }
    }
}
