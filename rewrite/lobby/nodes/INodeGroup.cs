using System.Collections.Generic;

namespace Orikivo
{
    public interface INodeGroup<T> : INode
        where T : INode
    {
        List<T> Nodes { get; }
        int AddNode(T content, int? nodeIndex = null);
        void UpdateNode(int index, T content);

        bool UseValueIndex { get; }
        int? PageValueLimit { get; }
        int? Page { get; }

        string ValueSeparator { get; }
        string ValueMap { get; }
        string ContentMap { get; }
        
    }
}
