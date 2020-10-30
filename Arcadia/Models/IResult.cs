#nullable enable
namespace Arcadia.Models
{
    public interface IResult
    {
        bool IsSuccess { get; }

        string? ErrorReason { get; }
    }
}
