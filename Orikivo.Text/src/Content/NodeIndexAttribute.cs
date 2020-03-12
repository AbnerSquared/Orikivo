using System;

namespace Orikivo.Text
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : Attribute
    {
        public NodeAttribute(int index) : base()
        {
            Index = index;
        }

        public int Index { get; }
    }
}
