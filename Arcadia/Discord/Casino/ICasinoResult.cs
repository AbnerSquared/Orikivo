using Orikivo;
namespace Arcadia.Casino
{
    public interface ICasinoResult
    {
        CasinoMode Mode { get; }

        // Isolate ApplyAndDisplay message to its own service
        // Instead, follow an Apply(IArcadeUser) structure
        Message ApplyAndDisplay(ArcadeUser user);

        long Reward { get; }

        bool IsSuccess { get; }
    }
}
