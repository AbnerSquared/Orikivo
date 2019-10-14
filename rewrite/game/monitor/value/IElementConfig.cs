using System.Collections.Generic;

namespace Orikivo
{
    public interface IElementConfig
    {
        string ContentFormatter { get; }
        List<char> InvalidChars { get; }
    }
}
