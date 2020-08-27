using System.Threading.Tasks;

namespace Arcadia.Casino
{
    public interface ICasinoHandle
    {
        ICasinoResult Execute();
    }

    public interface IResult
    {
        // > ICON
        string Icon { get; }

        // > ICON HEADER
        string Header { get; }

        // > ICON HEADER
        // > SUBTITLE
        string Subtitle { get; }

        // > ICON HEADER
        // > SUBTITLE
        //
        // CONTENT
        string Content { get; }


    }
}
