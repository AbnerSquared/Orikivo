using System.Collections.Generic;
namespace Arcadia.Old
{
    public interface IElementGroup<T> : IElement
        where T : IElement
    {
        List<T> Elements { get; }
        string ElementFormatter { get; }

        int ElementCount { get; }

        T ElementAt(int index);
        T GetElement(string id);
    }
}
