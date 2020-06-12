using System.Collections.Generic;

namespace Arcadia.Old
{
    public interface IElementConfig
    {
        string ContentFormatter { get; }
        List<char> InvalidChars { get; }
    }
}
