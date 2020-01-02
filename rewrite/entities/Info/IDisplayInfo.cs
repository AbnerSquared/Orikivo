using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic display container for a ContextValue.
    /// </summary>
    public interface IDisplayInfo
    {
        string Id { get; }

        string Name { get; }

        string Summary { get; }

        List<string> Aliases { get; }

        List<IReport> Reports { get; }

        List<ContextValue> Family { get; }

        ContextInfoType Type { get; }

        string Content { get; }

        string ToString();
    }
}
