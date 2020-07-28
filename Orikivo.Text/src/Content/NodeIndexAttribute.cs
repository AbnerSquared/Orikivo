using System;

namespace Orikivo.Text
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NodeAttribute : Attribute
    {
        public NodeAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}
